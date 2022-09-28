using Limbo.Umbraco.Vimeo.Models.Credentials;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Vimeo.Models.Videos;

namespace Limbo.Umbraco.Vimeo.Models.Api {

    internal class ApiVideoValue {

        [JsonProperty("credentials")]
        public object Credentials { get; }

        [JsonProperty("video")]
        public object Video { get; }

        [JsonProperty("parameters")]
        public object Parameters { get; }

        public ApiVideoValue(VimeoCredentials credentials, VimeoVideo video, JObject parameters) {
            Credentials = new { key = credentials.Key };
            Video = video.JObject;
            Parameters = parameters;
        }

    }

}