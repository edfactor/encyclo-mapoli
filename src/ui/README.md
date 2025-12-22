# Smart Profit Sharing UI

A React + TypeScript web application for Demoulas' Smart Profit Sharing platform. This project leverages the custom `smart-ui-library` for UI components, Tailwind CSS for styling, and Redux Toolkit for state management.

## Features

- Modular page structure for reports, updates, and inquiries
- Custom grids, accordions, and modals from `smart-ui-library`
- Tailwind CSS with custom theme and color palette
- Redux Toolkit for API and state management
- TypeScript throughout for type safety

## Project Structure

- `src/pages/` — Main app pages (reports, updates, etc.)
- `src/components/` — Shared UI components (Accordions, Grids, Layout, etc.)
- `src/reduxstore/` — Redux slices, API logic, and types
- `src/utils/` — Utility functions (date, formatting, etc.)
- `tailwind.config.js` — Tailwind setup with custom colors and fonts

## Setup

1. **Install recommended VS Code extensions**

   - Open the project in VS Code and install the recommended extensions when prompted, or manually install:
     - Prettier - Code formatter
     - Tailwind CSS IntelliSense
   - This ensures format-on-save works correctly with the project's coding standards.

2. **Authenticate with Demoulas JFrog Artifactory**
   - Follow the Set Me Up guide to [authenticate your local npm client.](https://demoulas.atlassian.net/wiki/spaces/JFD/pages/146047103/Register+JFrog+as+a+custom+npm+registry)
3. **Create `.npmrc` in `src/ui/`**
   ```
   registry=https://registry.npmjs.org/
   smart-ui-library:registry=https://demoulas.jfrog.io/artifactory/api/npm/npm-smart-registry-local/
   ```
4. **Install dependencies**
   ```
   npm i
   ```
5. **Start the development server**
   ```
   npm run dev
   ```

## Development

- **Run**: `npm run dev` (Vite dev server)
- **Lint**: `npm run lint` (must pass with **0 warnings**)
- **Build (QA)**: `npm run build:qa`

### Verification (Required)

- `npm run lint` (0 warnings)
- `npm run build:qa`

### Numeric Inputs (No Spinners)

Do not rely on native numeric input spinners (e.g., `type="number"`) for badge/SSN-style fields. Prefer text inputs with `inputMode="numeric"` + validation.

## UI Library & Styling

- All major UI elements (grids, accordions, modals, etc.) are imported from `smart-ui-library`.
- Tailwind CSS is configured in `tailwind.config.js` to include the library and custom color palette.

## Notes

- TypeScript is enforced throughout the codebase.
- For any issues with `smart-ui-library`, ensure your `.npmrc` is correct and you are authenticated with JFrog.
- See `src/pages/` for main feature implementations and `src/components/` for reusable UI.

## Playwright

- Create a file on the main folder called `.playwright.env`
- Add following content in to the file:
- CI=false
- QA=false
- USER_NAME= your_username
- PASSWORD= your_password
- To write a test case see file `e2e/master-inquiry/master-inquiry-page.test.ts`

---

For more details, see inline comments in the codebase or contact the project maintainers.
