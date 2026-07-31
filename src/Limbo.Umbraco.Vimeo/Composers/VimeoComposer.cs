// [CHANGE: Umbraco 17 upgrade - manifest filters replaced by IPackageManifestReader]
// Related: Manifests/VimeoPackageManifestReader.cs, Controllers/VimeoController.cs

using Limbo.Umbraco.Vimeo.Manifests;
using Limbo.Umbraco.Vimeo.Models.Settings;
using Limbo.Umbraco.Vimeo.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.Composers;

public class VimeoComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddSingleton<VimeoService>();

        builder.AddUmbracoOptions<VimeoSettings>();

        builder.Services.AddSingleton<IPackageManifestReader, VimeoPackageManifestReader>();

    }

}
