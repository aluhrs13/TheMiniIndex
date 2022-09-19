import { BeforeEnterObserver, RouterLocation } from "@vaadin/router";
import { LitElement, html } from "lit";
import { customElement, state } from "lit/decorators.js";

import authService from "../utils/AuthorizeService.js";
import { AuthenticationResultStatus } from "../utils/AuthorizeService.js";

// The main responsibility of this component is to handle the user's login process.
// This is the starting point for the login process. Any component that needs to authenticate
// a user can simply perform a redirect to this component with a returnUrl query parameter and
// let the component perform the login and return back to the return url.
//TODO: Rename class and element name
@customElement("app-login")
export class AppLogin extends LitElement implements BeforeEnterObserver {
  @state()
  message = "";

  @state()
  returnUrl = "";

  @state()
  action = "";

  onBeforeEnter(location: RouterLocation) {
    this.action = location.params.action as string;
  }

  connectedCallback() {
    const action = this.action;
    switch (action) {
      case "login":
        this.login(this.getReturnUrl());
        break;
      case "login-callback":
        this.processLoginCallback();
        break;
      case "login-failed":
        const params = new URLSearchParams(window.location.search);
        const error = params.get("message");
        if (error) {
          this.message = error;
        }
        break;
      case "profile":
        this.redirectToProfile();
        break;
      case "register":
        this.redirectToRegister();
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }

  render() {
    const action = this.action;
    const message = this.message;

    if (!!message) {
      return html`<div>${message}</div>`;
    } else {
      switch (action) {
        case "login":
          return html`<div>Processing login</div>`;
        case "login-callback":
          return html`<div>Processing login callback</div>`;
        case "profile":
        case "register":
          return html`<div></div>`;
        default:
          throw new Error(`Invalid action '${action}'`);
      }
    }
  }

  async login(returnUrl: string) {
    this.returnUrl = returnUrl;

    const result = await authService.signIn(state);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        break;
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(returnUrl);
        break;
      case AuthenticationResultStatus.Fail:
        this.message = result.message;
        break;
      default:
        throw new Error(`Invalid status result ${result.status}.`);
    }
  }

  async processLoginCallback() {
    const url = window.location.href;
    const result = await authService.completeSignIn(url);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as the only time completeSignIn finishes
        // is when we are doing a redirect sign in flow.
        throw new Error("Should not redirect.");
      case AuthenticationResultStatus.Success:
        await this.navigateToReturnUrl(this.getReturnUrl());
        break;
      case AuthenticationResultStatus.Fail:
        this.message = result.message;
        break;
      default:
        throw new Error(
          `Invalid authentication result status '${result.status}'.`
        );
    }
  }

  getReturnUrl() {
    const params = new URLSearchParams(window.location.search);
    const fromQuery = params.get("returnUrl");
    if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
      // This is an extra check to prevent open redirects.
      throw new Error(
        "Invalid return url. The return url needs to have the same origin as the current page."
      );
    }
    return this.returnUrl || fromQuery || window.location.origin;
  }

  redirectToRegister() {
    this.redirectToApiAuthorizationPath(
      `Identity/Account/Register?$returnUrl=${encodeURI(this.returnUrl)}`
    );
  }

  redirectToProfile() {
    this.redirectToApiAuthorizationPath("Identity/Account/Manage");
  }

  redirectToApiAuthorizationPath(apiAuthorizationPath: string) {
    const redirectUrl = `${window.location.origin}/${apiAuthorizationPath}`;
    window.location.replace(redirectUrl);
  }

  navigateToReturnUrl(returnUrl: string) {
    window.location.replace(returnUrl);
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "app-login": AppLogin;
  }
}
