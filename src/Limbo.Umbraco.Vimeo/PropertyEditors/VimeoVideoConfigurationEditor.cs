// [CHANGE: Umbraco 17 upgrade - IEditorConfigurationParser was removed, and configuration field views are no longer
// rewritten in C# as the editors are declared in the package manifest]
// Related: VimeoVideoConfiguration.cs, VimeoVideoPropertyEditor.cs, Manifests/VimeoPackageManifestReader.cs

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.Vimeo.PropertyEditors;

public class VimeoVideoConfigurationEditor : ConfigurationEditor<VimeoVideoConfiguration> {

    public VimeoVideoConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) { }

}
