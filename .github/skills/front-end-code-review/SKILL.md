---
name: front-end-code-review
description: Review React/TypeScript code against project patterns and best practices. Use when reviewing code, checking PRs, analyzing frontend code quality, or when the user mentions code review, PR review, or frontend review.
allowed-tools: Read, Grep, Glob, Bash, Task
---

# React/TypeScript Code Reviewer

Review frontend code against established project patterns and general React/TypeScript best practices.

## Instructions

### Step 1: Load Project Patterns

First, find and read the project patterns documentation:

```
Glob: **/FRONT_END_PATTERNS.md
```

Read this file to understand the project-specific conventions and review checklists.

### Step 2: Identify Files to Review

Either:

- Review specific files the user mentions
- Find changed files: `git diff --name-only develop | grep -E '\.(tsx?|jsx?)$'`
- Review a feature area via glob patterns

### Step 3: Review Each File

For each file:

1. Read the file content
2. Determine the file type (component, hook, API, test, grid columns, etc.)
3. Apply the relevant checklists from FRONT_END_PATTERNS.md
4. Check for general React/TypeScript best practices

### Step 4: Report Findings

Organize findings by severity:

- **Critical**: Must fix before merge (bugs, security issues, broken patterns)
- **Important**: Should fix (pattern violations, missing tests, poor practices)
- **Suggestion**: Nice to have (style improvements, minor optimizations)

Include positive observations about what was done well.

---

## General Best Practices (in addition to project patterns)

### Performance

- No unnecessary re-renders (check dependency arrays)
- Expensive computations memoized
- Large lists paginated or virtualized

### Security

- No dangerouslySetInnerHTML with user input
- Sensitive data not logged to console

### Accessibility

- Interactive elements keyboard accessible
- Form inputs have labels

### Code Quality

- No commented-out code or console.log statements
- Meaningful variable names
- DRY principle followed
- **Grid column definitions in separate `*Columns.tsx` files (not inline)**
- **Grid columns use factory functions from `gridColumnFactory.ts` where applicable**
- **MANDATORY: All forms use Yup validation with React Hook Form (NO manual validation like `if (!value)` checks)**
- **FORBIDDEN: Using `useState` for form field management (use `useForm` from React Hook Form)**

### TypeScript

- No `any` type (project requirement)
- Strict null checks handled
- Type guards for narrowing

---

## Report Template

```markdown
# Code Review: [File/Feature Name]

## Summary

[Brief overview of what was reviewed and overall assessment]

## Critical Issues

[Must fix before merge]

## Important Issues

[Should fix - pattern violations, missing tests]

## Suggestions

[Nice to have improvements]

## Positive Observations

[What was done well]
```

---

## Example Usage

User: "Review the Termination component"

1. Glob for `**/FRONT_END_PATTERNS.md` and read it
2. Glob for `**/Termination*.tsx` to find relevant files
3. Read each file and apply relevant checklists from patterns doc
4. Generate report with findings by severity
