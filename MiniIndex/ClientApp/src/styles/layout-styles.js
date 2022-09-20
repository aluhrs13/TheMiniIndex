import { css } from "lit";

export const rowStyles = css`
  .row {
    display: flex;
    flex-direction: row;
    max-width: 100%;
  }
`;

export const sidebarStyles = css`
  .with-sidebar {
    display: flex;
    flex-wrap: wrap;
    gap: var(--s1);
  }

  .with-sidebar > :first-child {
    flex-basis: 250px;
    flex-grow: 1;
  }

  .with-sidebar > :last-child {
    flex-basis: 0;
    flex-grow: 999;
    min-width: 50%;
  }
`;

export const switcherStyles = css`
  .switcher {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
  }

  .switcher > * {
    flex-grow: 1;
    flex-basis: calc((40rem - 100%) * 999);
  }

  .switcher > :nth-last-child(n + 5),
  .switcher > :nth-last-child(n + 5) ~ * {
    flex-basis: 100%;
  }
`;

export const stackStyles = css`
  .stack {
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    max-width: 100%;
  }

  .stack > * {
    margin-top: 0;
    margin-bottom: 0;
  }

  .stack > * + * {
    margin-top: var(--space, 1.5rem);
  }
`;

export const gridStyles = css`
  .grid {
    display: grid;
    grid-gap: 1rem;
  }

  @supports (width: min(250px, 100%)) {
    .grid {
      grid-template-columns: repeat(auto-fit, minmax(min(250px, 100%), 1fr));
    }
  }
`;

export const centerStyles = css`
  .center {
    box-sizing: content-box;
    margin-left: auto;
    margin-right: auto;
    max-width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .center-t {
    box-sizing: content-box;
    margin-left: auto;
    margin-right: auto;
    text-align: center;
    max-width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
`;
