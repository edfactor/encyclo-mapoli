# Database CLI Interaction Service Guide

## Overview

The Database CLI tooling in the Profit Sharing application uses .NET Aspire's Interaction Service to provide real-time user notifications and confirmations for database operations. This guide covers how the interaction service is used, how to extend it, and troubleshooting tips.

## What is the Interaction Service?

The Aspire Interaction Service (`IInteractionService`) allows applications to:
- Display notifications to users in the Aspire Dashboard
- Prompt for user confirmation before destructive operations
- Show progress updates for long-running operations
- Provide visual feedback for success/error states

When running `aspire run`, all interactions appear as dialogs and notifications in the Aspire dashboard UI. This provides a significantly better user experience than console-only feedback.

## Features

### Nuclear-Option Command

The **Nuclear-Option** command provides the most comprehensive example of interaction service usage. This command performs a full database reset through three sequential steps:

1. **Confirmation Dialog**: Before any destructive action, users see a warning dialog explaining what will happen
2. **Progress Notifications**: Each step shows "Starting" and "Completed/Failed" notifications
3. **Final Summary**: A success notification with a link to view logs

#### Nuclear-Option Workflow

```
User clicks "Full Nuclear Reset" in Aspire Dashboard
    ↓
⚠️ Confirmation Dialog appears:
   "WARNING: This will completely destroy and recreate the database!"
    ↓
User confirms (or cancels)
    ↓
🚀 Info notification: "Starting Nuclear Option"
    ↓
Step 1/3: Drop & Recreate Database
   📍 Info: "Starting: Step 1/3: Drop & Recreate Database"
   ✅ Success: "Completed: Step 1/3: Drop & Recreate Database"
    ↓
Step 2/3: Import from READY
   📍 Info: "Starting: Step 2/3: Import from READY"
   ✅ Success: "Completed: Step 2/3: Import from READY"
    ↓
Step 3/3: Import Navigation
   📍 Info: "Starting: Step 3/3: Import Navigation"
   ✅ Success: "Completed: Step 3/3: Import Navigation"
    ↓
✅ Final success notification: "Nuclear Option Complete!"
   (includes link to view console logs)
```

### Individual Database Commands

All database commands now provide notifications:

| Command | Confirmation Required | Notifications |
|---------|----------------------|---------------|
| **upgrade-db** | No | Start/Complete/Error |
| **drop-recreate-db** | Yes ⚠️ | Confirm, Start/Complete/Error |
| **import-from-ready** | No | Start/Complete/Error |
| **import-from-navigation** | No | Start/Complete/Error |

### Notification Types

The implementation uses different notification intents to provide visual cues:

- **Information** (ℹ️): Starting operations, informational updates
- **Success** (✅): Operations completed successfully
- **Warning** (⚠️): Confirmation dialogs for destructive actions
- **Error** (❌): Failed operations with error details

## Implementation Details

### Architecture

The interaction service integration is implemented across two main files:

1. **CommandHelper.cs**: Utility methods that execute database operations and emit notifications
2. **Program.cs (AppHost)**: Command registration with interaction service integration

### CommandHelper Updates

The `RunConsoleApp` method now accepts an optional `IInteractionService` parameter:

```csharp
public static ExecuteCommandResult RunConsoleApp(
    string projectPath, 
    string launchProfile, 
    ILogger logger, 
    string? operationName = null, 
    IInteractionService? interactionService = null)
```

**Before operation starts:**
```csharp
if (interactionService?.IsAvailable == true && !string.IsNullOrWhiteSpace(operationName))
{
    _ = interactionService.PromptNotificationAsync(
        title: $"Starting: {operationName}",
        message: $"Beginning database operation: {operationName}",
        options: new NotificationInteractionOptions
        {
            Intent = MessageIntent.Information
        });
}
```

