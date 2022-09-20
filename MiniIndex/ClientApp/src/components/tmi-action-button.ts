import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";

import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure, logError } from "../utils/PerformanceMarks";
import { buttonStyles } from "../styles/button-styles.js";

@customElement("tmi-action-button")
export class TMIActionButton extends LitElement {
  static override styles = [buttonStyles, css``];

  @property() url = "";
  @property() styleName = "btn-primary";
  @property() method = "GET";
  @property() authRequired = false;
  @property() block = false;
  @property() text = "";
  @state() _loading = false;
  @state() _data = "";
  @state() _error = "";

  async _getData() {
    this._loading = true;
    perfMark("tmi-genericGetData-start");
    const event = new Event("button-complete", {
      bubbles: true,
      composed: true,
    });
    this.dispatchEvent(event);
    const token = this.authRequired ? await authService.getAccessToken() : null;
    const response = await fetch(this.url, {
      headers: !token ? {} : { Authorization: `Bearer ${token}` },
      method: this.method,
    });

    if (response.ok) {
      this._data = await response.json();
      this._error = "";
    } else {
      logError("Failed to set Favorite: " + response.status);
      this._error = "An Error Occurred :(";
    }

    perfMark("tmi-genericGetData-end");
    perfMeasure(
      "tmi-genericGetData",
      "tmi-genericGetData-start",
      "tmi-genericGetData-end"
    );
    this._loading = false;
    //TODO: Fire an event
    //TODO: Actual loading state, disable button during it
  }

  override render() {
    return html`
      <button
        @click=${this._getData}
        ${this._loading ? "disabled" : ""}
        class="btn ${this.styleName}
         ${this.block ? "btn-block" : ""}
         "
      >
        ${this._loading
          ? "Loading"
          : this._error
          ? this._error
          : html`<slot></slot>`}
      </button>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-action-button": TMIActionButton;
  }
}
