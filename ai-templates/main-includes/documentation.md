## Documentation Creation Guidelines

When creating documentation for new features, architectural changes, or implementation guides:

### File Locations

- **Primary Documentation**: Create `.md` files in `docs/` folder at project root
- **User-Accessible Documentation**: Copy final documents to `src/ui/public/docs/` for web access
- **Template References**: Use existing documentation structure from `docs/` folder as examples

### File naming Conventions

- Use `UPPERCASE_WITH_UNDERSCORES.md` for major guides (e.g., `TELEMETRY_GUIDE.md`, `READ_ONLY_FUNCTIONALITY.md`)
- Use `PascalCase-With-Hyphens.md` for specific features (e.g., `Distribution-Processing-Requirements.md`)
- Use ticket-prefixed names for implementation summaries (e.g., `PS-1623_READ_ONLY_SUMMARY.md`)

### Required Documentation Updates

When creating new documentation:

1. **Create primary file** in `docs/` folder with comprehensive content
2. **Update `docs/README.md`** to include new documentation references
3. **Copy to public folder** for web accessibility: `src/ui/public/docs/`
4. **Update Documentation page** in `src/ui/src/pages/Documentation/Documentation.tsx`:
   ```typescript
   {
     key: "feature-name",
     title: "Feature Documentation Title",
     filename: "FEATURE_DOCUMENTATION.md",
     description: "Brief description of what this documentation covers"
   }
   ```
