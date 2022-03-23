using Limbo.Umbraco.Vimeo.Models.Credentials;
using Umbraco.Cms.Core.Configuration.Models;

namespace Limbo.Umbraco.Vimeo.Models.Settings {
    
    /// <summary>
    /// Class representing the settings for this package.
    /// </summary>
    [UmbracoOptions("Limbo:Vimeo", BindNonPublicProperties = true)]
    public class VimeoSettings {

        /// <summary>
        /// Gets a collection of the credentials configured for YouTube.
        /// </summary>
        public VimeoCredentials[] Credentials { get; internal set; }

    }

}