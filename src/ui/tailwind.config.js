/** @type {import('tailwindcss').Config} */

module.exports = {
  content: ["./src/**/*.{js,jsx,ts,tsx}", "./node_modules/smart-ui-library/**/*.{js,jsx,ts,tsx}"],
  important: "#root",
  theme: {
    extend: {
      fontFamily: {
        lato: "'Lato', sans-serif",
        sans: "Roboto"
      },
      colors: {
        "dsm-secondary": "#0258A5", // dsm-blue
        "dsm-secondary-hover": "#023F75",
        "dsm-secondary-pressed": "#033059",
        "dsm-primary": "#ED1848", // dsm-red
        "dsm-action": "#2E7D32",
        "dsm-action-hover": "#1B5E20",
        "dsm-action-pressed": "#124115",
        "dsm-action-secondary-hover": "#EAF2EA",
        "dsm-action-secondary-pressed": "#D0D9D0",
        "dsm-desctructive": "#DB1532",
        "dsm-destructive-hover": "#B21129",
        "dsm-destructive-pressed": "#9D0D23",
        "dsm-destructive-secondary-hover": "#DB1532",
        "dsm-destructive-secondary-pressed": "#E6C2C8",
        "dsm-grey": "#231F20", // default text
        "dsm-grey-secondary": "#7b7979",
        "dsm-grey-disabled": "#ABAAAA",
        "dsm-grey-input-border": "#C6C8CA",
        "dsm-grey-hover": "#F5F5F5",
        "dsm-grey-divder": "#E1E2E3",
        "dsm-white": "#FFFFFF",
        "dsm-focused": "#4BABDE",
        "dsm-utility": "#E41DC4",
        "dsm-app-banner": "#f2f2f2",
        "dsm-error": "#db1532",
        // "dsm-report-background": "#525653",
        "dsm-report-background": "#E1E2E3",
        "dsm-textField-disabled-bg": "#231F201F",
        "dsm-textField-disabled-text": "#231F20E5",
        "dsm-black": "#231F20",
        "dsm-filter": "#0259a507",
        "dsm-accordion-header": "rgba(2, 88, 165, 0.1)",
        "dsm-totals-row": "rgb(232, 232, 232)"
      }
    }
  }
};
