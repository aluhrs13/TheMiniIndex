import { LitElement, html, css } from 'lit';
import { customElement, property } from 'lit/decorators.js';

@customElement('tmi-button')
export class TMIButton extends LitElement {
    static override styles = css`
    :host {
      display: block;
      border: solid 1px gray;
      padding: 16px;
      max-width: 800px;
    }
  `;

    @property({ type: Number })
    count = 0;

    @property({ type: String })
    text="";

    @property({ type: String })
    tmistyle = "allow";

    @property({ type: String })
    method = "GET";

    @property({ type: String })
    url = "";

    @property({ type: String })
    data = "";

    @property({ type: String })
    currentState = ""

    fetchCompleted(data: boolean) {
        this.dispatchEvent(new CustomEvent('fetchCompleted', { detail: data, bubbles: true, composed: true }));
    }

    override render() {
        return html`
      <button onClick=${(event: Event) => this.makeCall(event)} class=${'btn ' + this.tmistyle + ' ' + this.currentState}>
        <slot></slot>
      </button>
    `;
    }

    private makeCall(event: Event): boolean {
        event.preventDefault();
        this.currentState = 'load';
        var slot = this.shadowRoot?.querySelector('slot');


        //TODO: Take in method and use that.
        fetch(this.url, {
            method: this.method, body: this.data, headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                this.fetchCompleted(response.ok);

                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }

                return response.json();
            })
            .then(() => {
                if (slot)
                    slot.innerText = "Success!";
                this.currentState = 'complete';
            })
            .catch(error => {
                console.error(error);
                if (slot)
                    slot.innerText = "Failed - Retry?!";
                this.currentState = 'error';
            });
        return false;
    }
}

declare global {
    interface HTMLElementTagNameMap {
        'tmi-button': TMIButton;
    }
}