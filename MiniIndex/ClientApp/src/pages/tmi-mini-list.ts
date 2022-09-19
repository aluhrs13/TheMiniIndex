import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";

import { getMinis, Mini } from "../utils/minis.js";
import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";
import "../components/tmi-mini-card.js";

@customElement("tmi-mini-list")
export class TMIMiniList extends LitElement {
  static override styles = css`
    :host {
      display: block;
    }

    ul {
      list-style-type: none;
      padding: 0;
      margin: 0;
    }

    li {
      text-dcoration: none;
    }

    .grid {
      display: grid;
      grid-gap: 1rem;
    }

    @supports (width: min(250px, 100%)) {
      .grid {
        grid-template-columns: repeat(auto-fit, minmax(min(250px, 100%), 1fr));
      }
    }
  `;

  @state() data: Mini[] = [];
  @state() loading: boolean = false;
  @state() initLoad: boolean = false;

  constructor() {
    super();
  }

  async firstUpdated() {
    this.data = await getMinis();
    this.loading = false;
  }

  override render() {
    return html`<div>
      ${this.loading ? html`<span>Loading</span>` : null}
      ${this.initLoad === false
        ? html`<ul class="grid" id="gallery">
            ${this.data.map((item) => {
              return html`<li>
                <tmi-mini-card
                  name=${item.name}
                  status=${item.status}
                  miniId=${item.id}
                  thumbnail=${item.thumbnail}
                  creatorId=${item.creator.id}
                  creatorName=${item.creator.name}
                  approvedTime=${item.linuxTime}
                ></tmi-mini-card>
              </li>`;
            })}
          </ul>`
        : this.initLoad && this.data && this.data.length <= 0
        ? html`No Data`
        : html`???`}
    </div>`;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-mini-list": TMIMiniList;
  }
}
