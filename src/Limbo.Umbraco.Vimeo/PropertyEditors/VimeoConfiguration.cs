using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors {

    public class VimeoConfiguration {

        [ConfigurationField("hideLabel", "Hide label", "boolean")]
        [JsonProperty("hideLabel")]
        public bool HideLabel { get; set; }

    }

}