using Limbo.Umbraco.Video.Models.Videos;
using Skybrud.Social.Vimeo.Models.Videos;

namespace Limbo.Umbraco.Vimeo.Models.Videos {

    /// <summary>
    /// Class representing a Vimeo thumbnail.
    /// </summary>
    public class VimeoThumbnail : VideoThumbnail {

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="picture"/>.
        /// </summary>
        /// <param name="picture">The video picture from the Vimeo API.</param>
        public VimeoThumbnail(VimeoVideoPicture picture) : base(picture.JObject) { }

    }

}