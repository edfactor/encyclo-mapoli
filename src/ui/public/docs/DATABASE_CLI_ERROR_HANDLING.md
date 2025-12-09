# Database CLI Error Handling - Implementation Guide

## Problem Statement

**Original Issue**: When Database CLI operations failed, errors were not properly surfaced to users in the Aspire Dashboard. Failures could occur silently or be missed because:

1. Only stderr output was checked (process exit code ignored)
2. Error notifications didn't include enough detail
3. Success could be incorrectly reported even when operations failed

## Solution Implemented

### 1. Exit Code Priority

Now uses process exit code as the **primary indicator** of success/failure:

```csharp
// Determine success based on exit code (most reliable indicator)
if (process.ExitCode != 0)
{
    // Operation failed - collect error details
    var errorMessage = !string.IsNullOrWhiteSpace(error)
        ? error
        : $"Process exited with code {process.ExitCode}. Check console logs for details.";

    logger.LogError("Process failed with exit code {ExitCode}: {ErrorMessage}",
        process.ExitCode, errorMessage);
    result = new ExecuteCommandResult { Success = false, ErrorMessage = errorMessage };
}
```

**Rationale**: Process exit codes are the standard way to indicate success (0) or failure (non-zero) across all platforms and tools.

### 2. Improved Error Messages

Error notifications now include:

- ❌ Clear failure indicator in title
- **Bold formatting** for emphasis
- **Formatted error details** in code blocks
- Instructions to check console logs

````csharp
_ = interactionService.PromptNotificationAsync(
    title: $"❌ Failed: {operationName}",
    message: $"**Database operation failed**: {operationName}\n\n**Error Details:**\n```\n{result.ErrorMessage}\n```\n\nCheck console logs for more information.",
    options: new NotificationInteractionOptions
    {
        Intent = MessageIntent.Error
    });
````

### 3. Proper Output Logging

Standard output is now logged at appropriate level:

```csharp
// Log output for debugging
if (!string.IsNullOrWhiteSpace(output))
{
    logger.LogInformation(output);
}
```

### 4. Stderr Handling

Distinguishes between errors and warnings written to stderr:

```csharp
else if (!string.IsNullOrWhiteSpace(error))
{
    // Exit code 0 but stderr has content - log as warning but treat as success
    // (some tools write non-error info to stderr)
    logger.LogWarning("Process succeeded but wrote to stderr: {StdErr}", error);
    result = CommandResults.Success();
}
```

**Rationale**: Some tools (like npm, dotnet) write informational messages to stderr even on success.

## Error Flow Visualization

### Before (Broken)

```
Database operation fails
  ↓
Exit code = 1, stderr has error message
  ↓
Code only checks stderr (found error text)
  ↓
Logs error but might report success if logic is flawed
  ↓
User sees "Success" notification 😕
```

### After (Fixed)

```
Database operation fails
  ↓
Exit code = 1 (non-zero = failure)
  ↓
Check exit code FIRST (primary indicator)
  ↓
Extract error message from stderr OR create message from exit code
  ↓
Log detailed error with exit code
  ↓
Show prominent error notification with ❌ icon
  ↓
User clearly sees failure with details 👍
```

## Error Notification Example

When a database operation fails, users now see:

````
┌─────────────────────────────────────────────────────┐
│ ❌ Failed: Step 2/3: Import from READY              │
│                                                     │
│ **Database operation failed**: Step 2/3: Import    │
│ from READY                                          │
│                                                     │
│ **Error Details:**                                  │
│ ```                                                 │
│ ORA-12154: TNS:could not resolve the connect       │
│ descriptor specified                                │
│ ```                                                 │
│                                                     │
│ Check console logs for more information.           │
│                                                     │
│ [Dismiss]                                           │
└─────────────────────────────────────────────────────┘
````

**Features**:

- ❌ Red error icon and intent
- Bold formatting highlights key info
- Code block preserves error formatting
- Link to console logs for details
- Stays visible until user dismisses

## Testing Error Handling

### Manual Testing

