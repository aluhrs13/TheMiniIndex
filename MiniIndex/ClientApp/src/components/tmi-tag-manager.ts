//3rd Party Imports
import { LitElement, html, css } from "lit";
import { customElement, property, state } from "lit/decorators.js";

//1st Party Imports
import authService from "../utils/AuthorizeService.js";
import { Tag } from "../utils/minis.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";

//Style and Component Imports
import { buttonStyles } from "../styles/button-styles.js";
import { switcherStyles } from "../styles/layout-styles.js";
import { fontStyles } from "../styles/font-styles.js";

@customElement("tmi-tag-manager")
export class TMITagManager extends LitElement {
  static override styles = [fontStyles, switcherStyles, buttonStyles, css``];

  @property() miniId = "";
  @state() _deletedTags: Tag[] = [];
  @state() _pendingTags: Tag[] = [];
  @state() _tagSearchResults: String[] = [];
  //TODO: Make a tag suggestion API and hook this up right.
  @state() _suggestedTags: Tag[] = [];

  async firstUpdated() {
    this._updateTagLists();
  }

  //TODO: Handle errors
  private async _updateTagLists() {
    perfMark("tmi-updateTags-start");
    const response = await fetch(
      "https://localhost:44386/api/Minis/" + this.miniId + "/Tags"
    );
    const data = await response.json();
    this._pendingTags = [];
    this._deletedTags = [];

    data.forEach((tag: Tag) => {
      if (tag.Status === "Pending") {
        this._pendingTags.push(tag);
      } else if (tag.Status === "Deleted" || tag.Status === "Rejected") {
        this._deletedTags.push(tag);
      }
    });

    perfMark("tmi-updateTags-end");
    perfMeasure("tmi-updateTags", "tmi-updateTags-start", "tmi-updateTags-end");
  }

  //TODO: Add a fancy multi-action button for controlling state
  private _printTagList(tagList: Tag[]) {
    if (tagList.length === 0) return "No tags";
    let html = "";
    tagList.forEach((tag: Tag) => {
      html += tag.TagName + " ";
    });
    return html;
  }

  //TODO: Remove any existing tags
  private async _typeahead(e: any) {
    if (e.target.value.length < 2) return;
    perfMark("tmi-searchTags-start");
    const token = await authService.getAccessToken();
    const response = await fetch(
      "https://localhost:44386/api/Tags?search=" + e.target.value,
      {
        headers: !token ? {} : { Authorization: `Bearer ${token}` },
      }
    );
    this._tagSearchResults = await response.json();

    perfMark("tmi-searchTags-end");
    perfMeasure("tmi-searchTags", "tmi-searchTags-start", "tmi-searchTags-end");
  }

  private async _addTag(tag: string) {
    console.log("adding " + tag);
    perfMark("tmi-addTag-start");
    var miniTagData = {
      Mini: {
        ID: this.miniId * 1,
      },
      Tag: {
        TagName: tag,
      },
    };

    const token = await authService.getAccessToken();
    const response = await fetch("https://localhost:44386/api/MiniTags", {
      headers: !token
        ? {}
        : {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
      method: "POST",
      body: JSON.stringify(miniTagData),
    });

    this._updateTagLists();
    perfMark("tmi-addTag-end");
    perfMeasure("tmi-addTag", "tmi-addTag-start", "tmi-addTag-end");
  }

  override render() {
    return html`
      <div>
        <h3>Suggested Tags</h3>
        ${this._printTagList(this._suggestedTags)}
      </div>
      <br />
      <h3>Tag Manually</h3>
      <div class="switcher">
        <div>
          <input
            type="text"
            placeholder="Tag Search"
            @input="${this._typeahead}"
          />
          <br />
          ${this._tagSearchResults.map(
            (tag) =>
              html`<button @click="${() => this._addTag(tag)}">${tag}</button>`
          )}
        </div>

        <div>
          <h4>Deleted Tags</h4>
          ${this._printTagList(this._deletedTags)}
          <h4>Pending Tags</h4>
          ${this._printTagList(this._pendingTags)}
        </div>
      </div>
    `;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "tmi-tag-manager": TMITagManager;
  }
}
