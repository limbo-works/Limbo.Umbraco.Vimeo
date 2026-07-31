// [CHANGE: Umbraco 17 upgrade - icons must now be registered as an "icons" extension instead of being
// auto-discovered from App_Plugins/*/BackOffice/Icons] Related: icon-limbo-vimeo.js, Manifests/VimeoPackageManifestReader.cs

export default [
    {
        name: "icon-limbo-vimeo",
        path: () => import("./icon-limbo-vimeo.js")
    },
    {
        // Same artwork as the icon above, kept for backwards compatibility with data types and document types
        // referencing it. Hidden so it doesn't show up as a duplicate in the icon picker.
        name: "icon-limbo-vimeo-alt",
        hidden: true,
        path: () => import("./icon-limbo-vimeo.js")
    }
];
