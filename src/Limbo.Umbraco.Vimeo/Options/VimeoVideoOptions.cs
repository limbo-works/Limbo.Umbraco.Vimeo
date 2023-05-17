using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Social.Vimeo.Models.Videos;

namespace Limbo.Umbraco.Vimeo.Options {

    /// <summary>
    /// Class with options describing a video - eg. based on a URL or embed code.
    /// </summary>
    public class VimeoVideoOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the video.
        /// </summary>
        [JsonProperty("videoId")]
        public long VideoId { get; set; }

        /// <summary>
        /// Get the unlisted hash of the video, if any.
        /// </summary>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string? Hash { get; set; }

        /// <summary>
        /// Gets or sets the color of the Vimeo branding.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Color { get; set; }

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

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        public VimeoVideoOptions(long videoId) {
            VideoId = videoId;
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <param name="query">The query string from the entered URL, if present.</param>
        public VimeoVideoOptions(long videoId, string? query) {
            VideoId = videoId;
            if (string.IsNullOrWhiteSpace(query)) return;
            UpdateFromQuery(HttpQueryString.Parse(query));
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <param name="hash">The unlisted hash of the video, if any.</param>
        /// <param name="query">The query string from the entered URL, if present.</param>
        public VimeoVideoOptions(long videoId, string? hash, string? query) {
            VideoId = videoId;
            Hash = hash;
            if (string.IsNullOrWhiteSpace(query)) return;
            UpdateFromQuery(HttpQueryString.Parse(query));
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <param name="query">The query string from the entered URL, if present.</param>
        public VimeoVideoOptions(long videoId, IHttpQueryString? query) {
            VideoId = videoId;
            if (query != null) UpdateFromQuery(query);
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <param name="hash">The unlisted hash of the video, if any.</param>
        /// <param name="query">The query string from the entered URL, if present.</param>
        public VimeoVideoOptions(long videoId, string hash, IHttpQueryString? query) {
            VideoId = videoId;
            Hash = hash;
            if (query != null) UpdateFromQuery(query);
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="video"/>.
        /// </summary>
        /// <param name="video">The Vimeo video.</param>
        public VimeoVideoOptions(VimeoVideo video) {
            VideoId = video.Id;
        }

        #endregion

        #region Member methods

        private void UpdateFromQuery(IHttpQueryString query) {
            Color = query["color"];
            if (query.TryGetBoolean("autoplay", out bool autoplay)) Autoplay = autoplay;
            if (query.TryGetBoolean("loop", out bool loop)) Loop = loop;
            if (query.TryGetBoolean("title", out bool title)) ShowTitle = title;
            if (query.TryGetBoolean("byline", out bool byline)) ShowByLine = byline;
            if (query.TryGetBoolean("portrait", out bool portrait)) ShowPortrait = portrait;
        }

        #endregion

    }

}