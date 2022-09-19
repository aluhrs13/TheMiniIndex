import "./pages/my-element";
import authService from "./utils/AuthorizeService";
import { perfMark, perfMeasure, logMsg } from "./utils/PerformanceMarks";

if (await authService.isAuthenticated()) {
  logMsg("Authenticated:");
  logMsg(await authService.getUser());
} else {
  logMsg("Not Authenticated...");
  //await authService.signIn("https://localhost:44386/");
}

import { LitElement, css, html } from "lit";
import { customElement } from "lit/decorators.js";
import { Router } from "@vaadin/router";
//import './script/pages/app-home';
//import './script/components/header';
//import './styles/global.css';

@customElement("app-index")
export class AppIndex extends LitElement {
  static get styles() {
    return css`
      main {
        padding-left: 16px;
        padding-right: 16px;
        padding-bottom: 16px;
      }
      #routerOutlet > * {
        width: 100% !important;
      }

      #routerOutlet > .leaving {
        animation: 160ms fadeOut ease-in-out;
      }

      #routerOutlet > .entering {
        animation: 160ms fadeIn linear;
      }

      @keyframes fadeOut {
        from {
          opacity: 1;
        }

        to {
          opacity: 0;
        }
      }

      @keyframes fadeIn {
        from {
          opacity: 0.2;
        }

        to {
          opacity: 1;
        }
      }
    `;
  }

  constructor() {
    super();
  }

  firstUpdated() {
    // this method is a lifecycle even in lit
    // for more info check out the lit docs https://lit.dev/docs/components/lifecycle/

    // For more info on using the @vaadin/router check here https://vaadin.com/router
    const router = new Router(this.shadowRoot?.querySelector("#routerOutlet"));
    router.setRoutes([
      // temporarily cast to any because of a Type bug with the router
      {
        path: (import.meta as any).env.BASE_URL,
        animate: true,
        children: [
          { path: "", component: "my-element" },
          {
            path: "/authentication/login/:action",
            component: "app-login",
            action: async () => {
              await import("./pages/tmi-login.js");
            },
          },
          {
            path: "/authentication/logout/:action",
            component: "app-logout",
            action: async () => {
              await import("./pages/tmi-logout.js");
            },
          },
        ],
      } as any,
    ]);
  }

  render() {
    return html`
      <main>
        <div id="routerOutlet"></div>
      </main>
    `;
  }
}
