using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Limbo.Umbraco.Vimeo.Models.Credentials;
using Limbo.Umbraco.Vimeo.Models.Settings;
using Limbo.Umbraco.Vimeo.Options;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Essentials.Strings;
using Skybrud.Social.Vimeo;

namespace Limbo.Umbraco.Vimeo.Services {

    /// <summary>
    /// Service for working with the YouTube integration.
    /// </summary>
    public class VimeoService {

        private readonly IOptions<VimeoSettings> _settings;

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        public VimeoService(IOptions<VimeoSettings> settings) {
            _settings = settings;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Returns whether the specified <paramref name="source"/> is recognized as a Vimeo video URL or embed code.
        /// </summary>
        /// <param name="source">The source </param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool TryGetVideoId(string source, out VimeoVideoOptions options) {

            options = null;
            if (string.IsNullOrWhiteSpace(source)) return false;

            // Is "source" an iframe?
            if (source.Contains("<iframe")) {

                // Match the "src" attribute
                Match m0 = Regex.Match(source, "src=\"(.+?)\"", RegexOptions.IgnoreCase);
                if (m0.Success == false) return false;

                // Update the source with the value from the "src" attribute
                source = m0.Groups[1].Value;

            }

            // Split the source into URL and query string
            (string url, string query) = source.Split('?');

            // Does "source" match known formats of Vimeo video URLs?
            long videoId;
            string hash = null;
            if (RegexUtils.IsMatch(url, "vimeo.com/(video/|)([0-9]+)$", out Match m1)) {
                videoId = long.Parse(m1.Groups[2].Value);
            } else if (RegexUtils.IsMatch(url, "vimeo.com/(video/|)([0-9]+)/([a-z0-9]+)$", out m1)) {
                videoId = long.Parse(m1.Groups[2].Value);
                hash = m1.Groups[3].Value;
            } else if (RegexUtils.IsMatch(url, "vimeo.com/manage/videos/([0-9]+)$", out m1)) {
                videoId = long.Parse(m1.Groups[1].Value);
            } else {
                return false;
            }

            options = new VimeoVideoOptions(videoId, hash, query);

            return true;

        }

        /// <summary>
        /// Returns a list of Vimeo credentials.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VimeoCredentials> GetCredentials() {
            return _settings.Value.Credentials ?? Array.Empty<VimeoCredentials>();
        }

        /// <summary>
        /// Creates a new HTTP service for accessing the Vimeo API using the specified <paramref name="credentials"/>.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="http">When this method returns, holds the created HTTP service if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        public virtual bool TryGetHttpService(VimeoCredentials credentials, out VimeoHttpService http) {

            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            if (!string.IsNullOrWhiteSpace(credentials.AccessToken)) {
                http = VimeoHttpService.CreateFromAccessToken(credentials.AccessToken);
                return true;
            }

            http = null;
            return false;

        }

        #endregion

    }

}