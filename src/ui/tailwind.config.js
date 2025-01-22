/** @type {import('tailwindcss').Config} */
import colors from "./src/colors";

module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "./node_modules/smart-ui-library/**/*.{js,jsx,ts,tsx}",
  ],
  important: "#root",
  theme: {
    extend: {
      fontFamily: {
        lato: "'Lato', sans-serif",
        sans: "Roboto",
      },
      colors,
    },
  },
};