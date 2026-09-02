# Umbraco 17 upgrade

Recap of the upgrade of **Limbo.Umbraco.Vimeo** from Umbraco 13 (`v13/main`) to Umbraco 17 (`v17/dev`).

Umbraco 14 replaced the AngularJS backoffice with one built on Web Components (Lit), and removed the old
`/umbraco/backoffice` controller stack. Since this package consists of a property editor, a backoffice API and a set
of backoffice assets, effectively every layer of it had to be rewritten — this was not a version-bump upgrade.

The structure follows [**Limbo.Umbraco.Video `v17/main`**](https://github.com/limbo-works/Limbo.Umbraco.Video/tree/v17/main),
which is the base package this one depends on: plain ESM JavaScript served as static web assets, no Node/npm/Vite
build step, and the package manifest generated in C#.

---

## 1. Project and dependencies

| | Before (v13) | After (v17) |
|---|---|---|
| Target framework | `net8.0` | `net10.0` |
| Package version | `13.0.1` | `17.0.0-alpha000` |
| `Umbraco.Cms.Core` | `[13.0.0,13.999)` | `[17.0.0,17.9.9)` |
| `Umbraco.Cms.Web.Website` | `[13.0.0,13.999)` | `[17.0.0,17.9.9)` |
| `Umbraco.Cms.Web.BackOffice` | `[13.0.0,13.999)` | **removed** |
| `Umbraco.Cms.Api.Management` | – | `[17.0.0,17.9.9)` |
| `Limbo.Umbraco.Video` | `13.0.0` | `17.0.0-alpha001` |
| `Skybrud.Social.Vimeo` | `1.1.2` | `1.1.2` (unchanged) |

`Umbraco.Cms.Web.BackOffice` was discontinued after Umbraco 13 — its last release is `13.16.0`. Backoffice API
controllers now live in `Umbraco.Cms.Api.Management`.

`NuGetAuditSuppress` entries were added for known vulnerabilities in transitive Umbraco dependencies (`MessagePack`,
`MimeKit`, `MailKit`, `System.Security.Cryptography.Xml`, `Microsoft.OpenApi`), so the package builds warning-free.

`debug.bat` now packs to `c:\nuget\Umbraco17` instead of `c:\nuget\Umbraco13`.

## 2. Backoffice assets: AngularJS → Lit

All AngularJS controllers, HTML views, LESS/CSS and the Web Compiler setup were deleted and replaced by ES modules
that are loaded directly by the backoffice.

**Removed**

- `wwwroot/Views/*.html` (Video, ButtonList, Color)
- `wwwroot/Scripts/Controllers/*.js` and `wwwroot/Scripts/Services/VimeoService.js`
- `wwwroot/Styles/*` (`Default.less`, `_buttonList.less`, and the compiled `.css`/`.min.css`)
- `compilerconfig.json` + `compilerconfig.json.defaults` — there is no LESS compilation step anymore, styling is
  scoped inside each element via Lit's `static styles` using UUI custom properties
- `wwwroot/Lang/en-US.xml` and `da-DK.xml`
- `wwwroot/BackOffice/Icons/*.svg`

**Added**

| File | Replaces | Custom element |
|---|---|---|
| `wwwroot/Elements/Video.js` | `Views/Video.html` + `Controllers/Video.js` + `Services/VimeoService.js` | `limbo-vimeo-video` |
| `wwwroot/Elements/ButtonList.js` | `Views/ButtonList.html` + `Controllers/ButtonList.js` | `limbo-vimeo-button-list` |
| `wwwroot/Elements/Color.js` | `Views/Color.html` + `Controllers/Color.js` | `limbo-vimeo-color` |
| `wwwroot/Localization/en-US.js`, `da-DK.js` | the XML language files | – |
| `wwwroot/Icons/icons.js` + `icon-limbo-vimeo.js` | the raw SVG files | – |

Notable API differences handled along the way:

- The property value is now communicated by setting `this.value` and dispatching `UmbChangeEvent` — there is no
  `$scope.model.value` two-way binding. The v13 workaround of assigning `""` instead of `null` to reset a value is no
  longer needed; the value is simply set to `undefined`.
- The old `limbo-video-duration` AngularJS directive is now a web component from `Limbo.Umbraco.Video`, imported via
  the `@limbo/video/elements/duration` import map entry that package registers.
- UUI components (`uui-input`, `uui-textarea`, `uui-button`, `uui-box`, `uui-loader-bar`, `uui-icon`) replace the raw
  HTML elements and the `umb-load-indicator` directive.
- Icons are no longer auto-discovered from `App_Plugins/*/BackOffice/Icons`. They must be registered as an `icons`
  extension pointing at a JS module that default-exports `[{ name, path: () => import(...) }]`, where the imported
  module default-exports the SVG markup as a string.
- The `color-limbo` suffix was dropped from the editor icon (`icon-limbo-vimeo-alt color-limbo` →
  `icon-limbo-vimeo-alt`), as icon colour is no longer part of the icon string.

## 3. Package manifest: `IManifestFilter` → `IPackageManifestReader`

`Manifests/VimeoManifestFilter.cs` was replaced by `Manifests/VimeoPackageManifestReader.cs`, registered in
`VimeoComposer` as `builder.Services.AddSingleton<IPackageManifestReader, VimeoPackageManifestReader>()` instead of
`builder.ManifestFilters().Append<VimeoManifestFilter>()`.

The manifest no longer lists scripts and stylesheets to be bundled. It now declares typed extensions:

- two `localization` extensions (`en`, `da`)
- one `icons` extension
- one `propertyEditorUi` extension for the video picker itself, bound to the C# schema through
  `meta.propertyEditorSchemaAlias`
- two `propertyEditorUi` extensions (button list and colour) used **only** to render data type configuration fields;
  they deliberately omit `propertyEditorSchemaAlias`

Cache busting changed from `?v={InformationalVersion}` appended to the view path to an MD5 hash of the informational
version appended to every JS URL, matching `Limbo.Umbraco.Video`.

## 4. Property editor schema (C#)

`[DataEditor]` in Umbraco 17 only accepts an alias plus `ValueType`, `IsDeprecated` and `ValueEditorIsReusable`. The
name, view, group and icon arguments are gone — that metadata now lives in the `propertyEditorUi` manifest entry:

```csharp
// Before
[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]

// After
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
```

Consequently:

- `EditorView` was removed and a new `EditorUiAlias` constant (`Limbo.Umbraco.Vimeo.Video`) was introduced.
- The `GetValueEditor` override that appended `?v=<version>` to `DataValueEditor.View` was removed — `View` no longer
  exists on the value editor.
- `IEditorConfigurationParser` was removed from Umbraco, so `VimeoVideoConfigurationEditor` no longer takes it, and
  the constructor that rewrote `{alias}`/`{version}` tokens in the configuration field views is gone.
- `VimeoVideoValueConverter` reads `propertyType.DataType.ConfigurationObject` instead of
  `propertyType.DataType.Configuration`.

### Configuration fields

`[ConfigurationField]` no longer takes a label, view or description; only the key remains. Labels, descriptions,
editors and default values are declared in `settings.properties` / `settings.defaultData` in the manifest.

Umbraco deserializes the configuration with `System.Text.Json` (previously Newtonsoft). This was verified against
`SystemTextConfigurationEditorJsonSerializer`, which registers `JsonStringEnumConverter` and a tolerant boolean
converter, and resolves property names from the `ConfigurationField` key — so existing data type configurations
(`"autoplay": "inherit"`, `"showTitle": true`, `"color": "00adef"`) keep deserializing without a migration.

**One configuration field was dropped: `hideLabel`.** Umbraco 14 moved label visibility from the data type to the
individual property on the Document Type ("Appearance" → hide label), so a data-type-level setting is no longer
honoured. Existing values for the key are simply ignored.

## 5. Backoffice API: `UmbracoAuthorizedApiController` → Management API

`UmbracoAuthorizedApiController` and `[PluginController]` were removed in Umbraco 14. `VimeoController` now derives
from `ManagementApiControllerBase`:

```csharp
[ApiExplorerSettings(GroupName = "Limbo Vimeo")]
[VersionedApiBackOfficeRoute("vimeo")]
[MapToApi("management")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public class VimeoController : ManagementApiControllerBase
```

| | Before | After |
|---|---|---|
| Endpoint | `POST/GET /umbraco/backoffice/Limbo/Vimeo/GetVideo` | `GET /umbraco/management/api/v1/vimeo/video` |
| Parameter | `source` from query **or** form body | `source` from query |
| Client call | `$http` via the `vimeoService` Angular factory | `umbHttpClient` + `tryExecute` |

The endpoint is registered in Umbraco's existing `management` OpenAPI document via `[MapToApi("management")]`, so no
separate Swagger document, `ISchemaIdHandler` or `UmbracoPipelineFilter` registration is needed, and no generated
OpenAPI client is required on the frontend — `umbHttpClient` is the backoffice's own pre-authenticated client.

Two subtleties handled here:

- **Serialization.** The response embeds the raw JSON from the Vimeo API as a Newtonsoft `JObject`. The Management
  API serializes with `System.Text.Json`, which cannot serialize a `JObject` correctly, so the response is serialized
  with Newtonsoft explicitly and returned via `Content(json, "application/json")`.
- **Error messages.** The backoffice no longer reads the server-side XML language files, so returning a
  `ILocalizedTextService`-localized string from the API is pointless. `Constants/VimeoTranslations.cs` was replaced by
  `Constants/VimeoErrorCodes.cs`; the API returns a stable code and `Video.js` maps it to a `limboVimeo_error*` key
  that is localized in the browser. The code has to travel as **problem details** — Umbraco's error interceptor
  discards any non-ok response body that lacks `type`/`title`/`status`, so the code is carried in the
  `operationStatus` extension. For the same reason "video not found" is returned as a `400` rather than a `404`: the
  interceptor handles 404 before it ever reads the body.

`Skybrud.Social.Vimeo` 1.1.2 exposes no async overload of `SearchVideos`, so the endpoint remains synchronous.

## 6. Models

`Limbo.Umbraco.Video` 17 changed `IVideoDetails` to expose `IReadOnlyList<>` instead of `IEnumerable<>`, so
`VimeoVideoDetails.Thumbnails` and `.Files` (and their explicit interface implementations) were changed accordingly.

Everything else in `Models/`, `Options/`, `Services/` and `Json/` is unchanged, including the Newtonsoft-based
parsing of the stored property value.

## 7. Stored data — no migration needed

The shape of the stored property value is deliberately unchanged:

```json
{
  "source": "https://vimeo.com/123456789",
  "credentials": { "key": "..." },
  "parameters": { },
  "video": { "_data": "<the raw Vimeo API JSON, serialized as a string>" }
}
```

The `video._data` double-serialization is still in place. It exists because Umbraco/JSON.NET mangles the timestamps
inside the payload returned by the Vimeo API, and it is still relied upon by `VimeoVideoDetails`. Content saved by
`v13.x` therefore keeps working after upgrading, and `VimeoValue.Details` / `.Embed` behave as before.

## 8. Review findings addressed

The rewritten extension was reviewed against the Umbraco 17.5 backoffice bundle. Fixes applied as a result:

- **Error codes never reached the UI.** The API originally returned `{ "error": "…" }`, which Umbraco's error
  interceptor throws away — every failure would have shown the generic "request failed" message. Now returned as
  problem details with `operationStatus`, and `videoNotFound` returns `400` instead of `404` (see section 5).
- **Duplicate error notification.** `tryExecute` raises a generic notification for most failures, on top of the
  inline message rendered by the element. Now called with `{ disableNotifications: true }`.
- **Unsaved edits.** `UmbChangeEvent` was only dispatched once the Vimeo lookup resolved, so saving immediately after
  typing could persist a stale value. The event is now also dispatched on input.
- **Input/textarea swapping mid-typing.** Whether to render `uui-input` or `uui-textarea` was derived from the live
  value, so typing `<` swapped the element and dropped focus. It is now decided on load and on commit only.
- **Enum binding made explicit.** `VimeoAutoplay`/`VimeoLoop` only carried Newtonsoft converters, which
  `System.Text.Json` ignores; they happened to bind because Umbraco registers `JsonStringEnumConverter` globally.
  A `System.Text.Json` `JsonStringEnumConverter` attribute was added so this no longer depends on Umbraco's setup.
- **Duplicate icon.** `icon-limbo-vimeo` and `icon-limbo-vimeo-alt` use the same artwork (as they did in v13);
  the `-alt` entry is kept for existing references but marked `hidden` so the icon picker doesn't list it twice.

Deliberately not changed: the endpoint stays synchronous (no async API available, see section 5), `credentials` is
still written into the property value even though C# doesn't read it back (it is part of the v13 storage format), and
the property editor UI does not declare `supportsReadOnly` — Umbraco falls back to blocking the editor with its own
overlay, which is safe but not as polished as a native read-only state.

## 9. Verification

- `dotnet build src/Limbo.Umbraco.Vimeo.sln` — succeeds with 0 errors and 0 warnings.
- `dotnet build src/Limbo.Umbraco.Vimeo --configuration Release /t:rebuild /t:pack` — produces
  `Limbo.Umbraco.Vimeo.17.0.0-alpha000.nupkg` containing `lib/net10.0/` and the seven static web assets under
  `staticwebassets/`, with the dependency ranges listed above.

**Not verified:** the package has not been run inside an actual Umbraco 17 site — this repository contains no host
site. The backoffice behaviour (rendering, saving, the API round-trip and the data type configuration UI) should be
smoke-tested in a real Umbraco 17 instance before the alpha is published.
