using System.Collections.Generic;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Strings;

namespace Limbo.Umbraco.Vimeo.Extensions {
    
    internal static class VimeoExtensions {
        
        public static void Deconstruct<T>(this IList<T> list, out T first, out T second) {
            // TODO: Move to Skybrud.Essentials
            first = list.Count > 0 ? list[0] : default;
            second = list.Count > 1 ? list[1] : default;
        }

        public static bool TryGetBoolean(this IHttpQueryString query, string key, out bool result) {
            // TODO: Move to Skybrud.Essentials.Http
            return StringUtils.TryParseBoolean(query[key], out result);
        }

    }

}