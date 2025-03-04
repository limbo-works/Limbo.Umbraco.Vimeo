using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors;

public class VimeoVideoConfigurationEditor : ConfigurationEditor<VimeoVideoConfiguration> {

    public VimeoVideoConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser) {

        foreach (ConfigurationField field in Fields) {

            if (field.View is not null) {

                field.View = field.View
                    .Replace("{version}", VimeoPackage.InformationalVersion)
                    .Replace("{alias}", field.Key);

            }

        }

    }

}