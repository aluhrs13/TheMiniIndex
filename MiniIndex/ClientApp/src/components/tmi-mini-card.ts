//3rd Party Imports
import { LitElement, html, css } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Router } from "@vaadin/router";

//1st Party Imports

//Style and Component Imports
import { fontStyles } from "../styles/font-styles.js";

@customElement("tmi-mini-card")
export class TMIMiniCard extends LitElement {
  static override styles = [
    fontStyles,
    css`
      :host {
        display: block;
      }
      a.card {
        color: var(--app-primary-color);
        height: 100%;
      }

      .card {
        filter: drop-shadow(4px 4px 4px var(--app-primary-color));
        background-color: white;
        border-radius: 2px;
      }

      .card-thumbnail {
        object-fit: cover;
        border-radius: 2px 2px 0px 0px;
        width: 100%;
      }

      .card-text {
        display: flex;
        flex-direction: row;
        align-items: center;
        width: 100%;
      }

      .card-padded {
        padding: 0.5rem;
      }

      .mini-name {
        text-align: left;
        line-height: 0.4em;
        white-space: nowrap;
        overflow: hidden;
        margin: 0 !important;
      }

      .mini-name > h3,
      .mini-name > h4 {
        margin: 0.5rem;
      }

      .new-tag {
        width: 0;
        height: 0;
        border-bottom: 60px solid #00201c;
        border-left: 80px solid transparent;
        position: absolute;
        right: 0px;
        bottom: 0px;
        z-index: 99;
        -webkit-filter: drop-shadow(-2px -2px 4px #40a076);
        filter: drop-shadow(-2px -2px 4px #40a076);
      }

      .new-tag-span {
        position: relative;
        bottom: -29px;
        right: 55px;
        z-index: 100;
        width: 60px;
        text-align: center;
        font-size: 13px;
        font-family: arial;
        transform: rotate(-37deg);
        font-weight: 600;
        color: white;
        display: block;
      }

      .mini-banner {
        background-color: #fff7b9;
        margin-top: -4px;
        padding: 0.5rem;
      }

      .hidden {
        display: none !important;
      }

      .approved-time {
        display: none;
      }
    `,
  ];
  @property() name = "";
  @property() status = "";
  @property() miniId = "";
  @property() thumbnail = "";
  @property() creatorId = "";
  @property() creatorName = "";
  @property() approvedTime = "";

  async view(id: string) {
    Router.go(`/Mini/${id}`);
  }

  //TODO: Banner for needs tags.
  override render() {
    console.log(this);
    return html`
      <div class="card ${this.status}" id="${this.miniId}">
        <div>
          <a @click="${() => this.view(this.miniId)}">
            <img
              class="card-thumbnail"
              src="${this.thumbnail}"
              width="314"
              height="236"
            />
          </a>
        </div>

        <div class="card-text">
          <div class="mini-name">
            <h3>${this.name}</h3>
            ${this.creatorName
              ? html` <h4>
                  by
                  <a
                    style="color:var(--app-primary-color)"
                    href="/Creators/Details/?id=${this.creatorId}"
                    >${this.creatorName}</a
                  >
                </h4>`
              : ""}
          </div>
        </div>
        <div class="new-tag hidden"><span class="new-tag-span">New!</span></div>
        <div class="approved-time">${this.approvedTime}</div>
      </div>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-mini-card": TMIMiniCard;
  }
}
