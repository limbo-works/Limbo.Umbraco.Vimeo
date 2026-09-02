using System;
using Limbo.Umbraco.Video.Models.Credentials;

namespace Limbo.Umbraco.Vimeo.Models.Settings;

/// <summary>
/// Class with information about the credentials used for accessing the Vimeo API.
/// </summary>
public class VimeoCredentials : ICredentials {

    /// <summary>
    /// Gets the key of the credentials.
    /// </summary>
    public required Guid Key { get; init; }

    /// <summary>
    /// Gets the friendly name of the credentials.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description of the credentials.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// If configured, gets the Vimeo access token.
    /// </summary>
    public required string AccessToken { get; init; }

}