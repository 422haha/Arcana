import { Preview } from '@storybook/react';
import '../src/app/styles/tailwind.css';

const preview: Preview = {
  parameters: {
    actions: { argTypesRegex: '^on[A-Z].*' },
    controls: { expanded: true },
    docs: { inlineStories: true },
    design: {
      // You can add custom configurations here
    },
  },
};

export default preview;
