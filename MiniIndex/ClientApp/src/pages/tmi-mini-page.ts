import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { BeforeEnterObserver, Router, RouterLocation } from "@vaadin/router";

import { getMiniDetail, DetailedMini, TagCategory } from "../utils/minis.js";
import { buttonStyles } from "../styles/button-styles.js";
import { switcherStyles, rowStyles } from "../styles/layout-styles.js";
import { fontStyles } from "../styles/font-styles.js";
import "../components/tmi-mini-card.js";

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
  @property() id: string = "";
  @state() mini: DetailedMini = null;
  @state() loading: boolean = false;
  @state() initLoad: boolean = false;

  constructor() {
    super();
  }

  onBeforeEnter(location: RouterLocation) {
    this.id = location.params.id as string;
  }

  async firstUpdated() {
    this.mini = await getMiniDetail(this.id);
    console.log(this.mini);
    this.loading = false;
  }

  override render() {
    return html`<div>
      ${this.loading
        ? html`<span>Loading</span>`
        : this.mini
        ? html`<div class="bounded">
            <h1>
              ${this.mini.Name} by
              <a href="/Creators/Details?id=${this.mini.Creator.ID}"
                >${this.mini.Creator.Name}</a
              >
            </h1>

            <div class="switcher">
              ${
                //TOOD: Do this.
                this.mini.Sources && this.mini.Sources.length > 0
                  ? this.mini.Sources.map((source) => {
                      return html`<a
                        href="/api/Minis/@Model.Mini.ID/Redirect"
                        class="btn btn-block style-primary @miniSource.Site.SiteName"
                        >View On @miniSource.Site.SiteName</a
                      >`;
                    })
                  : html`<a
                      href="/api/Minis/${this.mini.ID}/Redirect"
                      class="btn btn-block style-primary-border"
                      >View at source</a
                    >`
              }
              <a href="#" class="btn btn-block add-star" id="toggle-star">
                Add to Favorites
              </a>
            </div>

            <div class="switcher">
              <div align="center">
                <img style="max-width:100%" src="${this.mini.Thumbnail}" />
              </div>

              <div>
                <div class="row" style="align-items:flex-end;">
                  <h2 style="flex-grow:1;">Tags</h2>
                  <span class="small" style="padding-bottom:1.25rem;">
                    Something look wrong?
                    <a href="#">Tag this Mini</a>
                  </span>
                </div>
                ${this.mini.MiniTags.map((tag) => {
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
              <h2>Related Minis</h2>
              <div align="center">
                <div id="loading-spinner" style="width:64px;height:64px;"></div>
              </div>
              <div class="grid hidden" id="related-minis"></div>
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
