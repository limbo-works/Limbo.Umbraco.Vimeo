// [CHANGE: Umbraco 17 upgrade - UmbracoAuthorizedApiController and PluginController were removed in Umbraco 14, so
// this is now a Management API controller. The endpoint moved from
// /umbraco/backoffice/Limbo/Vimeo/GetVideo to /umbraco/management/api/v1/vimeo/video]
// Related: wwwroot/Elements/Video.js, Constants/VimeoErrorCodes.cs, Models/Api/ApiVideoValue.cs

using System;
using System.Linq;
using Limbo.Umbraco.Vimeo.Constants;
using Limbo.Umbraco.Vimeo.Models.Api;
using Limbo.Umbraco.Vimeo.Models.Credentials;
using Limbo.Umbraco.Vimeo.Options;
using Limbo.Umbraco.Vimeo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Vimeo;
using Skybrud.Social.Vimeo.Models.Videos;
using Skybrud.Social.Vimeo.Options.Videos;
using Skybrud.Social.Vimeo.Responses.Videos;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Web.Common.Authorization;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.Controllers;

[ApiExplorerSettings(GroupName = "Limbo Vimeo")]
[VersionedApiBackOfficeRoute("vimeo")]
[MapToApi("management")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public class VimeoController : ManagementApiControllerBase {

    private readonly ILogger<VimeoController> _logger;
    private readonly VimeoService _vimeoService;

    #region Constructors

    public VimeoController(ILogger<VimeoController> logger, VimeoService vimeoService) {
        _logger = logger;
        _vimeoService = vimeoService;
    }

    #endregion

    #region Public API methods

    /// <summary>
    /// Returns information about the Vimeo video matching the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The URL or embed code of the video.</param>
    [HttpGet("video")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult GetVideo([FromQuery] string? source) {

        // Return an error if "source" is missing
        if (string.IsNullOrWhiteSpace(source)) {
            return Error(StatusCodes.Status400BadRequest, VimeoErrorCodes.NoSource);
        }

        // Do we recognize the entered source as either a Vimeo video URL or embed code?
        if (!_vimeoService.TryGetVideoId(source, out VimeoVideoOptions? options)) {
            return Error(StatusCodes.Status400BadRequest, VimeoErrorCodes.InvalidSource);
        }

        VimeoCredentials? credentials = _vimeoService.GetCredentials().FirstOrDefault();
        if (credentials == null || !_vimeoService.TryGetHttpService(credentials, out VimeoHttpService? http)) {
            return Error(StatusCodes.Status400BadRequest, VimeoErrorCodes.NoCredentials);
        }

        VimeoVideo video;

        try {

            // Rebuild the video URL
            string url = $"https://vimeo.com/{options.VideoId}{(string.IsNullOrWhiteSpace(options.Hash) ? "" : $"/{options.Hash}")}";

            // Attempt to fetch information about the video from the Vimeo API
            VimeoVideoListResponse response = http.Videos.SearchVideos(new VimeoSearchVideosOptions {
                Links = [url]
            });

            // Return an error to the user if the video wasn't found
            // Returned as a 400 rather than a 404, as the backoffice discards the body of 404 responses
            if (response.Body.Data.Count == 0) return Error(StatusCodes.Status400BadRequest, VimeoErrorCodes.VideoNotFound);

            // Get the first video of the response
            video = response.Body.Data[0];

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed fetching video from Vimeo API with source: {Source}", source);

            return Error(StatusCodes.Status500InternalServerError, VimeoErrorCodes.GetVideoFailed);

        }

        JObject parameters = JObject.FromObject(options);
        parameters.Remove("videoId");

        // The response contains the raw JSON returned by the Vimeo API, which is represented by Newtonsoft's
        // JObject. The Management API serializes responses using System.Text.Json, which isn't able to serialize a
        // JObject correctly, so the response is serialized using Newtonsoft instead.
        string json = JsonConvert.SerializeObject(new ApiVideoValue(credentials, video, parameters));

        return Content(json, "application/json");

    }

    #endregion

    #region Private helper methods

    /// <summary>
    /// Returns a result with the specified <paramref name="statusCode"/> and error <paramref name="code"/>. The
    /// error code is localized by the backoffice.
    /// </summary>
    /// <remarks>The error is returned as problem details with the code in the <c>operationStatus</c> extension, as
    /// the error interceptor of the backoffice discards any error body that isn't shaped like problem details.</remarks>
    private static IActionResult Error(int statusCode, string code) {

        ProblemDetails problemDetails = new() {
            Status = statusCode,
            Title = code,
            Type = "Error"
        };

        problemDetails.Extensions["operationStatus"] = code;

        return new ObjectResult(problemDetails) {
            StatusCode = statusCode
        };

    }

    #endregion

}
