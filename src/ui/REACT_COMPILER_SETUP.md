# React Compiler Setup Guide

## Overview

The React Compiler has been successfully integrated into the Smart Profit Sharing UI project. This compiler automatically optimizes your React components by memoizing values and reducing unnecessary re-renders.

**React Version**: 19.1.0 ✅ (Fully compatible)

## What Was Changed

### 1. **Package Installation**

- Added `babel-plugin-react-compiler` (latest experimental version) to devDependencies
- Run `npm install` to get the package (already done)

### 2. **Configuration Files Updated**

#### `.babelrc` (NEW)

```json
{
  "plugins": [
    [
      "babel-plugin-react-compiler",
      {
        "target": "19"
      }
    ]
  ]
}
```

- Configures the compiler to target React 19
- Located at: `src/ui/.babelrc`

#### `vite.config.ts` (MODIFIED)

- Imported the React Compiler config
- Updated the `@vitejs/plugin-react` to use the compiler via Babel
- The compiler now runs automatically during development and production builds

## How It Works

The React Compiler:

1. **Automatic Memoization**: Automatically applies memoization logic to your components without needing to manually add `useMemo` or `useCallback`
2. **Smart Re-render Optimization**: Prevents unnecessary re-renders by understanding component dependencies
3. **Zero Runtime Overhead**: Optimizations are applied at build time
4. **Backwards Compatible**: Works with existing code; no refactoring required

## Starting Development

```powershell
cd src/ui
npm run dev  # Start dev server on port 3100
```

The compiler will:

- Run automatically during development
- Analyze your components for optimization opportunities
- Warn about any violations in the React Rules of Hooks or dependency patterns

## Building for Production

```powershell
# Production build (all modes)
npm run build:prod    # Production
npm run build:qa      # QA environment
npm run build:uat     # UAT environment
```

The compiler optimizations are included in the production build.

## Important React Rules

To get the maximum benefit from the compiler, follow these React Rules:

### ✅ DO:

- Use hooks only at the top level of components (not in loops, conditions, or nested functions)
- Keep component functions pure (same inputs = same outputs)
- Declare dependencies correctly in `useEffect` dependency arrays
- Use `key` prop when rendering lists

### ❌ DON'T:

- Call hooks conditionally
- Call hooks inside nested functions
- Mutate props or state directly
- Use implicit dependencies in effects

## Troubleshooting

### Issue: Build Errors

**If you see compiler warnings during build**:

- The compiler will warn about potential issues with React Rules
- Fix the warnings by restructuring your code to follow React rules
- This is actually helpful - the compiler catches issues that could cause bugs

### Issue: Unexpected Behavior

**If components behave unexpectedly after building**:

1. Check the browser console for warnings
2. Review component dependency arrays in `useEffect` hooks
3. Ensure all functions are pure (no side effects)

### Issue: Performance Not Improved

**If performance doesn't improve**:

- The compiler optimizes automatically; improvements depend on your component structure
- Focus on reducing component complexity and large prop objects
- Profile with React DevTools to see where time is spent

## Monitoring Compiler Optimizations

### Development

During development, check the browser console:

- Look for any warnings from the compiler
- These indicate code that should be refactored

### Production

- Build sizes should remain similar (compiler optimizations are build-time)
- Runtime performance should be improved or unchanged
- No new console errors should appear

## Configuration Options

To customize compiler behavior, modify `.babelrc`:

```json
{
  "plugins": [
    [
      "babel-plugin-react-compiler",
      {
        "target": "19",
        "environment": "development" // or "production"
      }
    ]
  ]
}
```

## Resources

- [React Compiler Documentation](https://react.dev/learn/react-compiler)
- [React Rules of Hooks](https://react.dev/reference/rules/rules-of-hooks)
- [React Rules of Components](https://react.dev/reference/rules/components-are-pure)
- [Babel Plugin React Compiler](https://github.com/facebook/react/tree/main/compiler)

## Next Steps

1. ✅ Run `npm run dev` to verify the setup
2. ✅ Build the project with `npm run build:prod`
3. ✅ Check console for any compiler warnings
4. ✅ Test your application as normal

The compiler is now active and optimizing your components automatically!

## Disabling the Compiler (If Needed)

To temporarily disable the compiler during development:

**In `vite.config.ts`**, comment out the Babel configuration:

```typescript
react({
  // babel: {
  //   plugins: [["babel-plugin-react-compiler", ReactCompilerConfig]]
  // }
});
```

Or remove the `.babelrc` file entirely.

---

_Setup completed on November 21, 2025_
