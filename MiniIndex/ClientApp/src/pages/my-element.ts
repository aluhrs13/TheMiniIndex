//3rd Party Imports
import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";

//1st Party Imports
import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";

//Style and Component Imports

@customElement("my-element")
export class MyElement extends LitElement {
  static override styles = [
    css`
      :host {
        border: solid 1px red;
      }
    `,
  ];

  @property() name = "";
  @state() _data = "";

  async _getData() {
    perfMark("tmi-genericGetData-start");
    const token = await authService.getAccessToken();
    const response = await fetch("https://localhost:44386/api/minis", {
      headers: !token ? {} : { Authorization: `Bearer ${token}` },
    });
    this._data = await response.json();
    perfMark("tmi-genericGetData-end");
    perfMeasure(
      "tmi-genericGetData",
      "tmi-genericGetData-start",
      "tmi-genericGetData-end"
    );
  }

  override render() {
    return html`
      <h1>Hello, ${this.name}!</h1>
      <button @click=${this._getData} part="button">Get Data!</button>
      <br />
      <slot>${this._data}</slot>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "my-element": MyElement;
  }
}
