; Unshipped analyzer changes
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

| Rule ID | Category    | Severity | Notes                                                                     |
| ------- | ----------- | -------- | ------------------------------------------------------------------------- |
| DSM003  | Reliability | Warning  | SSN used alone as dictionary key - will crash at runtime with duplicates  |
| DSM004  | Design      | Error    | Manual name concatenation fallback detected - use FullName only           |
| DSM005  | Design      | Warning  | Use FullName instead of non-standard person name properties in DTOs       |
| DSM006  | Design      | Error    | Math.Round must use MidpointRounding.AwayFromZero for financial rounding  |
| DSM007  | Security    | Error    | Age and DateOfBirth must be annotated with MaskSensitive in response DTOs |
| DSM008  | Security    | Error    | Assignments to response DTO Ssn must use MaskSsn()                        |
| DSM009  | Security    | Error    | BadgeNumber should not be annotated with MaskSensitive                    |

### Changed Rules

| Rule ID | Category | Severity | Notes                                                            |
| ------- | -------- | -------- | ---------------------------------------------------------------- |
| DSM002  | Design   | Info     | Expanded to support class DTOs and *Dto/*DetailDto type suffixes |
