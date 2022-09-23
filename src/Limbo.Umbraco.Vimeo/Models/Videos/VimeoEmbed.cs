using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.Vimeo.Options;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters;

namespace Limbo.Umbraco.Vimeo.Models.Videos {
    
    /// <summary>
    /// Class representing the embed options of the video.
    /// </summary>
    public class VimeoEmbed : IVideoEmbed {

        #region Properties

        /// <summary>
        /// Gets the embed URL.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the HTML embed code.
        /// </summary>
        [JsonProperty("html")]
        [JsonConverter(typeof(StringJsonConverter))]
        public IHtmlContent Html { get; }

        #endregion

        #region Constructors

        internal VimeoEmbed(VimeoVideoDetails video) {
            VimeoEmbedOptions o = new(video.Data);
            Url = o.GetEmbedUrl();
            Html = o.GetEmbedCode();
        }

        #endregion

    }

}