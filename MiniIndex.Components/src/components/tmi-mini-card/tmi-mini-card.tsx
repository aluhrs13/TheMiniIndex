import { Component, Prop, h } from '@stencil/core';
import { fixCDN } from '../../utils/utils';

@Component({
  tag: 'tmi-mini-card',
  styleUrl: 'tmi-mini-card.css',
  shadow: true,
})
export class TmiMiniCard {
  /**
   * Id of the Mini
   */
  @Prop() miniid: number;

  /**
   * Name of the Mini
   */
  @Prop() name: string;

  /**
   * Approval status of the Mini
   */
  @Prop() status: number;

  /**
   * Creator Details
   */
  @Prop() creatorname: string;

  /**
   * Creator Details
   */
  @Prop() creatorid: number;

  /**
   * Source site
   */
  @Prop() sourcesite: string;

  /**
   * Thumbnail URL
   */
  @Prop() thumbnail: string;

  render() {
    return (
      <div class="card">
        <div>
          <a href={`/Minis/Details?id=${this.miniid}`}>
            <img class="card-thumbnail" src={fixCDN(this.thumbnail)} width="314" height="236" />
          </a>
        </div>
        <div class="card-text">
          <img class="source-site-img" src={`https://miniindex.blob.core.windows.net/react-images/${this.sourcesite}.png`} />

          <div class="mini-name">
            <h1>{this.name}</h1>
            <h2>
              by <a href={`/Creators/Details?id=${this.creatorid}`}>{this.creatorname}</a>
            </h2>
          </div>
        </div>
      </div>
    );
  }
}
