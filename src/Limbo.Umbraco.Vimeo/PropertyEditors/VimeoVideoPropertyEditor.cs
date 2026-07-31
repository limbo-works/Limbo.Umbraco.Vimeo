// [CHANGE: Umbraco 17 upgrade - the DataEditor attribute no longer accepts a name, view, group or icon; those are
// now declared by the property editor UI in the package manifest]
// Related: Manifests/VimeoPackageManifestReader.cs, VimeoVideoConfigurationEditor.cs, wwwroot/Elements/Video.js

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors;

/// <summary>
/// Represents the property editor schema (data editor) of the Vimeo video picker.
/// </summary>
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class VimeoVideoPropertyEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    /// <summary>
    /// Gets the alias of the property editor schema. The alias is the only link between this class and the
    /// <c>propertyEditorSchemaAlias</c> of the property editor UI registered in the package manifest.
    /// </summary>
    public const string EditorAlias = "Limbo.Umbraco.Vimeo";

    /// <summary>
    /// Gets the alias of the property editor UI registered in the package manifest.
    /// </summary>
    public const string EditorUiAlias = "Limbo.Umbraco.Vimeo.Video";

    public const string EditorName = "Limbo Vimeo Video";

    public const string EditorIcon = "icon-limbo-vimeo-alt";

    #endregion

    #region Constructors

    public VimeoVideoPropertyEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    #endregion

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new VimeoVideoConfigurationEditor(_ioHelper);
    }

    #endregion

}
