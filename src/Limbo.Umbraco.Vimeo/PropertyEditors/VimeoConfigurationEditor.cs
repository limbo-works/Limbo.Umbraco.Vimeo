using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors {

    public class VimeoConfigurationEditor : ConfigurationEditor<VimeoConfiguration> {

        public VimeoConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser) { }

    }

}