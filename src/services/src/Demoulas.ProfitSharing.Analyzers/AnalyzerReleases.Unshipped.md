### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
DSM003 | Reliability | Warning | SSN used alone as dictionary key - will crash at runtime with duplicates
DSM004 | Design | Error | Manual name concatenation fallback detected - use FullName only
DSM005 | Design | Warning | Use FullName instead of non-standard person name properties in DTOs