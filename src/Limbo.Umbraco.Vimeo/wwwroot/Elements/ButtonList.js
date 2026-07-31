// [CHANGE: Umbraco 17 upgrade - port of the AngularJS "ButtonList" data type config controller]
// Related: Video.js, Color.js, Manifests/VimeoPackageManifestReader.cs, PropertyEditors/VimeoVideoConfiguration.cs

import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";

// Options used for the "autoplay" and "loop" settings, where the value is stored as a string
const stringOptions = [
    { value: "inherit", label: "limboVimeo_inherit" },
    { value: "enabled", label: "limboVimeo_enabled" },
    { value: "disabled", label: "limboVimeo_disabled" }
];

// Options used for the "show title", "show by line" and "show portrait" settings, where the value is stored as a
// nullable boolean - null meaning that the setting is inherited from the embed code (or Vimeo's own default).
const booleanOptions = [
    { value: null, label: "limboVimeo_inherit" },
    { value: true, label: "limboVimeo_enabled" },
    { value: false, label: "limboVimeo_disabled" }
];

export class LimboVimeoButtonListElement extends UmbElementMixin(LitElement) {

    static properties = {
        value: {},
        _options: { state: true }
    };

    constructor() {
        super();
        this.value = undefined;
        this._options = stringOptions;
    }

    // [CHANGE: code review - a setter without a matching getter made "config" read back as undefined]
    // Related: Video.js, Color.js
    set config(config) {
        this._config = config;
        // "valueType" is declared via the "config" collection of the setting in the package manifest
        const valueType = config?.getValueByAlias("valueType");
        this._options = valueType === "boolean" ? booleanOptions : stringOptions;
    }

    get config() {
        return this._config;
    }

    #isActive(option) {
        // The stored value may be undefined when the data type hasn't been saved yet, in which case the first
        // option ("inherit") is considered the active one.
        if (option.value === null) return this.value === null || this.value === undefined;
        if (option.value === "inherit") return this.value === "inherit" || this.value === undefined;
        return this.value === option.value;
    }

    #select(option) {
        this.value = option.value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    render() {
        return html`
            <div class="button-list">
                ${repeat(
                    this._options,
                    (option) => String(option.value),
                    (option) => html`
                        <uui-button
                            look=${this.#isActive(option) ? "primary" : "outline"}
                            color=${this.#isActive(option) ? "positive" : "default"}
                            label=${this.localize.term(option.label)}
                            @click=${() => this.#select(option)}></uui-button>
                    `
                )}
            </div>
        `;
    }

    static styles = css`
        :host {
            display: block;
        }

        .button-list {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-2);
        }
    `;

}

customElements.define("limbo-vimeo-button-list", LimboVimeoButtonListElement);

export default LimboVimeoButtonListElement;
