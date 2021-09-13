import { Config } from '@stencil/core';

export const config: Config = {
  namespace: 'miniindex-components',
  globalStyle: 'src/global/variables.css',
  outputTargets: [
    {
      type: 'dist',
      dir: '../MiniIndex/wwwroot/components',
      esmLoaderPath: '../loader',
    },
    {
      type: 'dist-custom-elements-bundle',
    },
    {
      type: 'docs-readme',
    },
    {
      type: 'www',
      serviceWorker: null, // disable service workers
    },
  ],
};
