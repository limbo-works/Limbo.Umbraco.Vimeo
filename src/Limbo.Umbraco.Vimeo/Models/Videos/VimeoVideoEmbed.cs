using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.Vimeo.Options;
using Limbo.Umbraco.Vimeo.PropertyEditors;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters;
using Skybrud.Essentials.Strings.Extensions;

namespace Limbo.Umbraco.Vimeo.Models.Videos;

/// <summary>
/// Class representing the embed options of the video.
/// </summary>
public class VimeoVideoEmbed : IVideoEmbed {

    #region Properties

    /// <summary>
    /// Gets the embed URL.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; }

    /// <summary>
    /// Gets the embed hash.
    /// </summary>
    [JsonProperty("h", NullValueHandling = NullValueHandling.Ignore)]
    public string? Hash { get; }

    /// <summary>
    /// Gets whether embedded videos should automatically start playing.
    /// </summary>
    [JsonProperty("autoplay", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Autoplay { get; }

    /// <summary>
    /// Gets whether embedded videos should loop.
    /// </summary>
    [JsonProperty("loop", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Loop { get; }

    /// <summary>
    /// Gets the color of the player.
    /// </summary>
    [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
    public string? Color { get; }

    /// <summary>
    /// Gets whether the video title should be shown in the player.
    /// </summary>
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowTitle { get; }

    /// <summary>
    /// Gets whether the video by line should be shown in the player.
    /// </summary>
    [JsonProperty("byline", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowByLine { get; }

    /// <summary>
    /// Gets whether the video user portrait should be shown in the player.
    /// </summary>
    [JsonProperty("portrait", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowPortrait { get; }

    /// <summary>
    /// Gets the HTML embed code.
    /// </summary>
    [JsonProperty("html")]
    [JsonConverter(typeof(StringJsonConverter))]
    public IHtmlContent Html { get; }

    #endregion

    #region Constructors

    internal VimeoVideoEmbed(VimeoVideoDetails video, VimeoVideoParameters parameters, VimeoVideoConfiguration? config) {

        Hash = video.Data.PlayerEmbedHash;

        // The "Color" option on the data type may be either "inherit",
        // "disabled" or a 6-digit hex color. No value is treated the
        // same as "inherit".
        //
        // If a color has been explicitly set on the data type, we use that
        // regardless of what's set in "parameters" (e.g. if specified in an
        // embed code).
        //
        // If set to "disabled", we ignore the parameter entirely, so the
        // player will fall back to whatever is default in Vimeo (for the
        // associated account).
        //
        // If set to "inherit" (or not set at all), we look for a color in the
        // parameters object (aka if set via an embed code).
        Color = config?.Color switch {
            { Length: 6 } => config.Color,
            "disabled" => null,
            _ => parameters.Color.NullIfWhiteSpace()
        };

        Autoplay = config?.Autoplay switch {
            VimeoAutoplay.Enabled => true,
            VimeoAutoplay.Disabled => false,
            _ => parameters.Autoplay
        };

        Loop = config?.Loop switch {
            VimeoLoop.Enabled => true,
            VimeoLoop.Disabled => false,
            _ => parameters.Loop
        };

        ShowTitle = config?.ShowTitle ?? parameters.ShowTitle;
        ShowByLine = config?.ShowByLine ?? parameters.ShowByLine;
        ShowPortrait = config?.ShowPortrait ?? parameters.ShowPortrait;

        VimeoEmbedOptions o = new(video.Data, this);

        Url = o.GetEmbedUrl();
        Html = o.GetEmbedCode();

    }

    #endregion

}