// [CHANGE: Umbraco 17 upgrade - port of the AngularJS "Color" data type config controller]
// Related: Video.js, ButtonList.js, Manifests/VimeoPackageManifestReader.cs, Models/Videos/VimeoVideoEmbed.cs

import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";

const defaultColor = "00adef";

// The value of this setting is either "inherit", "disabled" or a six digit hex color (without the leading "#").
// See VimeoVideoEmbed for how the three states affect the embedded player.
export class LimboVimeoColorElement extends UmbElementMixin(LitElement) {

    static properties = {
        value: { type: String },
        _color: { state: true }
    };

    constructor() {
        super();
        this.value = "inherit";
        this._color = defaultColor;
    }

    get #isCustom() {
        return this.value?.length === 6;
    }

    // [CHANGE: code review - "_color" was only refreshed when leaving custom mode, so a saved hex color was reset to
    // the default when the already active "Custom" button was clicked] Related: Video.js, ButtonList.js
    willUpdate(changed) {
        if (changed.has("value") && this.#isCustom) this._color = this.value;
    }

    #select(mode) {

        if (mode === "custom") {
            this.value = this._color ?? defaultColor;
        } else {
            if (this.#isCustom) this._color = this.value;
            this.value = mode;
        }

        this.dispatchEvent(new UmbChangeEvent());

    }

    #onColorChange(e) {
        const color = e.target.value ?? "";
        this._color = color.replace("#", "");
        this.value = this._color;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #look(active) {
        return active ? "primary" : "outline";
    }

    render() {
        return html`
            <div class="wrapper">

                <div class="button-list">
                    <uui-button
                        look=${this.#look(this.value === "inherit" || !this.value)}
                        color=${this.value === "inherit" || !this.value ? "positive" : "default"}
                        label=${this.localize.term("limboVimeo_inherit")}
                        @click=${() => this.#select("inherit")}></uui-button>
                    <uui-button
                        look=${this.#look(this.#isCustom)}
                        color=${this.#isCustom ? "positive" : "default"}
                        label=${this.localize.term("limboVimeo_custom")}
                        @click=${() => this.#select("custom")}></uui-button>
                    <uui-button
                        look=${this.#look(this.value === "disabled")}
                        color=${this.value === "disabled" ? "positive" : "default"}
                        label=${this.localize.term("limboVimeo_disabled")}
                        @click=${() => this.#select("disabled")}></uui-button>
                </div>

                ${this.#isCustom
                    ? html`
                        <div class="picker">
                            <label for="color"><umb-localize key="limboVimeo_selectColor">Select color</umb-localize></label>
                            <input id="color" type="color" .value=${`#${this.value}`} @change=${this.#onColorChange} />
                            <code>#${this.value}</code>
                        </div>
                    `
                    : nothing}

            </div>
        `;
    }

    static styles = css`
        :host {
            display: block;
        }

        .wrapper {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            gap: var(--uui-size-space-4);
        }

        .button-list {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-2);
        }

        .picker {
            display: flex;
            align-items: center;
            gap: var(--uui-size-space-2);
        }

        input[type="color"] {
            width: 40px;
            height: 32px;
            padding: 0;
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: none;
            cursor: pointer;
        }
    `;

}

customElements.define("limbo-vimeo-color", LimboVimeoColorElement);

export default LimboVimeoColorElement;
