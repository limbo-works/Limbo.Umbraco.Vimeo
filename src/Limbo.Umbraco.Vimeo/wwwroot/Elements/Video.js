// [CHANGE: Umbraco 17 upgrade - AngularJS controller + view replaced by a Lit based property editor UI]
// Related: ButtonList.js, Color.js, Manifests/VimeoPackageManifestReader.cs, Controllers/VimeoController.cs

import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, when, nothing } from "@umbraco-cms/backoffice/external/lit";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";
import { tryExecute } from "@umbraco-cms/backoffice/resources";

import "@limbo/video/elements/duration";

// Maps the error codes returned by the server to the localization keys of this package
const errorKeys = {
    noSource: "limboVimeo_errorNoSource",
    invalidSource: "limboVimeo_errorInvalidSource",
    noCredentials: "limboVimeo_errorNoCredentials",
    videoNotFound: "limboVimeo_errorVideoNotFound",
    getVideoFailed: "limboVimeo_errorGetVideoFailed"
};

export class LimboVimeoVideoElement extends UmbElementMixin(LitElement) {

    static properties = {
        value: { type: Object },
        readonly: { type: Boolean, reflect: true }
    };

    #value = null;
    #video = null;
    #loading = false;
    #error = null;
    #debounceTimer = null;
    #embed = false;
    #config = {};

    // Incremented for each request so responses arriving out of order can be discarded
    #requestToken = 0;

    constructor() {
        super();
        this.readonly = false;
    }

    // The configuration of the data type. Not used directly by the UI, but the setter is needed so Umbraco
    // doesn't complain about the property not being supported.
    set config(value) {
        this.#config = value;
    }

    get config() {
        return this.#config;
    }

    get value() {
        return this.#value;
    }

    set value(value) {
        const oldValue = this.#value;
        this.#value = value ?? null;
        this.#video = this.#parseDetails(this.#value);
        this.requestUpdate("value", oldValue);
        this.#updateTextareaMode();
    }