1. **Test Database Connection Failure**:

   - Stop the Oracle database
   - Run "Upgrade database" command
   - Verify error notification appears with connection error details

2. **Test Migration Failure**:

   - Create a breaking migration
   - Run "Upgrade database"
   - Verify exit code is non-zero and error is shown

3. **Test Nuclear-Option Failure**:
   - Cause step 2 to fail (invalid SQL file path)
   - Verify Nuclear-Option stops at failed step
   - Verify error notification shows which step failed

### Automated Testing

```csharp
[Fact]
public void RunConsoleApp_WhenProcessFails_ReturnsFailureResult()
{
    // Arrange: Command that will fail
    var logger = Mock.Of<ILogger>();

    // Act: Run command that exits with non-zero code
    var result = CommandHelper.RunConsoleApp(
        projectPath,
        "invalid-profile",
        logger,
        "Test Operation");

    // Assert
    result.Success.ShouldBeFalse();
    result.ErrorMessage.ShouldNotBeNullOrEmpty();
}
```

## Exit Code Reference

Common exit codes you might encounter:

| Exit Code | Meaning              | Example Scenario                |
| --------- | -------------------- | ------------------------------- |
| 0         | Success              | Operation completed normally    |
| 1         | General error        | Unhandled exception in CLI      |
| 2         | Misuse of command    | Invalid arguments to dotnet run |
| 127       | Command not found    | dotnet not in PATH              |
| 130       | Terminated by Ctrl+C | User cancelled operation        |
| 137       | Killed by signal     | Out of memory, process killed   |

## Logging Improvements

### Before

```csharp
logger.LogError(output);  // Wrong level for output
logger.LogError(error);   // Only logs if stderr has content
```

### After

```csharp
logger.LogInformation(output);  // Correct level for normal output
logger.LogError("Process failed with exit code {ExitCode}: {ErrorMessage}",
    process.ExitCode, errorMessage);  // Structured logging with exit code
logger.LogWarning("Process succeeded but wrote to stderr: {StdErr}", error);  // Distinguishes warnings
```

**Benefits**:

- Proper log levels for filtering
- Structured logging with exit codes
- Clear distinction between errors and warnings
- More debugging context

## Best Practices

### For CLI Tool Authors

1. **Always set appropriate exit codes**:

   ```csharp
   return 0;  // Success
   return 1;  // General failure
   return 2;  // Configuration error
   ```

2. **Write meaningful error messages to stderr**:

   ```csharp
   await Console.Error.WriteLineAsync($"Error: Connection to database failed: {ex.Message}");
   ```

3. **Include actionable guidance**:
   ```csharp
   Console.Error.WriteLine("Check connection string in appsettings.json");
   ```

### For Aspire Dashboard Users

1. **Always check notifications** - Error notifications persist until dismissed
2. **Click "View Logs" link** in notifications for full details
3. **Check console output** in Database-Cli resource for complete context
4. **Look for exit codes** in logs to understand severity

## Troubleshooting

### "Success notification shown but operation failed"

**Cause**: Old version without exit code checking  
**Fix**: Pull latest changes with proper exit code handling

### "Error notification has no details"

**Cause**: CLI wrote error to stdout instead of stderr  
**Fix**: Check console logs (linked in notification) for full output

### "Notification disappears before I can read it"

**Cause**: Info/Success notifications auto-dismiss after 5 seconds  
**Fix**: Error notifications persist until dismissed (no auto-dismiss)

## Related Documentation

- **Main Guide**: `DATABASE_CLI_INTERACTION_GUIDE.md`
- **Quick Reference**: `DATABASE_CLI_INTERACTION_QUICK_REFERENCE.md`
- **Flow Diagrams**: `DATABASE_CLI_INTERACTION_FLOW.md`

## Commit History

- **Initial Implementation**: Added interaction service notifications
- **Error Handling Fix**: Improved exit code checking and error surfacing (this document)

---

**Error Handling Guide Version 1.0** | October 2, 2025  
**Critical Bug Fix**: Exit code now properly checked before determining success/failure
