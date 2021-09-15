import { Component, Prop, h } from '@stencil/core';

@Component({
  tag: 'tmi-admin-card',
  styleUrl: 'tmi-admin-card.css',
  shadow: true,
})
export class TmiAdminCard {
  @Prop() miniid: string;
  @Prop() submitter: string;
  @Prop() status: string;
  @Prop() cost: string;
  @Prop() thumbnail: string;

  //If any of the button presses succeed, hide this row.
  buttonCompleted(event) {
    window.location.reload();
  }

  render() {
    return (
      <div class="mini-card">
        <b>Submitted by: </b> {this.submitter} <br />
        <b>Status: </b> {this.status} <br />
        <b>Cost: </b> {this.cost} <br />
        <b>Thumbnail URL: </b> {this.thumbnail} <br />
        <hr />
        <div class="row">
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Approve"
            tmistyle="btn approve"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": ' + this.miniid + ', "status": 1}'}
          ></tmi-do-something-button>
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Visible"
            tmistyle="btn visible"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": ' + this.miniid + ', "status": 0}'}
          ></tmi-do-something-button>
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Reject"
            tmistyle="btn deny"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": ' + this.miniid + ', "status": 2}'}
          ></tmi-do-something-button>
        </div>
      </div>
    );
  }
}
