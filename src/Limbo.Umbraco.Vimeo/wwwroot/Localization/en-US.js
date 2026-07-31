// [CHANGE: Umbraco 17 upgrade - replaces wwwroot/Lang/en-US.xml, which is no longer read by the backoffice]
// Related: da-DK.js, Manifests/VimeoPackageManifestReader.cs, Constants/VimeoErrorCodes.cs

export default {
    limboVimeo: {
        video: "Video",
        id: "ID",
        title: "Title",
        duration: "Duration",
        refresh: "Refresh current video",
        urlOrEmbedCode: "URL or embed code",
        urlPlaceholder: "Enter the URL or embed code of the video here...",
        inherit: "Inherit",
        enabled: "Enabled",
        disabled: "Disabled",
        custom: "Custom",
        selectColor: "Select color",
        errorNoSource: "No URL or embed code specified.",
        errorInvalidSource: "Source doesn't match a valid URL or embed code.",
        errorNoCredentials: "No credentials configured for Vimeo.",
        errorVideoNotFound: "A video with the specified URL or embed code wasn't found.",
        errorGetVideoFailed: "The request to the Vimeo API failed. Check the Umbraco log for further details or contact your administrator if the problem persists."
    }
};
