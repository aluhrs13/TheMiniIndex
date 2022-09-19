import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";

import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";

@customElement("my-element")
export class MyElement extends LitElement {
  static override styles = css`
    :host {
      display: block;
      border: solid 1px gray;
      padding: 16px;
      max-width: 800px;
      color: black;
    }
  `;

  @state()
  name = "World";

  @state()
  data = "";

  override render() {
    return html`
      <h1>Hello, ${this.name}!</h1>
      <button @click=${this._getData} part="button">Get Data!</button>
      <br />
      <slot>${this.data}</slot>
    `;
  }

  async _getData() {
    perfMark("tmi-genericGetData-start");
    const token = await authService.getAccessToken();
    const response = await fetch("https://localhost:44386/api/minis", {
      headers: !token ? {} : { Authorization: `Bearer ${token}` },
    });
    this.data = await response.json();
    perfMark("tmi-genericGetData-end");
    perfMeasure(
      "tmi-genericGetData",
      "tmi-genericGetData-start",
      "tmi-genericGetData-end"
    );
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "my-element": MyElement;
  }
}
