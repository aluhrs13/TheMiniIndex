import { Component, Prop, h, Watch, State } from '@stencil/core';

@Component({
  tag: 'tmi-admin-mini-row',
  styleUrl: 'tmi-admin-mini-row.css',
  shadow: true,
})
export class TmiAdminMiniRow {
  @Prop() miniid: string;
  @Prop() thumbnail: string;
  @Prop() name: string;
  @Prop() source: string;
  @Prop() tagdata: string;
  @Prop() type: string;
  @State() tags: string[];
  @State() visible: boolean;

  //Prints out tags in the right styling
  private printTags() {
      return (
        <div class="row">
          {this.tags.map(item => {
            return <p class="tag">{item}</p>;
          })}
        </div>
      );
  }

  componentWillLoad() {
    this.tags = JSON.parse(this.tagdata);
    this.visible = true;
  }

  //If any of the button presses succeed, hide this row.
  buttonCompleted(event) {
    this.visible = !event.detail;
  }

  //TODO: This can be cleaned up a lot.
  printButtons() {
    if (this.type == 'Pending') {
      return (
        <div id="mini-buttons">
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Approve"
            tmistyle="btn approve"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": '+this.miniid+', "status": 1}'}
          ></tmi-do-something-button>
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Deny"
            tmistyle="btn deny"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": '+this.miniid+', "status": 2}'}
          ></tmi-do-something-button>
        </div>
      );
    } else {
      return (
        <div id="mini-buttons">
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Visible"
            tmistyle="btn visible"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": '+this.miniid+', "status": 0}'}
          ></tmi-do-something-button>
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Deny"
            tmistyle="btn deny"
            method="PATCH"
            url={'/api/Minis/'}
            data={'{"id": '+this.miniid+', "status": 2}'}
          ></tmi-do-something-button>
        </div>
      );
    }
  }

  render() {
    if (this.visible) {
      return (
        <div class="mini-row">
          <div>
            <img src={this.thumbnail} id="imgplace" />
          </div>

          {this.printButtons()}
          <div>
            <a href={'/Minis/Edit?id=' + this.miniid}>{this.name}</a>
            <br />
            <small>{this.source}</small>
          </div>
          {this.printTags()}
        </div>
      );
    } else {
      return <span></span>;
    }
  }
}
