import { LitElement, html, css } from "lit";
import { customElement, state } from "lit/decorators.js";

import { buttonStyles } from "../utils/button-styles.js";
import { logMsg } from "../utils/PerformanceMarks.js";
import authService from "../utils/AuthorizeService";

@customElement("tmi-auth-cluster")
export class TMIAuthCluster extends LitElement {
  static styles = [
    buttonStyles,
    css`
      :host {
        border: solid 1px red;
      }
    `,
  ];

  @state()
  authed = false;

  _logout() {
    authService.signOut();
  }

  _login() {
    authService.signIn();
  }

  firstUpdated() {
    authService.isAuthenticated().then((result: boolean) => {
      this.authed = result;
    });
  }

  //Logged In: Profile, Moderator
  //Logged Out: Register, Login
  override render() {
    if (this.authed) {
      return html` <div class="center">
        <a @click=${this._logout} class="btn style-primary">Logout</a>
      </div>`;
    } else {
      return html` <div class="center">
        <a @click=${this._login} class="btn style-primary">Login</a>
      </div>`;
    }
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-auth-cluster": TMIAuthCluster;
  }
}
