angular.module("umbraco.services").factory("vimeoService", function ($http, limboVideoService) {

    // Get relevant settings from Umbraco's server variables
    const cacheBuster = Umbraco.Sys.ServerVariables.application.cacheBuster;
    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;
    
    // Fetches information about the video from our underlying API
    function getVideo(source) {

        const data = {
            source: source
        };

        return $http({
            method: "POST",
            url: `${umbracoPath}/backoffice/Limbo/Vimeo/GetVideo`,
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            data: $.param(data)
        });

    }

    // Returns a thumbnail object for the video
    function getThumbnail(video) {
        const p = video && video.pictures ? video.pictures.sizes.find(x => x.width >= 200) : null;
        return p ? { width: p.width, height: p.height, url: p.link } : null;
    }
    
    return {
        getVideo: getVideo,
        getThumbnail: getThumbnail,
        getDuration: function (seconds) {
            if (!seconds) return null;
            if (typeof seconds === "object" && seconds.duration) seconds = seconds.duration;
            return limboVideoService.getDuration(seconds);
        }
    }

});