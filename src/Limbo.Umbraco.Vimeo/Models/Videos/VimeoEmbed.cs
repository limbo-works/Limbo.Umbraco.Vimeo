using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.Vimeo.Options;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Converters;
using Skybrud.Essentials.Json.Extensions;

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

        internal VimeoEmbed(VimeoVideoDetails video, JObject parameters) {

            VimeoEmbedOptions o = new(video.Data);

            if (parameters != null) {
                o.Player.Color = parameters.GetString("color");
                o.Player.Autoplay = parameters.GetString("autoplay", ParseBoolean);
                o.Player.Loop = parameters.GetString("loop", ParseBoolean);
                o.Player.ShowTitle = parameters.GetString("title", ParseBoolean);
                o.Player.ShowByLine = parameters.GetString("byline", ParseBoolean);
                o.Player.ShowPortrait = parameters.GetString("portrait", ParseBoolean);
            }

            Url = o.GetEmbedUrl();
            Html = o.GetEmbedCode();

        }

        #endregion

        #region Static methods

        private static bool? ParseBoolean(string value) {
            return value switch {
                "True" => true,
                "False" => false,
                _ => null
            };
        }

        #endregion

    }

}