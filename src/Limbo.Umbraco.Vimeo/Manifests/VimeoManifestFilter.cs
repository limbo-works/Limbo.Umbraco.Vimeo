using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.Vimeo.Manifests;

/// <inheritdoc />
public class VimeoManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
            AllowPackageTelemetry = true,
            PackageId = VimeoPackage.Alias,
            PackageName = VimeoPackage.Name,
            Version = VimeoPackage.InformationalVersion,
            BundleOptions = BundleOptions.Independent,
            Scripts = [
                "/App_Plugins/Limbo.Umbraco.Vimeo/Scripts/Services/VimeoService.js",
                "/App_Plugins/Limbo.Umbraco.Vimeo/Scripts/Controllers/Color.js",
                "/App_Plugins/Limbo.Umbraco.Vimeo/Scripts/Controllers/ButtonList.js",
                "/App_Plugins/Limbo.Umbraco.Vimeo/Scripts/Controllers/Video.js"
            ],
            Stylesheets = [
                "/App_Plugins/Limbo.Umbraco.Vimeo/Styles/Default.css"
            ]
        };

        // Append the manifest
        manifests.Add(manifest);

    }

}