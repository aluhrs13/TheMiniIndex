import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { BeforeEnterObserver, Router, RouterLocation } from "@vaadin/router";

import { getMiniDetail, DetailedMini } from "../utils/minis.js";
import "../components/tmi-mini-card.js";

@customElement("tmi-mini-page")
export class TMIMiniPage extends LitElement implements BeforeEnterObserver {
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
  @property() id: string = "";
  @state() data: DetailedMini = {} as DetailedMini;
  @state() loading: boolean = false;
  @state() initLoad: boolean = false;

  constructor() {
    super();
  }

  onBeforeEnter(location: RouterLocation) {
    this.id = location.params.id as string;
  }

  async firstUpdated() {
    this.data = await getMiniDetail(1);
    console.log(this.data);
    this.loading = false;
  }

  override render() {
    return html`<div>
      ${this.loading ? html`<span>Loading</span>` : null}
      ${this.data ? html`${this.data.Name}` : html`<span>Not found</span>`}
    </div>`;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-mini-page": TMIMiniPage;
  }
}
