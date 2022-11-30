using System.Linq;
using Limbo.Umbraco.Vimeo.Models.Api;
using Limbo.Umbraco.Vimeo.Models.Credentials;
using Limbo.Umbraco.Vimeo.Options;
using Limbo.Umbraco.Vimeo.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Vimeo;
using Skybrud.Social.Vimeo.Models.Videos;
using Skybrud.Social.Vimeo.Responses.Videos;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.Controllers {

    [PluginController("Limbo")]
    public class VimeoController : UmbracoAuthorizedApiController {

        private readonly VimeoService _vimeoService;

        #region Constructors

        public VimeoController(VimeoService vimeoService) {
            _vimeoService = vimeoService;
        }

        #endregion

        #region Public API methods

        [HttpGet]
        [HttpPost]
        public object GetVideo() {

            // Get the "source" parameter from either GET or POST
            string source = HttpContext.Request.Query["source"];
            if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
                source = HttpContext.Request.Form["source"].FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(source)) return BadRequest("No source specified.");

            if (!_vimeoService.TryGetVideoId(source, out VimeoVideoOptions options)) return BadRequest("Source doesn't match a valid URL or embed code.");

            VimeoCredentials credentials = _vimeoService.GetCredentials().FirstOrDefault();
            if (credentials == null || !_vimeoService.TryGetHttpService(credentials, out VimeoHttpService http)) return BadRequest("No credentials configured for Vimeo.");

            VimeoVideoResponse response = http.Videos.GetVideo(options.VideoId);

            VimeoVideo video = response.Body;
            if (video == null) return NotFound("Video not found.");

            JObject parameters = JObject.FromObject(options);
            parameters.Remove("videoId");

            return new ApiVideoValue(credentials, video, parameters);

        }

        #endregion

    }

}