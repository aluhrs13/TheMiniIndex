import { LitElement, html, css } from 'lit';
import { customElement, property } from 'lit/decorators.js';

/**
 * An example element.
 *
 * @fires count-changed - Indicates when the count changes
 * @slot - This element has a slot
 * @csspart button - The button
 */
@customElement('tmi-admin-card')
export class AdminCard extends LitElement {
    static override styles = css`
    :host {
      display: block;
      border: solid 1px gray;
      padding: 16px;
      max-width: 800px;
    }
  `;

    @property()
    name = 'World';

    @property({ type: Number })
    count = 0;

    override render() {
        return html`
      <h1>${this.sayHello(this.name)}!</h1>
      <button @click=${this._onClick} part="button">
        Click Count: ${this.count}
      </button>
      <slot></slot>
    `;
    }

    private _onClick() {
        this.count++;
        this.dispatchEvent(new CustomEvent('count-changed'));
    }

    sayHello(name: string): string {
        return `Hello, ${name}`;
    }
}

declare global {
    interface HTMLElementTagNameMap {
        'tmi-admin-card': AdminCard;
    }
}