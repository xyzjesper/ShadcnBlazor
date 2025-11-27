import tailwindcss from '@tailwindcss/postcss';
import extractClasses from './extract-classes.js';

const config = {
    plugins: [
        tailwindcss(),
    ],
};

if (process.env.EXTRACT_CLASSES === "true") {
    config.plugins.push(extractClasses());
}

export default config;
