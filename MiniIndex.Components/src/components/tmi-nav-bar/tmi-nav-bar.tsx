import { Component, Prop, h } from '@stencil/core';

@Component({
  tag: 'tmi-nav-bar',
  styleUrl: 'tmi-nav-bar.css',
  shadow: true,
})
export class TmiNavBar {
  /**
   * Current selected page
   */
  @Prop() currentpage: string;

  render() {
    return (
      <div id="nav-bar">
        <img width="40" height="40" src="https://beta.theminiindex.com/static/media/logo.6e92ad4f.svg" />

        <div id="logotype">
          <span id="xsmall">THE</span>
          <br />
          <span id="larger">MINI INDEX</span>
        </div>

        <div id="right-aligned">
          <a href="">Minis</a>
          <a href="">Creators</a>
          <a href="">Send Feedback</a>
        </div>
      </div>
    );
  }
}
