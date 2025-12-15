# Pull Request Checklist: FullName Pattern

This checklist has been merged into the master code review instructions to eliminate duplication.

Use: [CODE_REVIEW_CHECKLIST.md](./CODE_REVIEW_CHECKLIST.md) → **Security → PII Protection & Data Exposure → “FullName pattern for person-name DTOs (backend + frontend)”**.

## Quick PR Verification (optional)

```markdown
## FullName Pattern Compliance

- [ ] Backend DTO uses `FullName` + `[MaskSensitive]`
- [ ] Backend mapping uses `DtoCommonExtensions.ComputeFullNameWithInitial(...)`
- [ ] Query includes `LastName`, `FirstName`, `MiddleName`
- [ ] Frontend DTO uses `fullName`
- [ ] UI renders `person.fullName` (no concatenation)
- [ ] Grids use `createNameColumn({ field: "fullName" })`
- [ ] Tests cover middle-name and no-middle-name cases
```
