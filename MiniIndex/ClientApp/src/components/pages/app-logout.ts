import { BeforeEnterObserver, RouterLocation } from '@vaadin/router';
import { LitElement, html } from 'lit';
import { customElement, state } from 'lit/decorators.js';

import authService from '../api-authorization/AuthorizeService.js';
import { AuthenticationResultStatus } from '../api-authorization/AuthorizeService.js';

// The main responsibility of this component is to handle the user's logout process.
// This is the starting point for the logout process, which is usually initiated when a
// user clicks on the logout button on the LoginMenu component.
@customElement('app-logout')
export class AppLogout extends LitElement implements BeforeEnterObserver {
    @state()
    message = "";

    @state()
    isReady = false;

    @state()
    authenticated = false;

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
            case "logout":
                this.logout(this.getReturnUrl());

                break;
            case "logout-callback":
                this.processLogoutCallback();
                break;
            case "logged-out":
                //TODO: This message never shows?
                this.message= "You successfully logged out!";
                this.isReady= true;
                break;
            default:
                throw new Error(`Invalid action '${action}'`);
        }

        this.populateAuthenticationState();
    }

    render() {
        if (!this.isReady) {
            return html`<div></div>`;

    } else {
            switch (this.action) {
                case "logout":
                    return (html`<div>Processing logout</div>`);
                case "logout-callback":
                    return (html`<div>Processing logout callback</div>`);
                case "logged-out":
                    return (html`<div>${this.message}</div>`);
        default:
                    throw new Error(`Invalid action '${this.action}'`);
            }
        }
    }

    async logout(returnUrl: string) {
        this.returnUrl = returnUrl;
        const isauthenticated = await authService.isAuthenticated();
        if (isauthenticated) {
            const result = await authService.signOut(state);
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
                    throw new Error("Invalid authentication result status.");
            }
        } else {
            this.message = "You successfully logged out!";
        }
    }

    async processLogoutCallback() {
        const url = window.location.href;
        const result = await authService.completeSignOut(url);
        switch (result.status) {
            case AuthenticationResultStatus.Redirect:
                // There should not be any redirects as the only time completeAuthentication finishes
                // is when we are doing a redirect sign in flow.
                throw new Error('Should not redirect.');
            case AuthenticationResultStatus.Success:
                await this.navigateToReturnUrl(this.getReturnUrl());
                break;
            case AuthenticationResultStatus.Fail:
                this.message = result.message;
                break;
            default:
                throw new Error("Invalid authentication result status.");
        }
    }

    async populateAuthenticationState() {
        this.authenticated = await authService.isAuthenticated();
        this.isReady = true;
    }

    getReturnUrl() {
        const params = new URLSearchParams(window.location.search);
        const fromQuery = params.get("returnUrl");
        if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
            // This is an extra check to prevent open redirects.
            throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
        }
        return (this.returnUrl) ||
            fromQuery ||
            `${window.location.origin}/authentication/logout/logged-out`;
    }

    navigateToReturnUrl(returnUrl:string) {
        return window.location.replace(returnUrl);
    }
}

declare global {
    interface HTMLElementTagNameMap {
        'app-logout': AppLogout;
    }
}