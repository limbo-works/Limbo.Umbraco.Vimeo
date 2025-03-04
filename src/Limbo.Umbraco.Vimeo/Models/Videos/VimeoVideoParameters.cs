using Limbo.Umbraco.Vimeo.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Skybrud.Essentials.Strings.Extensions;

namespace Limbo.Umbraco.Vimeo.Models.Videos;

/// <summary>
/// Class representing the parameters parsed from the source field of the <see cref="VimeoVideoPropertyEditor"/> property editor.
/// </summary>
public class VimeoVideoParameters {

    #region Properties

    /// <summary>
    /// Gets whether embedded videos should automatically start playing.
    /// </summary>
    [JsonProperty("autoplay", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Autoplay { get; }

    /// <summary>
    /// Gets whether embedded videos should automatically start playing.
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

    #endregion

    #region Constructors

    internal VimeoVideoParameters(JObject? json) {
        Autoplay = json.GetBooleanOrNull("autoplay");
        Loop = json.GetBooleanOrNull("loop");
        Color = json.GetString("color").NullIfWhiteSpace();
        ShowTitle = json.GetBooleanOrNull("title");
        ShowByLine = json.GetBooleanOrNull("byline");
        ShowPortrait = json.GetBooleanOrNull("portrait");
    }

    #endregion

    #region Static methods

    internal static VimeoVideoParameters Parse(JObject? json) {
        return new VimeoVideoParameters(json);
    }

    #endregion

}