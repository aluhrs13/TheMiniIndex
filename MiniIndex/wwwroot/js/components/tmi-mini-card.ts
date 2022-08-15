import { LitElement, html, css } from "lit";
import { customElement } from "lit/decorators.js";
import "@spectrum-web-components/card/sp-card.js";

@customElement("tmi-mini-card")
export class TmiMiniCard extends LitElement {
  static override styles = [
    css`
      h1,
      h2 {
        all: unset;
      }
    `,
  ];
  override render() {
    return html`
      <sp-card variant="quiet" heading="Card Heading" subheading="JPG Photo">
        <img slot="preview" src="https://place.dog/200/300" alt="Demo Image" />
      </sp-card>
    `;
  }
}
