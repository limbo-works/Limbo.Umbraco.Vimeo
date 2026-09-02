// [CHANGE: Umbraco 17 upgrade - IManifestFilter was removed, so VimeoManifestFilter has been replaced by this
// IPackageManifestReader] Related: Composers/VimeoComposer.cs, wwwroot/Elements/*.js, PropertyEditors/VimeoVideoPropertyEditor.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.Vimeo.PropertyEditors;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Limbo.Umbraco.Vimeo.Manifests;

#pragma warning disable CS1591

/// <summary>
/// Package manifest reader responsible for registering the backoffice extensions of this package.
/// </summary>
public class VimeoPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = VimeoPackage.Alias;

        // The informational version is hashed and appended to the URLs of the JavaScript files so browsers will
        // pick up new versions of the files rather than using a cached version
        string cacheBuster = VimeoPackage.InformationalVersion.ToMd5Hash();

        List<PackageManifest> manifests = [
            new() {
                Id = alias,
                Name = VimeoPackage.Name,
                Version = VimeoPackage.InformationalVersion,
                AllowTelemetry = true,
                Extensions = [

                    new {
                        type = "localization",
                        alias = $"{alias}.Localization.En",
                        name = "English",
                        js = $"/App_Plugins/{alias}/Localization/en-US.js?v={cacheBuster}",
                        meta = new {
                            culture = "en"
                        }
                    },

                    new {
                        type = "localization",
                        alias = $"{alias}.Localization.Da",
                        name = "Danish",
                        js = $"/App_Plugins/{alias}/Localization/da-DK.js?v={cacheBuster}",
                        meta = new {
                            culture = "da"
                        }
                    },

                    new {
                        type = "icons",
                        alias = $"{alias}.Icons",
                        name = "Limbo Vimeo Icons",
                        js = $"/App_Plugins/{alias}/Icons/icons.js?v={cacheBuster}"
                    },

                    // The property editor UI shown when editing a property using this package's property editor.
                    // The "settings" describe the configuration fields of the data type - which in Umbraco 13 and
                    // below were declared via the "ConfigurationField" attributes in C#.
                    new {
                        type = "propertyEditorUi",
                        alias = VimeoVideoPropertyEditor.EditorUiAlias,
                        name = VimeoVideoPropertyEditor.EditorName,
                        element = $"/App_Plugins/{alias}/Elements/Video.js?v={cacheBuster}",
                        elementName = "limbo-vimeo-video",
                        meta = new {
                            label = VimeoVideoPropertyEditor.EditorName,
                            icon = VimeoVideoPropertyEditor.EditorIcon,
                            group = "Limbo",
                            propertyEditorSchemaAlias = VimeoVideoPropertyEditor.EditorAlias,
                            settings = new {
                                properties = new object[] {
                                    new {
                                        alias = "autoplay",
                                        label = "Autoplay",
                                        description = "Select whether videos should autoplay when embedded.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        weight = 10
                                    },
                                    new {
                                        alias = "loop",
                                        label = "Loop",
                                        description = "Select whether videos should loop.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        weight = 20
                                    },
                                    new {
                                        alias = "color",
                                        label = "Color",
                                        description = "Select the color of the embedded player.",
                                        propertyEditorUiAlias = ColorUiAlias,
                                        weight = 30
                                    },
                                    new {
                                        alias = "showTitle",
                                        label = "Show title",
                                        description = "Select whether the video title should be shown in the embedded player.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        config = BooleanButtonListConfig,
                                        weight = 40
                                    },
                                    new {
                                        alias = "showByLine",
                                        label = "Show by line",
                                        description = "Select whether the video by line should be shown in the embedded player.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        config = BooleanButtonListConfig,
                                        weight = 50
                                    },
                                    new {
                                        alias = "showPortrait",
                                        label = "Show portrait",
                                        description = "Select whether the video author portrait should be shown in the embedded player.",
                                        propertyEditorUiAlias = ButtonListUiAlias,
                                        config = BooleanButtonListConfig,
                                        weight = 60
                                    }
                                },
                                defaultData = new object[] {
                                    new { alias = "autoplay", value = "inherit" },
                                    new { alias = "loop", value = "inherit" },
                                    new { alias = "color", value = "inherit" }
                                }
                            }
                        }
                    },

                    // Property editor UI used for a few of the configuration fields above. It doesn't specify a
                    // "propertyEditorSchemaAlias" as it may only be used to configure other property editors.
                    new {
                        type = "propertyEditorUi",
                        alias = ButtonListUiAlias,
                        name = "Limbo Vimeo Button List",
                        element = $"/App_Plugins/{alias}/Elements/ButtonList.js?v={cacheBuster}",
                        elementName = "limbo-vimeo-button-list",
                        meta = new {
                            label = "Limbo Vimeo Button List",
                            icon = VimeoVideoPropertyEditor.EditorIcon,
                            group = "media"
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = ColorUiAlias,
                        name = "Limbo Vimeo Color",
                        element = $"/App_Plugins/{alias}/Elements/Color.js?v={cacheBuster}",
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
    /// Gets the alias of the property editor UI used for the tri-state configuration fields.
    /// </summary>
    public const string ButtonListUiAlias = $"{VimeoPackage.Alias}.ButtonList";

    /// <summary>
    /// Gets the alias of the property editor UI used for the "color" configuration field.
    /// </summary>
    public const string ColorUiAlias = $"{VimeoPackage.Alias}.Color";

    /// <summary>
    /// Configuration telling the button list UI to store the value as a nullable boolean rather than a string.
    /// </summary>
    private static object[] BooleanButtonListConfig => [
        new { alias = "valueType", value = "boolean" }
    ];

    #endregion

}