**After operation completes:**
```csharp
if (result.Success)
{
    // Success notification
    _ = interactionService.PromptNotificationAsync(
        title: $"Completed: {operationName}",
        message: $"Database operation completed successfully: {operationName}",
        options: new NotificationInteractionOptions
        {
            Intent = MessageIntent.Success
        });
}
else
{
    // Error notification
    _ = interactionService.PromptNotificationAsync(
        title: $"Failed: {operationName}",
        message: $"Database operation failed: {operationName}\n{result.ErrorMessage}",
        options: new NotificationInteractionOptions
        {
            Intent = MessageIntent.Error
        });
}
```

### AppHost Command Registration

Commands are registered with `executeCommand` callbacks that retrieve the interaction service from DI:

```csharp
.WithCommand(
    name: "upgrade-db",
    displayName: "Upgrade database",
    executeCommand: async (c) =>
    {
        var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
        return await Task.FromResult(
            CommandHelper.RunConsoleApp(projectPath!, "upgrade-db", logger, 
                "Upgrade Database", interactionService));
    },
    commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
```

### Confirmation Dialogs

Destructive operations (like `drop-recreate-db` and `Nuclear-Option`) require user confirmation:

```csharp
var confirmation = await interactionService.PromptConfirmationAsync(
    title: "⚠️ Confirm Database Drop",
    message: "Are you sure you want to **drop and recreate** the database? All data will be lost!",
    options: new MessageBoxInteractionOptions
    {
        Intent = MessageIntent.Warning,
        PrimaryButtonText = "Yes, Drop Database",
        SecondaryButtonText = "Cancel",
        ShowSecondaryButton = true,
        EnableMessageMarkdown = true
    });

if (!confirmation.Data)
{
    return CommandResults.Failure("User cancelled the operation.");
}
```

Key features:
- Markdown support for rich formatting
- Clear primary/secondary button labels
- Warning intent for visual emphasis
- Returns failure result if cancelled (prevents operation)

## Adding Notifications to New Commands

To add interaction service notifications to a new database command:

### Step 1: Update CommandHelper Call

When calling `CommandHelper.RunConsoleApp`, pass the interaction service and a descriptive operation name:

```csharp
.WithCommand(
    name: "my-new-command",
    displayName: "My New Operation",
    executeCommand: async (c) =>
    {
        var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
        return await Task.FromResult(
            CommandHelper.RunConsoleApp(
                projectPath!, 
                "my-launch-profile", 
                logger, 
                "My Operation Description",  // <- This shows in notifications
                interactionService));
    },
    commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
```

### Step 2: Add Confirmation (if destructive)

If the operation is destructive or irreversible, add a confirmation dialog:

```csharp
executeCommand: async (c) =>
{
    var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
    
    if (interactionService.IsAvailable)
    {
        var confirmation = await interactionService.PromptConfirmationAsync(
            title: "⚠️ Confirm Destructive Action",
            message: "Explain what will happen and consequences...",
            options: new MessageBoxInteractionOptions
            {
                Intent = MessageIntent.Warning,
                PrimaryButtonText = "Yes, Proceed",
                SecondaryButtonText = "Cancel",
                ShowSecondaryButton = true,
                EnableMessageMarkdown = true
            });

        if (!confirmation.Data)
        {
            return CommandResults.Failure("User cancelled the operation.");
        }
    }
    
    return await Task.FromResult(
        CommandHelper.RunConsoleApp(projectPath!, "profile", logger, "Operation", interactionService));
}
```

### Step 3: Test in Dashboard

Run `aspire run` and click your command in the Database-Cli resource to verify:
- Confirmation dialog appears (if applicable)
- Starting notification shows when operation begins
- Success/error notification shows when complete
- Notifications stack properly if multiple commands run

## Best Practices

### Notification Messages

**DO:**
- Use clear, descriptive operation names (e.g., "Step 2/3: Import from READY")
- Include context in error messages (include error details)
- Use markdown for structured messages in confirmations
- Include step numbers for multi-step operations

**DON'T:**
- Use generic names like "operation" or "command"
- Hide error details from users
- Spam users with too many notifications for trivial operations
- Use notifications without checking `interactionService?.IsAvailable`

