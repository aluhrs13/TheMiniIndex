import { Component, Prop, h, State, Event, EventEmitter } from '@stencil/core';

@Component({
  tag: 'tmi-do-something-button',
  styleUrl: 'tmi-do-something-button.css',
  shadow: true,
})
export class TmiDoSomethingButton {
  @Prop() text: string;
  @Prop() tmistyle: string;
  @Prop() method: string;
  @Prop() url: string;
  @State() currentState: string;
  @State() currentText: string;

  componentWillLoad() {
    this.currentText = this.text;
  }

  private makeCall(event): boolean {
    event.preventDefault();
    this.currentState = 'load';

    //TODO: Take in method and use that.
    fetch(this.url)
      .then(response => {
        this.fetchCompleted.emit(response.ok);

        if (!response.ok) {
          throw new Error('Network response was not ok');
        }

        return response.json();
      })
      .then(data => {
        this.currentState = 'complete';
      })
      .catch(error => {
        this.currentText = 'Failed - Retry?';
        this.currentState = 'error';
      });
    return false;
  }

  @Event() fetchCompleted: EventEmitter<boolean>;
  fetchCompletedHandler(data: boolean) {
    this.fetchCompleted.emit(data);
  }

  render() {
    return (
      <button onClick={event => this.makeCall(event)} class={'btn ' + this.tmistyle + ' ' + this.currentState}>
        {this.currentText}
      </button>
    );
  }
}
