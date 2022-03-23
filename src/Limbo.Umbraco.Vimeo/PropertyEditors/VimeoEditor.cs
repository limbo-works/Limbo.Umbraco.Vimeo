using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors {

    /// <summary>
    /// Represents a block list property editor.
    /// </summary>
    [DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
    public class VimeoEditor : DataEditor {

        #region Constants

        internal const string EditorAlias = "Limbo.Umbraco.Vimeo";

        internal const string EditorName = "Limbo Vimeo Video";

        internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Vimeo/Views/Video.html";

        internal const string EditorIcon = "icon-vimeo color-limbo";

        #endregion

        #region Constructors

        public VimeoEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

        #endregion

    }

}