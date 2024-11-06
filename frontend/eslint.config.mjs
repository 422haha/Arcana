// @ts-check
// https://typescript-eslint.io/getting-started/typed-linting

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import prettierConfig from 'eslint-config-prettier';

export default tseslint.config(
  eslint.configs.recommended,
  // ...tseslint.configs.recommended
  // ...tseslint.configs.strict,
  // ...tseslint.configs.stylistic,
  ...tseslint.configs.strictTypeChecked,
  ...tseslint.configs.stylisticTypeChecked,
  prettierConfig, // https://typescript-eslint.io/users/what-about-formatting/#suggested-usage---prettier
  {
    languageOptions: {
      parserOptions: {
        projectService: true, // indicates to ask TypeScript's type checking service for each source file's type information (see Parser#projectService).
        tsconfigRootDir: import.meta.dirname, // tells our parser the absolute path of your project's root directory (see Parser#tsconfigRootDir).
      },
    },
  }
);
