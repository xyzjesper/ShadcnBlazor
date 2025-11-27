const plugin = require('tailwindcss/plugin');
const classes = require("./classes.json");

module.exports = plugin.withOptions(
    (options = {}) => {
        return () => {}
    },
    (options = {}) => {
        return {
            safelist: classes,
        }
    }
);