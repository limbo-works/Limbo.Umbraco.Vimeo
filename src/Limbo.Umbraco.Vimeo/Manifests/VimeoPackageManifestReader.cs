// [CHANGE: Umbraco 17 upgrade - IManifestFilter was removed, so VimeoManifestFilter has been replaced by this
// IPackageManifestReader] Related: Composers/VimeoComposer.cs, wwwroot/Elements/*.js, PropertyEditors/VimeoVideoPropertyEditor.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.Vimeo.PropertyEditors;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

using static Limbo.Umbraco.Vimeo.VimeoPackage;

namespace Limbo.Umbraco.Vimeo.Manifests;

#pragma warning disable CS1591

/// <summary>
/// Package manifest reader responsible for registering the backoffice extensions of this package.
/// </summary>
public class VimeoPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        List<PackageManifest> manifests = [
            new() {
                Id = Alias,
                Name = Name,
                Version = InformationalVersion,
                AllowTelemetry = true,
                Extensions = [

                    new {
                        type = "localization",
                        Alias = $"{Alias}.Localization.En",
                        name = "English",
                        js = $"/App_Plugins/{Alias}/Localization/en-US.js",
                        meta = new {
                            culture = "en"
                        }
                    },

                    new {
                        type = "localization",
                        Alias = $"{Alias}.Localization.Da",
                        name = "Danish",
                        js = $"/App_Plugins/{Alias}/Localization/da-DK.js",
                        meta = new {
                            culture = "da"
                        }
                    },

                    new {
                        type = "icons",
                        Alias = $"{Alias}.Icons",
                        name = "Limbo Vimeo Icons",
                        js = $"/App_Plugins/{Alias}/Icons/icons.js"
                    },

                    // The property editor UI shown when editing a property using this package's property editor.
                    // The "settings" describe the configuration fields of the data type - which in Umbraco 13 and
                    // below were declared via the "ConfigurationField" attributes in C#.
                    new {
                        type = "propertyEditorUi",
                        Alias = VimeoVideoPropertyEditor.EditorUiAlias,
                        name = VimeoVideoPropertyEditor.EditorName,
                        element = $"/App_Plugins/{Alias}/Elements/Video.js",
                        elementName = "limbo-vimeo-video",
                        meta = new {
                            label = VimeoVideoPropertyEditor.EditorName,
                            icon = VimeoVideoPropertyEditor.EditorIcon,
                            group = VimeoVideoPropertyEditor.EditorGroup,
                            propertyEditorSchemaAlias = VimeoVideoPropertyEditor.EditorAlias,
                            settings = new {
                                properties = new object[] {
                                    new {
                                        Alias = "autoplay",
                                        label = "Autoplay",
                                        description = "Select whether videos should autoplay when embedded.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        weight = 10
                                    },
                                    new {
                                        Alias = "loop",
                                        label = "Loop",
                                        description = "Select whether videos should loop.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        weight = 20
                                    },
                                    new {
                                        Alias = "color",
                                        label = "Color",
                                        description = "Select the color of the embedded player.",
                                        propertyEditorUiAlias = ColorUiAlias,
                                        weight = 30
                                    },
                                    new {
                                        Alias = "showTitle",
                                        label = "Show title",
                                        description = "Select whether the video title should be shown in the embedded player.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        config = BooleanButtonListConfig,
                                        weight = 40
                                    },
                                    new {
                                        Alias = "showByLine",
                                        label = "Show by line",
                                        description = "Select whether the video by line should be shown in the embedded player.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        config = BooleanButtonListConfig,
                                        weight = 50
                                    },
                                    new {
                                        Alias = "showPortrait",
                                        label = "Show portrait",
                                        description = "Select whether the video author portrait should be shown in the embedded player.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        config = BooleanButtonListConfig,
                                        weight = 60
                                    }
                                },
                                defaultData = new object[] {
                                    new { Alias = "autoplay", value = "inherit" },
                                    new { Alias = "loop", value = "inherit" },
                                    new { Alias = "color", value = "inherit" }
                                }
                            }
                        }
                    },

                    // Property editor UI used for a few of the configuration fields above. It doesn't specify a
                    // "propertyEditorSchemaAlias" as it may only be used to configure other property editors.
                    new {
                        type = "propertyEditorUi",
                        Alias = ButtonListUiAlias,
                        name = "Limbo Vimeo Button List",
                        element = $"/App_Plugins/{Alias}/Elements/ButtonList.js",
                        elementName = "limbo-vimeo-button-list",
                        meta = new {
                            label = "Limbo Vimeo Button List",
                            icon = VimeoVideoPropertyEditor.EditorIcon,
                            group = "media"
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        Alias = ColorUiAlias,
                        name = "Limbo Vimeo Color",
                        element = $"/App_Plugins/{Alias}/Elements/Color.js",
                        elementName = "limbo-vimeo-color",
                        meta = new {
                            label = "Limbo Vimeo Color",
                            icon = VimeoVideoPropertyEditor.EditorIcon,
                            group = "media"
                        }
                    }

                ]
            }
        ];

        return await Task.FromResult(manifests);

    }

    #region Constants

    /// <summary>
    /// Gets the Alias of the property editor UI used for the tri-state configuration fields.
    /// </summary>
    public const string ButtonListUiAlias = $"{Alias}.ButtonList";

    /// <summary>
    /// Gets the Alias of the property editor UI used for the "color" configuration field.
    /// </summary>
    public const string ColorUiAlias = $"{Alias}.Color";

    /// <summary>
    /// Configuration telling the button list UI to store the value as a nullable boolean rather than a string.
    /// </summary>
    private static object[] BooleanButtonListConfig => [
        new { alias = "valueType", value = "boolean" }
    ];

    #endregion

}
