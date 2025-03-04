using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.Models.Videos;

[JsonConverter(typeof(EnumCamelCaseConverter))]
public enum VimeoAutoplay {

    Inherit,

    Enabled,

    Disabled

}