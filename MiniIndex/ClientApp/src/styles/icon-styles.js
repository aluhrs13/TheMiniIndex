import { css } from "lit";

//TODO: Refactor into components
export const iconStyles = css`
  .icon-container {
    border-style: solid;
    border-width: 2px;
    border-color: var(--app-secondary-color);
    border-radius: 4px;
    padding: 5px;
  }

  .icon {
    filter: invert(97%) sepia(7%) saturate(488%) hue-rotate(108deg)
      brightness(91%) contrast(96%);
    height: 28px;
    width: 28px;
  }

  .icon-heart {
    background-image: url("../images/IconFont/heart.svg");
  }

  .icon-menu {
    background-image: url("../images/IconFont/bars.svg");
  }

  .icon-random {
    background-image: url("../images/IconFont/random.svg");
  }

  .icon-profile {
    background-image: url("../images/IconFont/user-circle.svg");
  }
`;
