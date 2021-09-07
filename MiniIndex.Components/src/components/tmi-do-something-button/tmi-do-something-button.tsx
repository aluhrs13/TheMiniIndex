import { Component, Prop, h, State, Event, EventEmitter } from '@stencil/core';

@Component({
  tag: 'tmi-do-something-button',
  styleUrl: 'tmi-do-something-button.css',
  shadow: true,
})
export class TmiDoSomethingButton {
  @Prop() text: string;
  @Prop() tmistyle: string;
  @Prop() url: string;
  @State() currentState: string;
  @State() currentText: string;

  componentWillLoad() {
    this.currentText = this.text;
  }

  private makeCall(): string {
    this.currentState = 'load';

    fetch(this.url)
      .then(response => {
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        return response.json();
      })
      .then(data => {
        console.log(data);
        this.fetchCompleted.emit(this.url + ' succeeded');
        this.currentState = 'complete';
      })
      .catch(error => {
        console.error('Error in call:', error);
        this.fetchCompleted.emit(this.url + ' failed');
        this.currentText = 'Failed - Retry';
        this.currentState = 'error';
      });
    return '';
  }

  @Event() fetchCompleted: EventEmitter<string>;
  fetchCompletedHandler(data: string) {
    this.fetchCompleted.emit(data);
  }

  render() {
    return (
      <button onClick={() => this.makeCall()} class={this.tmistyle + ' ' + this.currentState}>
        {this.currentText}
      </button>
    );
  }
}
