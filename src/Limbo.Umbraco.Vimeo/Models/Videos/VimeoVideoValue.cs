using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.Video.Models.Providers;
using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.Vimeo.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;

namespace Limbo.Umbraco.Vimeo.Models.Videos;

/// <summary>
/// Class representing the value of the <see cref="VimeoVideoPropertyEditor"/> property editor.
/// </summary>
public class VimeoVideoValue : IVideoValue {

    #region Properties

    /// <summary>
    /// Gets the source (URL or embed code) as entered by the user.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; }

    /// <summary>
    /// Gets information about the video provider.
    /// </summary>
    [JsonProperty("provider")]
    public VimeoVideoProvider Provider { get; }

    /// <summary>
    /// Gets the embed parameters specified for the video.
    /// </summary>
    [JsonIgnore]
    public VimeoVideoParameters Parameters { get; }

    /// <summary>
    /// Gets the details about the picked video.
    /// </summary>
    [JsonProperty("details")]
    public VimeoVideoDetails Details { get; }

    /// <summary>
    /// Gets embed information for the video.
    /// </summary>
    [JsonProperty("embed")]
    public VimeoVideoEmbed Embed { get; }

    IVideoProvider IVideoValue.Provider => Provider;

    IVideoDetails IVideoValue.Details => Details;

    IVideoEmbed IVideoValue.Embed => Embed;

    #endregion

    #region Constructors

    private VimeoVideoValue(JObject json, VimeoVideoConfiguration? config) {
        Source = json.GetRequiredString("source");
        Provider = VimeoVideoProvider.Default;
        Parameters = json.GetObject("parameters", VimeoVideoParameters.Parse)!;
        Details = json.GetObject("video", VimeoVideoDetails.Parse)!;
        Embed = new VimeoVideoEmbed(Details, Parameters, config);
    }

    #endregion

    #region Static methods

    [return: NotNullIfNotNull(nameof(json))]
    internal static VimeoVideoValue? Parse(JObject? json, VimeoVideoConfiguration? config) {
        return json == null ? null : new VimeoVideoValue(json, config);
    }

    #endregion

}