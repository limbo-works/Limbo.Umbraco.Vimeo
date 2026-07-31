// [CHANGE: Umbraco 17 upgrade - ConfigurationField no longer takes a label, view or description; labels and
// editors for these fields are declared by the property editor UI in the package manifest]
// Related: Manifests/VimeoPackageManifestReader.cs, VimeoVideoConfigurationEditor.cs, wwwroot/Elements/ButtonList.js

using Limbo.Umbraco.Vimeo.Models.Videos;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors;

/// <summary>
/// Class representing the configuration of the <see cref="VimeoVideoPropertyEditor"/> property editor.
/// </summary>
/// <remarks>Umbraco deserializes the configuration using <c>System.Text.Json</c>, where the key of the
/// <see cref="ConfigurationFieldAttribute"/> is used as the name of the underlying JSON property.</remarks>
public class VimeoVideoConfiguration {

    // [CHANGE: code review - the two enums are nullable as System.Text.Json throws when deserializing a stored null
    // into a non-nullable enum, where the Newtonsoft converter used up until Umbraco 13 fell back to the default]
    // Related: Models/Videos/VimeoAutoplay.cs, Models/Videos/VideoLoop.cs, Models/Videos/VimeoVideoEmbed.cs

    /// <summary>
    /// Gets or sets whether embedded videos should automatically start playing. A value of <see langword="null"/> is
    /// treated the same as <see cref="VimeoAutoplay.Inherit"/>.
    /// </summary>
    [ConfigurationField("autoplay")]
    public VimeoAutoplay? Autoplay { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should loop. A value of <see langword="null"/> is treated the same as
    /// <see cref="VimeoLoop.Inherit"/>.
    /// </summary>
    [ConfigurationField("loop")]
    public VimeoLoop? Loop { get; set; }

    /// <summary>
    /// Gets or sets the color of the embedded player. The value is either <c>inherit</c>, <c>disabled</c> or a six
    /// digit hex color.
    /// </summary>
    [ConfigurationField("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets whether the video title should be shown in the embedded player. A value of <see langword="null"/>
    /// means that the setting is inherited from the embed code entered by the user.
    /// </summary>
    [ConfigurationField("showTitle")]
    public bool? ShowTitle { get; set; }

    /// <summary>
    /// Gets or sets whether the video by line should be shown in the embedded player. A value of
    /// <see langword="null"/> means that the setting is inherited from the embed code entered by the user.
    /// </summary>
    [ConfigurationField("showByLine")]
    public bool? ShowByLine { get; set; }

    /// <summary>
    /// Gets or sets whether the video author portrait should be shown in the embedded player. A value of
    /// <see langword="null"/> means that the setting is inherited from the embed code entered by the user.
    /// </summary>
    [ConfigurationField("showPortrait")]
    public bool? ShowPortrait { get; set; }

}
