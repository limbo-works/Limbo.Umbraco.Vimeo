using Limbo.Umbraco.Video.Models;
using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.Vimeo.Models.Videos;
using Limbo.Umbraco.Vimeo.Options;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters;

namespace Limbo.Umbraco.Vimeo.Models {
    
    /// <summary>
    /// Class representing the embed options of the video.
    /// </summary>
    public class VimeoEmbed : IVideoEmbed {

        #region Properties

        /// <summary>
        /// Gets the HTML embed code.
        /// </summary>
        [JsonProperty("html")]
        [JsonConverter(typeof(StringJsonConverter))]
        public HtmlString Html { get; }

        #endregion

        #region Constructors

        internal VimeoEmbed(VimeoVideoDetails video) {
            Html = new VimeoEmbedOptions(video.Data).GetEmbedCode();
        }

        #endregion

    }

}