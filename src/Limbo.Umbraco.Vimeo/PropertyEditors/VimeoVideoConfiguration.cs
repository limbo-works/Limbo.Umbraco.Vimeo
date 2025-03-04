using Limbo.Umbraco.Vimeo.Models.Videos;
using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors;

public class VimeoVideoConfiguration {

    /// <summary>
    /// Gets or sets whether embedded videos should automatically start playing.
    /// </summary>
    [ConfigurationField("autoplay",
        "Autoplay",
        $"/App_Plugins/{VimeoPackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether videos should autoplay when embedded.")]
    public VimeoAutoplay Autoplay { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should loop.
    /// </summary>
    [ConfigurationField("loop",
        "Loop",
        $"/App_Plugins/{VimeoPackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether videos should loop.")]
    public VimeoLoop Loop { get; set; }

    [ConfigurationField("color", "Color", $"/App_Plugins/{VimeoPackage.Alias}/Views/Color.html?type={{alias}}",
        Description = "Select the color of the embedded player.")]
    public string? Color { get; set; }

    [ConfigurationField("showTitle", "Show title", $"/App_Plugins/{VimeoPackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether the video title should be shown in the embedded player.")]
    public bool? ShowTitle { get; set; }

    [ConfigurationField("showByLine", "Show by line", $"/App_Plugins/{VimeoPackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether the video by line should be shown in the embedded player.")]
    public bool? ShowByLine { get; set; }

    [ConfigurationField("showPortrait", "Show portrait", $"/App_Plugins/{VimeoPackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether the video author portrait should be shown in the embedded player.")]
    public bool? ShowPortrait { get; set; }

    [ConfigurationField("hideLabel", "Hide label", "boolean", Description = "Select whether the label and description of properties using this data type should be hidden.<br /><br />Hiding the label and description can be useful in some cases - eg. to give the video picker a bit more horizontal space.")]
    [JsonProperty("hideLabel")]
    public bool HideLabel { get; set; }

}