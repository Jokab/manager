import jsLint from "@eslint/js"
import tsLint from "typescript-eslint"
import vueLint from "eslint-plugin-vue"
import vueLint2 from "vue-eslint-parser"


export default [
    jsLint.configs.recommended,
    ...tsLint.configs.recommended,
    ...vueLint.configs["flat/essential"],
    {
        files: ["**/*.{js,mjs,cjs,ts,mts,jsx,tsx}"],
        languageOptions: {
            // common parser options, enable TypeScript and JSX
            parser: tsLint.parser,
            parserOptions: {
                sourceType: "module"
            }
        }
    },
    {
        files: ["*.vue", "**/*.vue"],
        languageOptions: {
            parser: vueLint2,
            parserOptions: {
                // <script lang="ts" /> to enable TypeScript in Vue SFC
                parser: tsLint.parser,
                sourceType: "module"
            }
        }
    }
]; 