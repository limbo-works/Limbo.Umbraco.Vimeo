// [CHANGE: Umbraco 17 upgrade - IPublishedDataType.Configuration renamed to PublishedDataType.ConfigurationObject]
// Related: VimeoVideoConfiguration.cs, VimeoVideoPropertyEditor.cs, Models/Videos/VimeoVideoValue.cs

using System;
using Limbo.Umbraco.Vimeo.Models.Videos;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors;

/// <summary>
/// Property value converter for <see cref="VimeoVideoPropertyEditor"/>.
/// </summary>
public class VimeoVideoValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == VimeoVideoPropertyEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source switch {
            JObject json => json,
            string str => str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null,
            _ => null
        };
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        if (inter is not JObject json) return null;
        return json.GetObject("video") is null ? null : VimeoVideoValue.Parse(json, propertyType.DataType.ConfigurationObject as VimeoVideoConfiguration);
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(VimeoVideoValue);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}