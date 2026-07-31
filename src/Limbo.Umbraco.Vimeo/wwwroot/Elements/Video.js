// [CHANGE: Umbraco 17 upgrade - AngularJS controller + view replaced by a Lit based property editor UI]
// Related: ButtonList.js, Color.js, Manifests/VimeoPackageManifestReader.cs, Controllers/VimeoController.cs

import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, nothing } from "@umbraco-cms/backoffice/external/lit";
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
        _loading: { state: true },
        _error: { state: true },
        _video: { state: true },
        _embed: { state: true }
    };

    constructor() {
        super();
        this.value = undefined;
        this._loading = false;
        this._error = null;
        this._video = null;
        this._embed = false;
    }

    // The configuration of the data type. Not used directly by the UI, but the setter is needed so Umbraco
    // doesn't complain about the property not being supported.
    set config(value) {
        this._config = value;
    }

    get config() {
        return this._config;
    }

    // Gets the source (URL or embed code) of the current value
    get #source() {
        return this.value?.source ?? "";
    }

    connectedCallback() {
        super.connectedCallback();
        this.#parseValue();
        // Whether to render a textarea rather than an input is decided when the value is loaded and when the user
        // commits a change - not while typing, as swapping the element mid-typing would lose focus and caret
        this._embed = this.#source.indexOf("<") >= 0;
    }

    // [CHANGE: code review - "_embed" was only derived in "connectedCallback", so a value pushed into an already
    // connected element (eg. when switching variant, or when the value arrives after the element was created) kept
    // rendering a stored embed code in the single line input] Related: Color.js, ButtonList.js
    updated(changed) {
        super.updated(changed);
        if (!changed.has("value")) return;
        this.#parseValue();
        // Only re-evaluate when the new source didn't come from this element, as swapping the element while the user
        // is typing would lose focus and caret
        if (this.#source !== this.#typedSource) this._embed = this.#source.indexOf("<") >= 0;
    }

    // Reads the raw Vimeo video data from the "_data" property of the property value. The data is stored as a
    // serialized string as Umbraco would otherwise mangle the timestamps within the JSON returned by the Vimeo API.
    #parseValue() {
        const data = this.value?.video?._data;
        if (!data) {
            this._video = null;
            return;
        }
        try {
            this._video = typeof data === "string" ? JSON.parse(data) : data;
        } catch {
            this._video = null;
        }
    }

    // The source as last entered by the user, so "updated" can tell an outside change from the user typing
    #typedSource;

    // Incremented for each request so responses arriving out of order can be discarded
    #requestId = 0;

    #onInput(e) {
        const source = e.target.value ?? "";
        this.#typedSource = source;
        this.value = { ...(this.value ?? {}), source };
        // Committed right away so the entered source isn't lost if the user saves before the request completes
        this.#dispatchChange();
    }

    #onChange(e) {
        this.#onInput(e);
        this._embed = this.#source.indexOf("<") >= 0;
        this.#getVideo();
    }

    #dispatchChange() {
        this.dispatchEvent(new UmbChangeEvent());
    }

    // Fetches information about the video of the entered URL/embed code from our own management API endpoint
    async #getVideo() {

        const source = this.#source.trim();

        // [CHANGE: code review - without this guard a slow response for an earlier source could land after a newer
        // one and silently revert the source entered by the user] Related: Color.js, ButtonList.js
        const requestId = ++this.#requestId;

        this._error = null;

        // Reset the property value entirely when the user clears the input
        if (!source) {
            this.value = undefined;
            this._video = null;
            this.#dispatchChange();
            return;
        }

        this._loading = true;

        // Notifications are disabled as the error is rendered inline by this element - otherwise the user would get
        // a generic "Bad Request" notification on top of the localized message below
        const { data, error } = await tryExecute(
            this,
            umbHttpClient.get({
                url: "/umbraco/management/api/v1/vimeo/video",
                query: { source },
                security: [{ type: "http", scheme: "bearer" }]
            }),
            { disableNotifications: true }
        );

        // Discard the response if a newer request has been started in the meantime
        if (requestId !== this.#requestId) return;

        this._loading = false;

        if (error || !data) {

            // Keep the entered source so the user can correct it, but drop the video details
            this.value = { source };
            this._video = null;
            this._error = this.#localizeError(error);
            this.#dispatchChange();

            return;

        }

        this.value = {
            source,
            credentials: data.credentials,
            parameters: data.parameters,
            // Serialized on purpose - see the comment in #parseValue
            video: { _data: JSON.stringify(data.video) }
        };

        this._video = data.video;

        this.#dispatchChange();

    }

    // Translates the error code returned by the server into a message in the language of the current user. The
    // server returns the code via the "operationStatus" extension of the problem details response.
    #localizeError(error) {
        const code = error?.operationStatus ?? error?.problemDetails?.operationStatus ?? error?.body?.operationStatus;
        const key = errorKeys[code];
        return key ? this.localize.term(key) : this.localize.term(errorKeys.getVideoFailed);
    }

    get #videoId() {
        // The "uri" of a video is in the format "/videos/1234" or "/videos/1234:hash"
        return this._video?.uri?.split("/")[2]?.split(":")[0] ?? null;
    }

    get #thumbnail() {
        const sizes = this._video?.pictures?.sizes;
        if (!sizes?.length) return null;
        return sizes.find((x) => x.width >= 200) ?? sizes[sizes.length - 1];
    }

    #renderDetails() {

        if (!this._video) return nothing;

        const thumbnail = this.#thumbnail;

        return html`
            <uui-box headline=${this.localize.term("limboVimeo_video")}>
                <div class="details">
                    ${thumbnail
                        ? html`<img class="thumbnail" src=${thumbnail.link} alt=${this._video.name ?? ""} loading="lazy" />`
                        : nothing}
                    <div class="info">
                        <table>
                            <tr>
                                <th><umb-localize key="limboVimeo_id">ID</umb-localize></th>
                                <td><code>${this.#videoId}</code></td>
                            </tr>
                            <tr>
                                <th><umb-localize key="limboVimeo_title">Title</umb-localize></th>
                                <td>${this._video.name}</td>
                            </tr>
                            <tr>
                                <th><umb-localize key="limboVimeo_duration">Duration</umb-localize></th>
                                <td><limbo-video-duration .value=${String(this._video.duration ?? "")}></limbo-video-duration></td>
                            </tr>
                        </table>
                        ${this._video.description
                            ? html`<div class="description">${this._video.description}</div>`
                            : nothing}
                    </div>
                </div>
            </uui-box>
        `;

    }

    render() {
        return html`
            <div class="wrapper">

                <uui-label for="source"><umb-localize key="limboVimeo_urlOrEmbedCode">URL or embed code</umb-localize></uui-label>

                ${this._error
                    ? html`<div class="error"><uui-icon name="icon-alert"></uui-icon> ${this._error}</div>`
                    : nothing}

                ${this._embed
                    ? html`<uui-textarea
                            id="source"
                            label=${this.localize.term("limboVimeo_urlOrEmbedCode")}
                            .value=${this.#source}
                            rows="5"
                            placeholder=${this.localize.term("limboVimeo_urlPlaceholder")}
                            @input=${this.#onInput}
                            @change=${this.#onChange}></uui-textarea>`
                    : html`<uui-input
                            id="source"
                            label=${this.localize.term("limboVimeo_urlOrEmbedCode")}
                            .value=${this.#source}
                            placeholder=${this.localize.term("limboVimeo_urlPlaceholder")}
                            @input=${this.#onInput}
                            @change=${this.#onChange}></uui-input>`}

                ${this._video
                    ? html`<uui-button
                            look="secondary"
                            compact
                            label=${this.localize.term("limboVimeo_refresh")}
                            @click=${this.#getVideo}></uui-button>`
                    : nothing}

                ${this.#renderDetails()}

                ${this._loading ? html`<uui-loader-bar></uui-loader-bar>` : nothing}

            </div>
        `;
    }

    static styles = css`
        :host {
            display: block;
        }

        .wrapper {
            display: flex;
            flex-direction: column;
            gap: var(--uui-size-space-3);
            align-items: flex-start;
        }

        uui-input,
        uui-textarea {
            width: 100%;
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
            margin-top: var(--uui-size-space-4);
            white-space: pre-wrap;
        }
    `;

}

customElements.define("limbo-vimeo-video", LimboVimeoVideoElement);

export default LimboVimeoVideoElement;
