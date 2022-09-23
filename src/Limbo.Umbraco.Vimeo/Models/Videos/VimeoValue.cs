using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.Vimeo.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.Vimeo.Models.Videos {
    
    /// <summary>
    /// Class representing the value of the <see cref="VimeoEditor"/> property editor.
    /// </summary>
    public class VimeoValue : IVideoValue {

        #region Properties

        /// <summary>
        /// Gets the source (URL or embed code) as entered by the user.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; }

        /// <summary>
        /// Gets the details about the picked video.
        /// </summary>
        [JsonProperty("details")]
        public VimeoVideoDetails Details { get; }

        /// <summary>
        /// Gets embed information for the video.
        /// </summary>
        [JsonProperty("embed")]
        public VimeoEmbed Embed { get; }
        
        IVideoDetails IVideoValue.Details => Details;
        
        IVideoEmbed IVideoValue.Embed => Embed;

        #endregion

        #region Constructors

        private VimeoValue(JObject json) {
            Source = json.GetString("source");
            Details = json.GetObject("video", VimeoVideoDetails.Parse);
            Embed = new VimeoEmbed(Details);
        }

        #endregion

        #region Static methods

        internal static VimeoValue Parse(JObject json) {
            return json == null ? null : new VimeoValue(json);
        }

        #endregion

    }

}