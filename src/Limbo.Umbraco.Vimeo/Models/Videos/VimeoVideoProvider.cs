using Limbo.Umbraco.Video.Models.Providers;

namespace Limbo.Umbraco.Vimeo.Models.Videos {

    /// <summary>
    /// Class with limited information about a video provider.
    /// </summary>
    public class VimeoVideoProvider : VideoProvider {

        /// <summary>
        /// Gets a reference to a <see cref="VimeoVideoProvider"/> instance.
        /// </summary>
        public static readonly VimeoVideoProvider Default = new();

        private VimeoVideoProvider() : base("vimeo", "Vimeo") { }

    }

}