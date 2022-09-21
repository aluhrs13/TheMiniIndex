import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";

import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";
import { miniListStyles } from "../styles/layout-styles.js";
import "../components/tmi-mini-card.js";

@customElement("tmi-related-minis")
export class TMIRelatedMinis extends LitElement {
  static override styles = [miniListStyles, css``];

  @property() miniId = 0;
  @state() _data = [];

  async firstUpdated() {
    perfMark("tmi-getRelatedMinis-start");
    const token = await authService.getAccessToken();
    const response = await fetch(`/api/Minis/${this.miniId}/Related`, {
      headers: !token ? {} : { Authorization: `Bearer ${token}` },
    });
    this._data = await response.json();
    perfMark("tmi-getRelatedMinis-end");
    perfMeasure(
      "tmi-getRelatedMinis",
      "tmi-getRelatedMinis-start",
      "tmi-getRelatedMinis-end"
    );
  }

  override render() {
    console.log(this._data);
    return html`<div>
      ${this._loading
        ? html`<span>Loading</span>`
        : this._data.length > 0
        ? html`<ul class="grid" id="gallery">
            ${this._data.map((item) => {
              return html`<li>
                <tmi-mini-card
                  name=${item.Name}
                  status=${item.Status}
                  miniId=${item.ID}
                  thumbnail=${item.Thumbnail}
                ></tmi-mini-card>
              </li>`;
            })}
          </ul>`
        : html`<span>No data</span>`}
    </div>`;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-related-minis": TMIRelatedMinis;
  }
}
