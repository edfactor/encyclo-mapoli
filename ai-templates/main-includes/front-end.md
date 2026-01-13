## Frontend Conventions

- Node managed via Volta; assume Node 20.x LTS. Do not hardcode npx version hacks
- Package registry split: `.npmrc` sets private `smart-ui-library` registry; keep that line when modifying
- State mgmt: Centralize API/data logic in `src/reduxstore/`; prefer RTK Query or slices patterns already present
- Styling: Tailwind utility-first; extend via `tailwind.config.js`; avoid inline style objects for reusable patterns—create small components
- **MANDATORY Form Validation**: ALL forms MUST use Yup validation with React Hook Form. NO manual validation (e.g., `if (!value)` checks). See @ai-templates/front-end/fe-input-form-validation.md
- **MANDATORY Grid Column Organization**:
  - ALL grid column definitions MUST be in separate `*Columns.tsx` files (NEVER inline in components)
  - MUST use factory functions from `src/ui/src/utils/gridColumnFactory.ts` for standard column types
  - Available factories: `createDateColumn`, `createCurrencyColumn`, `createYesOrNoColumn`, `createBadgeColumn`, `createNameColumn`, `createStateColumn`, `createAddressColumn`, `createSSNColumn`, `createPhoneColumn`, `createHoursColumn`, etc.
- E2E: Playwright tests under `src/ui/e2e`; new tests should support `.playwright.env` driven creds (no hard-coded secrets)

## Testing & Quality

- Frontend: Add Playwright or component tests colocated (if pattern emerges) but keep end-to-end in `e2e/`
- Security warnings/analyzers treated as errors; keep build green

For testing of front end react components, see this file: @ai-templates/front-end/fe-unit-tests.md
