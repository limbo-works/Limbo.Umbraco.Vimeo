// [CHANGE: Umbraco 17 upgrade - replaces wwwroot/Lang/da-DK.xml, which is no longer read by the backoffice]
// Related: en-US.js, Manifests/VimeoPackageManifestReader.cs, Constants/VimeoErrorCodes.cs

export default {
    limboVimeo: {
        video: "Video",
        id: "ID",
        title: "Titel",
        duration: "Længde",
        refresh: "Genindlæs den valgte video",
        urlOrEmbedCode: "URL eller embed-kode",
        urlPlaceholder: "Angiv videoens URL eller embed-kode her...",
        inherit: "Nedarv",
        enabled: "Slået til",
        disabled: "Slået fra",
        custom: "Brugerdefineret",
        selectColor: "Vælg farve",
        errorNoSource: "Ingen URL eller embed-kode angivet.",
        errorInvalidSource: "Den angivne værdi matcher ikke en gyldig URL eller embed-kode.",
        errorNoCredentials: "Ingen credentials konfigureret for Vimeo.",
        errorVideoNotFound: "En video med den angivne URL eller embed-kode blev ikke fundet.",
        errorGetVideoFailed: "Der opstod en fejl i forespørgslen til Vimeo's API. Se Umbraco's log for flere oplysninger eller kontakt din administrator hvis fejlen fortsætter."
    }
};
