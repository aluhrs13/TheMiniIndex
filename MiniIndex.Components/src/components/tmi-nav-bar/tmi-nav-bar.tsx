import { Component, Prop, h, Listen, State } from '@stencil/core';

@Component({
  tag: 'tmi-nav-bar',
  styleUrl: 'tmi-nav-bar.css',
  shadow: false,
})
export class TmiNavBar {
  /**
   * Current selected page
   */
  @Prop() currentpage: string;

  @State() open: boolean;

  //TODO: Limit this to just ham.
  //TODO: Figure out resize: https://medium.com/stencil-tricks/creating-responsive-components-in-stencil-using-the-resizeobserver-api-8eb9a1f69469
  @Listen('click', { capture: true })
  handleClick() {
    this.open = !this.open;
  }

  render() {
    return (
      <div id="nav-bar">
        <div id="top-bar">
          <div id="logotype">
            <img width="40" height="40" src="https://beta.theminiindex.com/static/media/logo.6e92ad4f.svg" />

            <div>
              <span id="xsmall">THE</span>
              <br />
              <span id="larger">MINI INDEX</span>
            </div>
          </div>
          <div id="hamburger">
            <a href="#">Ham</a>
          </div>
        </div>
        <div id="right-aligned" class={this.open ? 'show' : 'hide'}>
          <a href="">Minis</a>
          <a href="">Creators</a>
          <a href="">Add a Mini</a>
          <a href="">About</a>

          <a href="">Register</a>
          <a href="">Login</a>
        </div>
      </div>
    );
  }
}
