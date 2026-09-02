// [CHANGE: Umbraco 17 upgrade - data type configuration is deserialized with System.Text.Json, which ignores the
// Newtonsoft converter, so the string enum converter is now declared for both serializers]
// Related: VideoLoop.cs, PropertyEditors/VimeoVideoConfiguration.cs, Manifests/VimeoPackageManifestReader.cs

using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.Models.Videos;

[JsonConverter(typeof(EnumCamelCaseConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum VimeoAutoplay {

    Inherit,

    Enabled,

    Disabled

}
