// [CHANGE: Umbraco 17 upgrade - replaces VimeoTranslations; the backoffice no longer reads the XML language files,
// so the API returns stable error codes which are localized client side]
// Related: Controllers/VimeoController.cs, wwwroot/Elements/Video.js, wwwroot/Localization/en-US.js

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.Constants;

/// <summary>
/// Static class with the error codes returned by <see cref="Controllers.VimeoController"/>.
/// </summary>
/// <remarks>Each code has a matching <c>limboVimeo_error*</c> entry in the localization files of this package.</remarks>
public static class VimeoErrorCodes {

    public const string NoSource = "noSource";

    public const string InvalidSource = "invalidSource";

    public const string NoCredentials = "noCredentials";

    public const string VideoNotFound = "videoNotFound";

    public const string GetVideoFailed = "getVideoFailed";

}
