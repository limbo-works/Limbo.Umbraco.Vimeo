namespace Limbo.Umbraco.Vimeo {

    /// <summary>
    /// Static class with various constants used throughout this package.
    /// </summary>
    public static class VimeoConstants {

        /// <summary>
        /// A regular expression used to match known Vimeo video URLs.
        /// </summary>
        public const string RegexUrlPattern = @"^(?:http|https)?:?\/?\/?(?:www\.)?(?:player\.)?vimeo\.com\/(?:channels\/\w+/(?<id>\d+)|groups\/\w+/videos\/(?<id>\d+)|video\/(?<id>\d+)|(?<id>\d+)|(?<id>\d+)\/(?<hash>\w+))$";

    }

}