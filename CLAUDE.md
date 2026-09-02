# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Limbo.Umbraco.Vimeo` is a single-project NuGet package for Umbraco: a property editor that lets editors paste a Vimeo URL or embed code, fetches video metadata from the Vimeo API, and exposes it in C# as `VimeoVideoValue`. There is no host Umbraco site and no test project in this repo — the package is consumed by other solutions.

## Commands

Solution lives in `src/`:

```bash
dotnet build src/Limbo.Umbraco.Vimeo.sln          # Debug builds get a "buildYYYYMMDDHHmm" VersionSuffix
dotnet build src/Limbo.Umbraco.Vimeo --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget
```

`release.bat` / `debug.bat` wrap the pack commands. Built `.nupkg` files for released versions are committed under `releases/nuget/`.

There is **no frontend build step**: the backoffice assets under `wwwroot/` are hand-written ES modules shipped verbatim as static web assets. Do not introduce npm/Vite/TypeScript — the sibling package `Limbo.Umbraco.Video` (`v17/main`) sets the convention this repo follows.

## Branching / versioning

Branch per Umbraco major: `v17/dev` (current, Umbraco 17 / .NET 10), `v13/main` (Umbraco 13), plus EOL `v1/main` and `v2/main`. The package version in the csproj (`VersionPrefix`) tracks the Umbraco major. When bumping the target Umbraco version, expect to touch: `TargetFramework`, the `Umbraco.Cms.*` version ranges, the `Limbo.Umbraco.Video` dependency, `VersionPrefix`, README's version table, and `VimeoPackage.DocumentationUrl`.

`documentation/UMBRACO-17-UPGRADE.md` records what the v13 → v17 rewrite changed and why; read it before touching the backoffice layer.

## Architecture

Data flow when an editor pastes a source string:

1. `wwwroot/Elements/Video.js` (`limbo-vimeo-video`, a Lit element) calls the Management API via `umbHttpClient` + `tryExecute`.
2. `GET /umbraco/management/api/v1/vimeo/video?source=…` → `Controllers/VimeoController.cs` (`ManagementApiControllerBase`, `[VersionedApiBackOfficeRoute("vimeo")]`).
3. The controller uses `VimeoService.TryGetVideoId` to parse the source (iframe `src` extraction, then `VimeoConstants.RegexUrlPattern`) into `VimeoVideoOptions` (video id + unlisted hash + raw query string), picks the first configured credentials, and calls the Vimeo API via `Skybrud.Social.Vimeo`'s `VimeoHttpService.Videos.SearchVideos`.
4. The response is `ApiVideoValue` — **serialized with Newtonsoft and returned as `Content(json, "application/json")`**, because the payload embeds a Newtonsoft `JObject` that System.Text.Json (the Management API default) cannot serialize.
5. `Video.js` stores the raw API video JSON **double-serialized** as `value.video = { _data: JSON.stringify(...) }` — deliberate, because Umbraco/JSON.NET would otherwise mangle timestamps in the payload. `VimeoVideoDetails` reads it back out of the `_data` string. Don't "simplify" this without handling the timestamp corruption; it is also the reason v13-era content still loads.
6. On read, `VimeoVideoValueConverter` → `VimeoVideoValue.Parse(json, config)` builds `Details` (`VimeoVideoDetails`, wrapping the `Skybrud.Social.Vimeo` `VimeoVideo` in `Data`) and `Embed` (`VimeoVideoEmbed`).

Embed option precedence lives in `Models/Videos/VimeoVideoEmbed.cs`: data-type configuration (`VimeoVideoConfiguration`) wins over parameters parsed from the pasted embed code. For `Color`, a 6-char hex on the data type wins, `"disabled"` suppresses the parameter entirely, and anything else (incl. unset / `"inherit"`) falls through to the embed-code parameter. `VimeoVideoEmbed` then feeds `VimeoEmbedOptions`/`VimeoEmbedPlayerOptions`, which build the player URL query string and the `<iframe>` HTML.

### Backoffice registration

`Composers/VimeoComposer.cs` registers `VimeoService`, binds `VimeoSettings`, and registers `VimeoPackageManifestReader` as an `IPackageManifestReader` singleton. That reader is the single source of truth for everything the backoffice loads — **any new file under `wwwroot/` must be declared there** or it won't load:

- `localization` extensions → `wwwroot/Localization/*.js` (keys are referenced as `limboVimeo_*`)
- `icons` extension → `wwwroot/Icons/icons.js`, whose entries lazily import a module default-exporting the SVG string
- `propertyEditorUi` extensions → `wwwroot/Elements/*.js`

The main `propertyEditorUi` binds to the C# schema through `meta.propertyEditorSchemaAlias` == `VimeoVideoPropertyEditor.EditorAlias`; that alias is the only link between client and server. Its `meta.settings.properties` declare the data type configuration fields — labels, descriptions and editors that in v13 lived in `[ConfigurationField]` attributes. `ButtonList.js` and `Color.js` are `propertyEditorUi` extensions **without** a schema alias, so they can only be used to configure other property editors.

`StaticWebAssetBasePath` maps `wwwroot` to `App_Plugins/Limbo.Umbraco.Vimeo`, which is why manifest URLs use that path. Every JS URL gets `?v=<md5 of InformationalVersion>` for cache busting.

Configuration is bound from `Limbo:Vimeo` (`Models/Settings/VimeoSettings.cs` → `List<VimeoCredentials>`); only the first credentials entry is currently used. See README for the appsettings shape.

Interfaces `IVideoValue` / `IVideoDetails` / `IVideoEmbed` / `IVideoProvider` / `ICredentials` come from the sibling `Limbo.Umbraco.Video` package — Vimeo, YouTube etc. implement a shared contract, so changes to these models should stay compatible with that abstraction.

## Conventions

- `src/.editorconfig` is authoritative (4 spaces, CRLF, no final newline, `dotnet_sort_system_directives_first`). Follow existing style: file-scoped namespaces, `#region` blocks (`Properties` / `Constructors` / `Member methods` / `Static methods`), K&R braces, collection expressions.
- Nullable is enabled; XML docs are generated, so public members need `///` docs. Files that intentionally skip them use `#pragma warning disable 1591` / `CS1591` at the top.
- Property-value serialization is **Newtonsoft** throughout (`Skybrud.Essentials.Json.Newtonsoft` helpers like `json.GetString(...)`, `GetObject(...)`, `GetBooleanOrNull(...)`). Data type *configuration*, by contrast, is deserialized by Umbraco with System.Text.Json using the `[ConfigurationField]` key as the property name — so Newtonsoft attributes have no effect there.
- User-facing error strings are localized client-side: add a code to `Constants/VimeoErrorCodes.cs`, map it in `wwwroot/Elements/Video.js`, and add the `error*` key to **both** `wwwroot/Localization/en-US.js` and `da-DK.js`.
