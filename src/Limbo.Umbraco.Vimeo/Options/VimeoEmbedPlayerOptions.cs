using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;

namespace Limbo.Umbraco.Vimeo.Options {
    
    /// <summary>
    /// Class representing the options for the Vimeo video player.
    /// </summary>
    public class VimeoEmbedPlayerOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the color of the Vimeo branding.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        /// <summary>
        /// Indicates whether the video should automatically start to play when the player loads.
        /// </summary>
        [JsonProperty("autoplay", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Autoplay { get; set; }

        /// <summary>
        /// Gets whether the video should play again and again.
        /// </summary>
        [JsonProperty("loop", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Loop { get; set; }

        /// <summary>
        /// Gets or sets whether the video title should be shown in the player.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets whether the author by line should be shown in the player.
        /// </summary>
        [JsonProperty("byline", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowByLine { get; set; }

        /// <summary>
        /// Gets or sets whether the author portrait should be shown in the player.
        /// </summary>
        [JsonProperty("portrait", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowPortrait { get; set; }

        #endregion

        #region Member methods

        /// <summary>
        /// Appends the player options to the specified <paramref name="query"/> string.
        /// </summary>
        /// <param name="query">The query string.</param>
        public void AppendToQueryString(IHttpQueryString query) {

            string color = Color?.TrimStart('#');

            if (Autoplay != null) query.Add("autoplay", Autoplay.Value ? 1 : 0);
            if (Loop != null) query.Add("loop", Loop.Value ? 1 : 0);
            if (string.IsNullOrWhiteSpace(color) == false) query.Add("color", color);
            if (ShowTitle != null) query.Add("title", ShowTitle.Value ? 1 : 0);
            if (ShowByLine != null) query.Add("byline", ShowByLine.Value ? 1 : 0);
            if (ShowPortrait != null) query.Add("portrait", ShowPortrait.Value ? 1 : 0);

        }

        #endregion

    }

}