### Confirmation Dialogs

**Always confirm before:**
- Dropping databases
- Deleting data
- Operations that cannot be undone
- Operations that take significant time

**Don't confirm for:**
- Read-only operations
- Idempotent operations (like apply migrations)
- Operations that can be easily reversed

### Progress Updates

For long-running multi-step operations:
1. Show initial "Starting" notification with overview
2. Show progress for each step (e.g., "Step 1/3")
3. Show final success/error summary with link to logs

### Error Handling

Always check if notifications succeeded:

```csharp
// Fire-and-forget is OK for non-critical notifications
_ = interactionService.PromptNotificationAsync(...);

// Await confirmation dialogs since they affect control flow
var confirmation = await interactionService.PromptConfirmationAsync(...);
if (!confirmation.Data) { return CommandResults.Failure("..."); }
```

## Troubleshooting

### Notifications Don't Appear

**Check:**
1. Is `aspire run` being used? (Notifications only work in dashboard context)
2. Is `IInteractionService.IsAvailable` checked before calling?
3. Are there any exceptions in the AppHost console output?
4. Is the command registered with `executeCommand` callback that retrieves the service?

**Common Issues:**
- Running `dotnet run` directly bypasses Aspire dashboard (no interaction service)
- Forgetting to add `using Aspire.Hosting;` directive
- Not checking `IsAvailable` before using interaction service

### Confirmation Dialog Doesn't Block Operation

**Issue:** Operation proceeds even if user cancels

**Fix:** Check the result and return failure:
```csharp
var confirmation = await interactionService.PromptConfirmationAsync(...);
if (!confirmation.Data)  // User cancelled or closed dialog
{
    return CommandResults.Failure("User cancelled the operation.");
}
// Continue with operation...
```

### Multiple Notifications Overlap

**Issue:** Too many notifications stack up and hide each other

**Solutions:**
1. Reduce notification frequency (don't notify for every minor step)
2. Use markdown in a single notification to show multiple pieces of info
3. Fire some notifications without awaiting (`_ = ...`) so they don't block
4. Clear old notifications by keeping track and dismissing them

### Notification Shows Wrong Status

**Issue:** Operation fails but success notification shows

**Check:**
- Is the exit code being checked properly?
- Are error conditions handled before showing success?
- Is the `result.Success` property accurate?

**Fix:** Ensure CommandHelper checks process exit codes:
```csharp
if (!string.IsNullOrWhiteSpace(error))
{
    result = new ExecuteCommandResult { Success = false, ErrorMessage = error };
}
else
{
    result = CommandResults.Success();
}
```

## Future Enhancements

Potential improvements to the interaction service integration:

1. **Progress Bars**: Show actual progress percentage for long operations
2. **Cancellation Support**: Allow users to cancel in-progress operations
3. **Detailed Logs in Notifications**: Include clickable links to specific log sections
4. **Input Prompts**: Ask users for parameters (e.g., target schema, year) via dialogs
5. **Notification History**: Keep track of recent operations and their outcomes
6. **Toast Notifications**: Use less-intrusive toasts for non-critical updates

## References

- [.NET Aspire Interaction Service Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/interaction-service)
- [CommandHelper.cs](../../services/src/Demoulas.ProfitSharing.AppHost/Helpers/CommandHelper.cs)
- [AppHost Program.cs](../../services/src/Demoulas.ProfitSharing.AppHost/Program.cs)
- [Database CLI Program.cs](../../services/src/Demoulas.ProfitSharing.Data.Cli/Program.cs)

## Support

For issues or questions:
1. Check the Aspire dashboard console logs (Database-Cli resource)
2. Review the operation name being passed to CommandHelper
3. Verify `IInteractionService.IsAvailable` returns true
4. Check for exceptions in the AppHost console output

---

**Last Updated:** October 2, 2025  
**Version:** 1.0  
**Author:** Development Team
