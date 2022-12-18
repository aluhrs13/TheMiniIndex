//3rd Party Imports
import { LitElement, html, css } from "lit";
import { customElement, state } from "lit/decorators.js";

//1st Party Imports
import authService from "../utils/AuthorizeService";

//Style and Component Imports
import { buttonStyles } from "../styles/button-styles.js";

//TODO: Implement most of this, figure out FOUC
@customElement("tmi-auth-cluster")
export class TMIAuthCluster extends LitElement {
  static styles = [buttonStyles, css``];

  @state() _authed = false;

  _logout() {
    authService.signOut();
  }

  _login() {
    authService.signIn();
  }

  firstUpdated() {
    authService.isAuthenticated().then((result: boolean) => {
      this._authed = result;
    });
  }

  //TODO: Logged In: Profile, Moderator
  //TODO: Logged Out: Register, Login
  override render() {
    if (this._authed) {
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
