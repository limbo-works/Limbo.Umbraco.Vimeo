using System.Web;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Vimeo.Models.Videos;

namespace Limbo.Umbraco.Vimeo.Options {
    
    /// <summary>
    /// Class representing the embed options for a Vimeo video
    /// </summary>
    public class VimeoEmbedOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the video.
        /// </summary>
        [JsonProperty("videoId")]
        public long VideoId { get; set; }

        /// <summary>
        /// Gets or sets the embed URL of the video.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title of the embed iframe.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the options for the player.
        /// </summary>
        [JsonProperty("player", NullValueHandling = NullValueHandling.Ignore)]
        public VimeoEmbedPlayerOptions Player { get; set; }

        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        public VimeoEmbedOptions(long videoId) {
            VideoId = videoId;
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="video"/>.
        /// </summary>
        /// <param name="video">The Vimeo video.</param>
        public VimeoEmbedOptions(VimeoVideo video) {
            VideoId = video.Id;
            Url = video.JObject.GetString("player_embed_url");
            Title = video.Name;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Returns the embed URL for the video matching <see cref="Url"/> or <see cref="VideoId"/>.
        /// </summary>
        /// <returns>The embed URL.</returns>
        public virtual string GetEmbedUrl() {
            
            IHttpQueryString query = new HttpQueryString();

            if (string.IsNullOrWhiteSpace(Url)) {
                if (VideoId == 0) throw new PropertyNotSetException(nameof(VideoId));
                Player?.AppendToQueryString(query);
                return $"https://player.vimeo.com/video/{VideoId}?{query}".TrimEnd('?');
            }
            
            string[] pieces = Url.Split('?');
            if (pieces.Length > 1) query = HttpQueryString.Parse(pieces[1]);
            Player?.AppendToQueryString(query);
            return $"{pieces[0]}?{query}".TrimEnd('?');

        }

        /// <summary>
        /// Returns the HTML embed code for the video described by this options instance.
        /// </summary>
        /// <returns>An instance of <see cref="HtmlString"/> representing the HTML embed code.</returns>
        public virtual IHtmlContent GetEmbedCode() {
            return GetEmbedCode(null, null);
        }

        /// <summary>
        /// Returns the HTML embed code for the video described by this options instance.
        /// </summary>
        /// <param name="width">The width of the video.</param>
        /// <param name="height">The height of the video.</param>
        /// <returns>An instance of <see cref="HtmlString"/> representing the HTML embed code.</returns>
        public virtual HtmlString GetEmbedCode(int? width, int? height) {

            width ??= 640;
            height ??= 360;

            string url = GetEmbedUrl();

            string title = string.IsNullOrWhiteSpace(Title) ? "Vimeo video player" : Title;

            string html = $"<iframe src=\"{HttpUtility.HtmlAttributeEncode(url)}\" width=\"{width}\" height=\"{height}\" title=\"{HttpUtility.HtmlAttributeEncode(title)}\" frameborder=\"0\" allow=\"autoplay; fullscreen; picture-in-picture\" allowfullscreen></iframe>";

            return new HtmlString(html);

        }

        #endregion

    }

}