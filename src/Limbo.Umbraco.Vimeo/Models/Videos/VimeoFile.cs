using Limbo.Umbraco.Video.Models.Videos;
using Skybrud.Social.Vimeo.Models.Videos;

namespace Limbo.Umbraco.Vimeo.Models.Videos {

    /// <summary>
    /// Class representing a Vimeo video file.
    /// </summary>
    public class VimeoFile : VideoFile {
        
        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The video file from the Vimeo API.</param>
        public VimeoFile(VimeoVideoFile file) : base(file.JObject) { }

    }

}