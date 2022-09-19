import { css } from "lit";

export const buttonStyles = css`
  .style-primary-border,
  .btn-primary {
    color: #fff;
    background-color: var(--app-primary-color);
    border-color: var(--app-secondary-color);
  }

  .style-secondary-border,
  .btn-secondary {
    color: #000;
    background-color: var(--app-secondary-color);
    border-color: var(--app-primary-color);
  }

  .style-primary {
    color: #fff;
    background-color: var(--app-primary-color);
  }

  .style-secondary {
    color: #000;
    background-color: var(--app-secondary-color);
  }

  .style-danger {
    background-color: darkred !important;
    color: white;
  }

  .style-moderator {
    background-color: #fff7b9 !important;
  }

  .style-dull {
    background-color: gray !important;
    color: black !important;
  }

  .style-green {
    background-color: darkgreen !important;
    color: white !important;
  }

  /* Buttons */
  .btn {
    font-weight: bolder;
    display: inline-block;
    text-align: center;
    white-space: nowrap;
    vertical-align: middle;
    user-select: none;
    border: 1px solid transparent;
    padding: 0.5rem !important;
    border-radius: 0.25rem;
    margin: 4px;
    text-decoration: none;
  }

  .btn:disabled {
    background-color: gray;
    opacity: 100% !important;
  }

  .btn:hover {
    opacity: 50%;
  }

  .btn-block {
    width: 100%;
  }
`;
