using Limbo.Umbraco.Vimeo.Models.Settings;
using Limbo.Umbraco.Vimeo.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.Composers {

    public class VimeoComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddTransient<VimeoService>();

            builder.AddUmbracoOptions<VimeoSettings>();

        }

    }

}