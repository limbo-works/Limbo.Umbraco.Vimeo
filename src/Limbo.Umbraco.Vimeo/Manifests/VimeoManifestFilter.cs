using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.Vimeo.Manifests {

    /// <inheritdoc />
    public class VimeoManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {
            manifests.Add(new PackageManifest {
                PackageName = "limbo-umbraco-vimeo",
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    "/App_Plugins/Limbo.Umbraco.Vimeo/Scripts/Services/VimeoService.js",
                    "/App_Plugins/Limbo.Umbraco.Vimeo/Scripts/Controllers/Video.js"
                },
                Stylesheets = new[] {
                    "/App_Plugins/Limbo.Umbraco.Vimeo/Styles/Default.css"
                }
            });
        }

    }

}