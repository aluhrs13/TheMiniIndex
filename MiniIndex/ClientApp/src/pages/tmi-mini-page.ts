//3rd Party Imports
import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { BeforeEnterObserver, RouterLocation } from "@vaadin/router";

//1st Party Imports
import { getMiniDetail, DetailedMini, TagCategory } from "../utils/minis.js";

//Style and Component Imports
import "../components/tmi-action-button.js";
import "../components/tmi-mini-card.js";
//TODO: Can I delay loading some of these?
import "../components/tmi-related-minis.js";
import "../components/tmi-tag-manager.js";
import { buttonStyles } from "../styles/button-styles.js";
import { fontStyles } from "../styles/font-styles.js";
import { switcherStyles, rowStyles } from "../styles/layout-styles.js";

@customElement("tmi-mini-page")
export class TMIMiniPage extends LitElement implements BeforeEnterObserver {
  static override styles = [
    buttonStyles,
    fontStyles,
    switcherStyles,
    rowStyles,
    css`
      :host {
        display: block;
      }

      .badge {
        display: inline-block;
        padding: 0.5rem 0.5rem;
        margin: 0.25rem;
        border-radius: 0.25rem;
      }
    `,
  ];
  @property() miniId: number | null = null;
  @state() _data: DetailedMini | null = null;
  @state() _loading: boolean = true;
  //TODO: Favorites API and populate this
  @state() _isFavorite: boolean | null = null;
  //TODO: Save this between pages and sessions
  @state() _isEditMode: boolean = true;

  onBeforeEnter(location: RouterLocation) {
    this.miniId = location.params.id;
  }

  connectedCallback() {
    super.connectedCallback();
    this._toggleFavorite = this._toggleFavorite.bind(this);
    window.addEventListener("button-complete", this._toggleFavorite);
  }
  disconnectedCallback() {
    window.removeEventListener("button-complete", this._toggleFavorite);
    super.disconnectedCallback();
  }

  async firstUpdated() {
    this._data = await getMiniDetail(this.miniId);
    this._loading = false;
  }

  private _toggleFavorite() {
    this._isFavorite = !this._isFavorite;
  }

  private _toggleEdit() {
    this._isEditMode = !this._isEditMode;
  }

  override render() {
    return html`<div>
      ${this._loading
        ? html`<span>Loading</span>`
        : this._data
        ? html`<div class="bounded">
            <h1>
              ${this._data.Name} by
              <a href="/Creators/Details?id=${this._data.Creator.ID}"
                >${this._data.Creator.Name}</a
              >
            </h1>

            <div class="switcher">
              ${
                //TOOD: Do this.
                this._data.Sources && this._data.Sources.length > 0
                  ? this._data.Sources.map((source) => {
                      return html`<a
                        href="/api/Minis/@Model.Mini.ID/Redirect"
                        class="btn btn-block style-primary @miniSource.Site.SiteName"
                        >View On @miniSource.Site.SiteName</a
                      >`;
                    })
                  : html`<a
                      href="/api/Minis/${this._data.ID}/Redirect"
                      class="btn btn-block style-primary-border"
                      >View at source</a
                    >`
              }

              <tmi-action-button
                block="true"
                url="/api/Starred/${this.id}"
                styleName=${this._isFavorite ? "style-danger" : "style-green"}
                method=${this._isFavorite ? "DELETE" : "POST"}
                authRequired="true"
              >
                ${this._isFavorite
                  ? "Remove from Favorites"
                  : "Add to Favorites"}
              </tmi-action-button>
            </div>

            <div class="switcher">
              <div align="center">
                <img style="max-width:100%" src="${this._data.Thumbnail}" />
              </div>

              <div>
                <div class="row" style="align-items:flex-end;">
                  <h2 style="flex-grow:1;">Tags</h2>
                  <span class="small" style="padding-bottom:1.25rem;">
                    Something look wrong?
                    <button @click="${this._toggleEdit}">Tag this Mini</button>
                  </span>
                </div>
                ${this._data.MiniTags.map((tag) => {
                  return tag.Status == 1
                    ? html` <span class="badge style-primary">
                        ${tag.Tag.Category
                          ? html`<b>${TagCategory[tag.Tag.Category]}:</b>`
                          : ""}
                        ${tag.Tag.TagName}
                      </span>`
                    : null;
                })}
              </div>
            </div>
            <aside>
              <hr />
              ${!this._isEditMode
                ? html`
                    <h2>Related Minis</h2>
                    <tmi-related-minis
                      miniId=${this.miniId}
                    ></tmi-related-minis>
                  `
                : html`<h2>Edit Tags</h2>
                    <tmi-tag-manager miniId=${this.miniId}></tmi-tag-manager>`}
            </aside>
          </div>`
        : html`<span>Not found</span>`}
    </div>`;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-mini-page": TMIMiniPage;
  }
}