    get #videoId() {
        // The "uri" of a video is in the format "/videos/1234" or "/videos/1234:hash"
        return this.#video?.uri?.split("/")[2]?.split(":")[0] ?? null;
    }

    get #thumbnail() {
        const sizes = this.#video?.pictures?.sizes;
        if (!sizes?.length) return null;
        return sizes.find((x) => x.width >= 200) ?? sizes[sizes.length - 1];
    }

    connectedCallback() {
        super.connectedCallback();
        this.#video = this.#parseDetails(this.#value);
        this.#embed = this.#source.indexOf("<") >= 0;
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        window.clearTimeout(this.#debounceTimer);
    }

    updated() {
        this.#updateTextareaMode();
    }

    // Reads the raw Vimeo video out of the escaped JSON in "details._data".
    #parseDetails(value) {
        const details = value?.details ?? value?.data ?? value?.video;
        if (!details?._data) return null;
        try {
            return JSON.parse(details._data);
        } catch {
            return null;
        }
    }

    // Gets the source (URL or embed code) of the current value
    get #source() {
        return this.#value?.source ?? "";
    }

    async #updateTextareaMode() {

        const uui = this.shadowRoot?.querySelector("uui-textarea");
        if (!uui) return;

        await (uui).updateComplete;

        const textarea = uui.shadowRoot?.querySelector("textarea");

        console.log("textarea: ", uui, textarea);

        if (!textarea) return;

        textarea.style.resize = this.#embed ? "vertical" : "none";

    }

    #commit(value) {
        this.value = value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #clear() {
        this.#error = "";
        this.#loading = false;
        window.clearTimeout(this.#debounceTimer);
        this.#requestToken++;
        this.#commit(null);
    }

    async #lookup() {

        const source = this.#source.trim();

        const requestId = ++this.#requestToken;

        // Reset the property value entirely when the user clears the input
        if (!source) {
            this.value = undefined;
            this.#video = null;
            this.#error = null;
            this.#loading = false;
            this.#dispatchChange();
            return;
        }

        this.#error = null;
        this.#loading = true;
        this.requestUpdate();

        // Notifications are disabled as the error is rendered inline by this element - otherwise the user would get
        // a generic "Bad Request" notification on top of the localized message below
        const { data, error } = await tryExecute(
            this,
            umbHttpClient.get({
                url: "/umbraco/management/api/v1/limbo/vimeo/video",
                query: { source },
                security: [{ type: "http", scheme: "bearer" }]
            }),
            { disableNotifications: true }
        );

        // Discard the response if a newer request has been started in the meantime
        if (requestId !== this.#requestToken) return;

        this.#loading = false;

        if (error || !data) {
            // Keep the entered source so the user can correct it, but drop the video details
            this.value = { source };
            this.#video = null;
            this.#error = this.#localizeError(error);
            this.#dispatchChange();
            return;

        }

        this.value = {
            source,
            credentials: data.credentials,
            parameters: data.parameters,
            video: { _data: JSON.stringify(data.video) }
        };

        this.#video = data.video;

        this.#dispatchChange();

    }

    #scheduleLookup(source) {

        window.clearTimeout(this.#debounceTimer);

        // Discard the result of any lookup that is already in flight
        this.#requestToken++;

        const value = source.trim();
        if (!value) {
            this.#clear();
            return;
        }

        this.#error = null;
        this.requestUpdate();

        this.#debounceTimer = window.setTimeout(() => this.#lookup(value), 250);

    }

    #onSourceInput(event) {
        const source = event.target.value ?? "";
        this.#commit({ ...(this.#value ?? {}), source });
        this.#embed = source.indexOf("<") >= 0;
        this.#scheduleLookup(source);
    }

    #onRefresh() {
        window.clearTimeout(this.#debounceTimer);
        const source = this.#value?.source.trim();
        if (!source) {
            this.#clear();
            return;
        }
        this.#lookup(source);
    }

    #onClear() {
        this.#clear();
    }

    #dispatchChange() {
        this.dispatchEvent(new UmbChangeEvent());
    }

    // Translates the error code returned by the server into a message in the language of the current user. The
    // server returns the code via the "operationStatus" extension of the problem details response.
    #localizeError(error) {
        const code = error?.operationStatus ?? error?.problemDetails?.operationStatus ?? error?.body?.operationStatus;
        const key = errorKeys[code];
        return key ? this.localize.term(key) : this.localize.term(errorKeys.getVideoFailed);
    }

    #renderEditor() {

        return html`
            <div class="editor">
                <uui-label for="source">
                    <umb-localize key="limboVimeo_urlOrEmbedCode">URL or embed code</umb-localize>
                </uui-label>
                <textarea
                            id="source"
                            label=${this.localize.term("limboVimeo_urlOrEmbedCode")}
                            .value=${this.#source}
                            class="${this.#embed ? "embed" : "url"}"
                            placeholder=${this.localize.term("limboVimeo_urlPlaceholder")}
                            @input=${this.#onSourceInput}></textarea>


                ${when(this.#error, () => html`
                    <div class="error">
                        <uui-icon name="icon-alert"></uui-icon>
                        ${this.#error}
                    </div>
                `)}
                ${when(this.#video, () => html`
                    <div class="actions">
                        <uui-button
                            look="outline"
                            label=${this.localize.term("limboVimeo_refresh")}
                            ?disabled=${this.readonly}
                            @click=${this.#onRefresh}></uui-button>
                        <uui-button
                            look="outline"
                            color="danger"
                            label=${this.localize.term("limboVimeo_clear")}
                            ?disabled=${this.readonly}
                            @click=${this.#onClear}></uui-button>
                    </div>
                `)}
            </div>
        `;

    }

    #renderDetails() {

        if (!this.#video) return nothing;

        const thumbnail = this.#thumbnail;

        return html`
            <div class="block">
                <h5>${this.localize.term("limboVimeo_video")}</h5>
                <div class="box">
                    <div class="card-row">
                        ${when(thumbnail, () => html`
                            <img class="thumbnail" src=${thumbnail.link} alt=${this.#video.name ?? ""} loading="lazy" />
                        `)}
                        <table>
                            <tr>
                                <th><umb-localize key="limboVimeo_id">ID</umb-localize></th>
                                <td><code>${this.#videoId}</code></td>
                            </tr>
                            <tr>
                                <th><umb-localize key="limboVimeo_title">Title</umb-localize></th>
                                <td>${this.#video.name}</td>
                            </tr>
                            <tr>
                                <th><umb-localize key="limboVimeo_duration">Duration</umb-localize></th>
                                <td><limbo-video-duration .value=${String(this.#video.duration ?? "")}></limbo-video-duration></td>
                            </tr>
                        </table>
                    </div>
                    ${when(this.#video.description, () => html`<div class="description">${this.#video.description}</div>`)}
                </div>
            </div>
        `;

    }

    render() {
        return html`
            <div class="wrapper ${this.#loading ? "loading" : ""}">
                <div>
                    ${this.#renderEditor()}
                    ${this.#renderDetails()}
                </div>
                ${this.#loading ? html`<uui-loader></uui-loader>` : nothing}
            </div>
        `;
    }

    static styles = css`

        :host {
            display: block;
            position: relative;
        }

        .wrapper > div {
            /*display: flex;
            flex-direction: column;
            gap: var(--uui-size-space-3);
            align-items: flex-start;*/
        }

        .loading > div {
            opacity: 0.6;
            pointer-events: none;
        }

        .loading uui-loader {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
        }

        uui-input,
        uui-textarea {
            width: 100%;
        }

        #source {
            width: 100%;
            resize: vertical;
            box-sizing: border-box;
            padding: var(--uui-size-space-2);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface);
            color: var(--uui-color-text);
            font: inherit;
            min-height: 110px;
            &.url {
                resize: none;
                min-height: 35px;
                max-height: 35px;
            }
        }

        uui-textarea.url::part(textarea) {
            resize: none;
            background: red;
        }

        uui-textarea {
            --uui-textarea-min-height: 110px;
        }

        uui-textarea.url {
            --uui-textarea-min-height: 32px;
            --uui-textarea-max-height: 32px;
        }

        .error {
            display: flex;
            align-items: center;
            gap: var(--uui-size-space-2);
            color: var(--uui-color-danger);
        }

        uui-box {
            width: 100%;
        }

        .details {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-4);
        }

        .thumbnail {
            width: 200px;
            height: auto;
            border-radius: var(--uui-border-radius);
        }

        .info {
            flex: 1;
            min-width: 200px;
        }

        table {
            border-collapse: collapse;
        }

        th {
            text-align: left;
            padding-right: var(--uui-size-space-5);
            font-weight: bold;
            vertical-align: top;
        }

        td {
            vertical-align: top;
        }

        .description {
            flex: 1;
            margin-top: var(--uui-size-space-4);
            white-space: pre-wrap;
        }

        h5 {
            margin: 0;
        }

        .editor {
            display: grid;
            gap: var(--uui-size-space-3);
        }

        .actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-3);
        }

        .block {
            margin-top: var(--uui-size-layout-1);
        }

        .box {
            padding: var(--uui-size-space-4);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface-alt);
        }

        .card-row {
            display: flex;
            gap: var(--uui-size-space-4);
            align-items: flex-start;
            flex-wrap: wrap;
        }

    `;

}

customElements.define("limbo-vimeo-video", LimboVimeoVideoElement);

export default LimboVimeoVideoElement;
