using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors {

    /// <summary>
    /// Represents a block list property editor.
    /// </summary>
    [DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
    public class VimeoEditor : DataEditor {

        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        #region Constants

        internal const string EditorAlias = "Limbo.Umbraco.Vimeo";

        internal const string EditorName = "Limbo Vimeo Video";

        internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Vimeo/Views/Video.html";

        internal const string EditorIcon = "icon-limbo-vimeo-alt color-limbo";

        #endregion

        #region Constructors

        public VimeoEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser, IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) {
            _ioHelper = ioHelper;
            _editorConfigurationParser = editorConfigurationParser;
        }

        #endregion

        #region Member methods

        public override IDataValueEditor GetValueEditor(object? configuration) {
            IDataValueEditor editor = base.GetValueEditor(configuration);
            if (editor is DataValueEditor dve) dve.View += $"?v={VimeoPackage.InformationalVersion}";
            return editor;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() {
            return new VimeoConfigurationEditor(_ioHelper, _editorConfigurationParser);
        }

        #endregion

    }

}