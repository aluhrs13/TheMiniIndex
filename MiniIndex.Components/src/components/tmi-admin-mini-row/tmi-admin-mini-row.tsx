import { Component, Prop, h, Watch, State } from '@stencil/core';

@Component({
  tag: 'tmi-admin-mini-row',
  styleUrl: 'tmi-admin-mini-row.css',
  shadow: true,
})
export class TmiAdminMiniRow {
  @Prop() miniid: string;
  @Prop() name: string;
  @Prop() source: string;
  @Prop() tagdata: string;
  @Prop() type: string;
  @State() tags: string[];
  @State() visible: boolean;

  //Prints out tags in the right styling
  private printTags() {
    if (this.type == 'pending') {
      return (
        <div class="row">
          {this.tags.map(item => {
            return <p class="tag">{item}</p>;
          })}
        </div>
      );
    } else {
      return (
        <div class="column">
          {this.tags.map(item => {
            return (
              <div>
                <tmi-do-something-button
                  onFetchCompleted={ev => this.buttonCompleted(ev)}
                  text={'Tag as ' + item + ' and Approve'}
                  tmistyle="start"
                  method="PATCH"
                  url={'/api/minis/' + this.miniid}
                ></tmi-do-something-button>
              </div>
            );
          })}
        </div>
      );
    }
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
    if (this.type == 'pending') {
      return (
        <div id="mini-buttons">
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Approve"
            tmistyle="btn approve"
            url="/api/minis/search?pageIndex=1&SearchString=bard"
          ></tmi-do-something-button>
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Deny"
            tmistyle="btn deny"
            url="/api/minis/search?pageIndex=1&SearchString=bard"
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
            url="/api/minis/search?pageIndex=1&SearchString=bard"
          ></tmi-do-something-button>
          <tmi-do-something-button
            onFetchCompleted={ev => this.buttonCompleted(ev)}
            text="Deny"
            tmistyle="btn deny"
            method="PATCH"
            url="/api/minis/search?pageIndex=1&SearchString=bard"
          ></tmi-do-something-button>
        </div>
      );
    }
  }

  //TODO: Fix img URL
  //TODO: Fix API URLs
  render() {
    if (this.visible) {
      return (
        <div class="mini-row">
          <div>
            <img src="" id="imgplace" />
          </div>
          {this.printButtons()}
          <div>
            <a href={'/minis/id=' + this.miniid}>{this.name}</a>
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
