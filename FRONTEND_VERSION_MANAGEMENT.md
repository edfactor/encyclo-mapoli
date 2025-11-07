# Frontend Version Management from .buildinfo.json

## Overview
The frontend build process now automatically reads the application version from `.buildinfo.json` and uses it to:
1. Update `package.json` version during deployment builds
2. Expose version as a constant in the Vite build for use in the application

## How It Works

### 1. Vite Configuration (`src/ui/vite.config.ts`)
- Reads `.buildinfo.json` at build time
- Constructs version string as `{buildNumber}.{buildId}`
- Falls back to `1.0.1` if file not found (development scenario)
- Exposes version as `__APP_VERSION__` constant in build

### 2. Pipeline Updates (`bitbucket-pipelines.yml`)
Both QA and UAT build steps now:
1. Extract `buildNumber` and `buildId` from `.buildinfo.json`
2. Run `npm version "{buildNumber}.{buildId}" --no-git-tag-version` to update `package.json`
3. Proceed with normal build process

### 3. TypeScript Support (`src/ui/src/vite-env.d.ts`)
Added declaration for `__APP_VERSION__` global constant for TypeScript type safety.

## Usage in Code

### Access the Build Version in React Components
```typescript
const App = () => {
  // __APP_VERSION__ is automatically available as a string
  console.log(`Application version: ${__APP_VERSION__}`);
  
  return (
    <div>
      Version: {__APP_VERSION__}
    </div>
  );
};
```

### Example Output
If `.buildinfo.json` contains:
```json
{
  "buildNumber": "2024.11",
  "buildId": "42",
  "branch": "release",
  "commitHash": "abc123def456"
}
```

Then:
- `package.json` version becomes: `2024.11.42`
- `__APP_VERSION__` constant equals: `2024.11.42`
- Build artifacts reference this version

## Development Notes

### Development Build (Local)
- `.buildinfo.json` typically doesn't exist in dev environment
- Vite defaults to `1.0.1` (from fallback)
- Application still works normally

### Deployment Build (QA/UAT)
- `.buildinfo.json` is generated in previous pipeline step
- Version is automatically extracted and applied
- No manual version management needed

## Integration Points

1. **Build Information Display** - Use `__APP_VERSION__` in UI to show current build
2. **API Version Headers** - Can be included in API calls for debugging
3. **Release Tracking** - Version corresponds to BitBucket build number

## Files Modified

- `src/ui/vite.config.ts` - Added buildinfo.json reading and version constant
- `bitbucket-pipelines.yml` - Added version update steps to QA and UAT builds
- `src/ui/src/vite-env.d.ts` - Added TypeScript declaration

## Benefits

✓ **Automatic Versioning** - No manual version updates needed
✓ **Single Source of Truth** - Version comes from build system
✓ **Build Traceability** - Version directly tied to build number
✓ **Type Safe** - Proper TypeScript support
✓ **Development Friendly** - Graceful fallback for local development
