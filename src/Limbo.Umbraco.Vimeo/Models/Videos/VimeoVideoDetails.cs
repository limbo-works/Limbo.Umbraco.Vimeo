using System;
using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Skybrud.Social.Vimeo.Models.Videos;

namespace Limbo.Umbraco.Vimeo.Models.Videos {

    /// <summary>
    /// Class with details about a Vimeo video.
    /// </summary>
    public class VimeoVideoDetails : JsonObjectBase, IVideoDetails {

        #region Properties

        /// <summary>
        /// Gets a reference to the <see cref="VimeoVideo"/> as received from the Vimeo API.
        /// </summary>
        [JsonIgnore]
        public VimeoVideo Data { get; }

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; }

        /// <summary>
        /// Gets the Vimeo URL of the video.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the title of the video.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; }

        /// <summary>
        /// Gets the description of the video.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; }

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        [JsonProperty("duration")]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets a list of thumbnails of the video.
        /// </summary>
        [JsonProperty("thumbnails", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<VimeoThumbnail> Thumbnails { get; }

        /// <summary>
        /// Gets a list of video files of the video.
        /// </summary>
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<VideoFile> Files { get; }

        IEnumerable<IVideoThumbnail> IVideoDetails.Thumbnails => Thumbnails;

        IEnumerable<IVideoFile> IVideoDetails.Files => Files;

        TimeSpan? IVideoDetails.Duration => Duration;

        #endregion

        #region Constructors

        private VimeoVideoDetails(JObject json) : base(json) {

            Data = json.GetString("_data", x => JsonUtils.ParseJsonObject(x, VimeoVideo.Parse))!;

            Id = Data.Id;
            Url = Data.Link;
            Title = Data.Name;
            Description = Data.Description;
            Duration = Data.Duration;
            Thumbnails = Data.Pictures.Sizes.Select(x => new VimeoThumbnail(x)).ToList();
            Files = Data.JObject!.Property("files") is null ? null : Data.Files.Select(x => new VimeoFile(x)).ToList();

        }

        #endregion

        #region Static methods

        /// <summary>
        /// Returns a new <see cref="VimeoVideoDetails"/> parsed from the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The instance of <see cref="JObject"/> to parse.</param>
        /// <returns>An instance of <see cref="VimeoVideoDetails"/>.</returns>
        public static VimeoVideoDetails Parse(JObject json) {
            return json == null ? null : new VimeoVideoDetails(json);
        }

        #endregion

    }

}