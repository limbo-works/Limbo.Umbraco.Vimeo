using Limbo.Umbraco.Vimeo.PropertyEditors;

namespace Limbo.Umbraco.Vimeo.Constants;

/// <summary>
/// Static class with constants for property editor UI in this package.
/// </summary>
public static class VimeoPropertyEditorUiAliases {

    /// <summary>
    /// Gets the alias of the property editor UI used for the Vimeo video picker.
    /// </summary>
    public const string Video = VimeoVideoPropertyEditor.EditorUiAlias;

    /// <summary>
    /// Gets the alias of the property editor UI used for the tri-state configuration fields.
    /// </summary>
    public const string ButtonList = $"{VimeoPackage.Alias}.ButtonList";

    /// <summary>
    /// Gets the alias of the property editor UI used for the "color" configuration field.
    /// </summary>
    public const string Color = $"{VimeoPackage.Alias}.Color";

}