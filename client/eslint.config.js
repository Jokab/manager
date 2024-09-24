import pluginVue from 'eslint-plugin-vue';
import typesciptParser from '@typescript-eslint/parser'
import vueParser from 'vue-eslint-parser';

export default [
    // add more generic rulesets here, such as:
    // js.configs.recommended,
    ...pluginVue.configs['flat/recommended'],
    {
        rules: {
            // override/add rules settings here, such as:
            // 'vue/no-unused-vars': 'error'
        },
    },
    {
        languageOptions: {
            parser: vueParser,
            parserOptions: {
                parser: typesciptParser,
                sourceType: 'module',
            },
        },
    },
]   