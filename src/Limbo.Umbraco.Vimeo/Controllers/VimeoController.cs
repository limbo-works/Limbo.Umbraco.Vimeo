using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Limbo.Umbraco.Vimeo.Constants;
using Limbo.Umbraco.Vimeo.Models.Api;
using Limbo.Umbraco.Vimeo.Models.Credentials;
using Limbo.Umbraco.Vimeo.Options;
using Limbo.Umbraco.Vimeo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Vimeo;
using Skybrud.Social.Vimeo.Models.Videos;
using Skybrud.Social.Vimeo.Options.Videos;
using Skybrud.Social.Vimeo.Responses.Videos;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.Controllers {

    [PluginController("Limbo")]
    public class VimeoController : UmbracoAuthorizedApiController {

        private readonly ILogger<VimeoController> _logger;
        private readonly VimeoService _vimeoService;
        private readonly ILocalizedTextService _localizedTextService;

        #region Constructors

        public VimeoController(ILogger<VimeoController> logger, VimeoService vimeoService, ILocalizedTextService localizedTextService) {
            _logger = logger;
            _vimeoService = vimeoService;
            _localizedTextService = localizedTextService;
        }

        #endregion

        #region Public API methods

        [HttpGet]
        [HttpPost]
        public object GetVideo() {

            // Get the "source" parameter from either GET or POST
            string? source = HttpContext.Request.Query["source"];
            if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
                source = HttpContext.Request.Form["source"].FirstOrDefault();
            }

            // Return an error if "source" is missing
            if (string.IsNullOrWhiteSpace(source)) {
                return BadRequest(Localize(VimeoTranslations.Errors.NoSource));
            }

            // Do we recognize the entered source as either a vimeo video URL or embed code?
            if (!_vimeoService.TryGetVideoId(source, out VimeoVideoOptions? options)) {
                return BadRequest(Localize(VimeoTranslations.Errors.InvalidSource));
            }

            VimeoCredentials? credentials = _vimeoService.GetCredentials().FirstOrDefault();
            if (credentials == null || !_vimeoService.TryGetHttpService(credentials, out VimeoHttpService? http)) {
                return BadRequest(Localize(VimeoTranslations.Errors.NoCredentials));
            }

            VimeoVideo video;

            try {

                // Rebuild the video URL
                string url = $"https://vimeo.com/{options.VideoId}{(string.IsNullOrWhiteSpace(options.Hash) ? "" : $"/{options.Hash}")}";

                // Attempt to fetch information baout the video from the Vimeo API
                VimeoVideoListResponse response = http.Videos.SearchVideos(new VimeoSearchVideosOptions {
                    Links = new List<string> { url }
                });

                // Return an error to the user if the video wasn't found
                if (response.Body.Data.Count == 0) return NotFound(Localize(VimeoTranslations.Errors.VideoNotFound));

                // Get the first video of the response
                video = response.Body.Data[0];

            } catch (Exception ex) {

                _logger.LogError(ex, "Failed fetching video from Vimeo API with source: {Source}", source);

                return InternalServerError(Localize(VimeoTranslations.Errors.GetVideoFailed));

            }

            JObject parameters = JObject.FromObject(options);
            parameters.Remove("videoId");

            return new ApiVideoValue(credentials, video, parameters);

        }

        #endregion

        #region Private helper methods

        private string Localize(string alias) {
            return _localizedTextService.Localize("limboVimeo", alias, CultureInfo.CurrentCulture);
        }

        private object BadRequest(string message) {
            return base.BadRequest(new { message });
        }

        private object NotFound(string message) {
            return base.NotFound(new { message });
        }

        private object InternalServerError(string message) {
            return new JsonResult(new { message }) {
                StatusCode = 500
            };
        }

        #endregion

    }

}