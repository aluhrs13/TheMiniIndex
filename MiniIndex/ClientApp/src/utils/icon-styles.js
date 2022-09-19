import { css } from "lit";

export const iconStyles = css`
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
