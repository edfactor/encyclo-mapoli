# Database CLI Interaction Service - Quick Reference

## Quick Start

### Running Nuclear-Option with Notifications

```bash
aspire run
```

Navigate to Database-Cli → Click "Full Nuclear Reset" → See interactive notifications!

## Code Patterns

### Basic Command with Notifications

```csharp
.WithCommand(
    name: "my-command",
    displayName: "My Command",
    executeCommand: async (c) =>
    {
        var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
        return await Task.FromResult(
            CommandHelper.RunConsoleApp(
                projectPath!, 
                "launch-profile", 
                logger, 
                "Operation Name",  // Shows in notifications
                interactionService));
    },
    commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
```

### Command with Confirmation Dialog

```csharp
executeCommand: async (c) =>
{
    var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
    
    if (interactionService.IsAvailable)
    {
        var confirmation = await interactionService.PromptConfirmationAsync(
            title: "⚠️ Confirm Action",
            message: "Are you sure? This cannot be undone!",
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
            return CommandResults.Failure("User cancelled.");
        }
    }
    
    return await Task.FromResult(CommandHelper.RunConsoleApp(...));
}
```

### Multi-Step Operation with Progress

```csharp
executeCommand: async (c) =>
{
    var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
    
    // Initial notification
    if (interactionService.IsAvailable)
    {
        await interactionService.PromptNotificationAsync(
            title: "Starting Multi-Step Operation",
            message: "This will take several minutes...",
            options: new NotificationInteractionOptions
            {
                Intent = MessageIntent.Information
            });
    }
    
    // Step 1
    var step1 = CommandHelper.RunConsoleApp(
        projectPath!, "profile1", logger, "Step 1/3: First Task", interactionService);
    if (!step1.Success) return CommandResults.Failure($"Failed at step 1: {step1.ErrorMessage}");
    
    // Step 2
    var step2 = CommandHelper.RunConsoleApp(
        projectPath!, "profile2", logger, "Step 2/3: Second Task", interactionService);
    if (!step2.Success) return CommandResults.Failure($"Failed at step 2: {step2.ErrorMessage}");
    
    // Step 3
    var step3 = CommandHelper.RunConsoleApp(
        projectPath!, "profile3", logger, "Step 3/3: Final Task", interactionService);
    if (!step3.Success) return CommandResults.Failure($"Failed at step 3: {step3.ErrorMessage}");
    
    // Success notification
    if (interactionService.IsAvailable)
    {
        await interactionService.PromptNotificationAsync(
            title: "✅ Operation Complete!",
            message: "All steps completed successfully.",
            options: new NotificationInteractionOptions
            {
                Intent = MessageIntent.Success
            });
    }
    
    return CommandResults.Success();
}
```

## Notification Types

| Intent | Icon | Use Case |
|--------|------|----------|
| `MessageIntent.Information` | ℹ️ | Starting operations, informational updates |
| `MessageIntent.Success` | ✅ | Operation completed successfully |
| `MessageIntent.Warning` | ⚠️ | Confirmation dialogs, cautionary messages |
| `MessageIntent.Error` | ❌ | Failed operations with error details |

## Common Checklist

### Adding Notifications to Existing Command

- [ ] Add `using Aspire.Hosting;` to Program.cs
- [ ] Retrieve `IInteractionService` from `c.ServiceProvider`
- [ ] Pass interaction service to `CommandHelper.RunConsoleApp`
- [ ] Provide descriptive operation name
- [ ] Add confirmation dialog if operation is destructive
- [ ] Test in Aspire dashboard (`aspire run`)

### Creating New Command with Notifications

- [ ] Register command with `.WithCommand()`
- [ ] Use `async (c) =>` for executeCommand callback
- [ ] Get interaction service from DI container
- [ ] Check `interactionService.IsAvailable` before use
- [ ] Show starting notification for long operations
- [ ] Pass service to CommandHelper
- [ ] Show final success/error notification for multi-step flows
- [ ] Test all success and error paths

## Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| No notifications appear | Must run via `aspire run` (not `dotnet run`) |
| Confirmation doesn't block | Check `if (!confirmation.Data) return Failure;` |
| Notifications overlap | Reduce frequency, use markdown in single notification |
| Wrong status shown | Verify `result.Success` is accurate |

## Examples in Codebase

**Nuclear-Option** (multi-step with confirmation):
- File: `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs`
- Lines: ~50-120
- Shows: Confirmation, progress notifications, final summary

**Drop-Recreate-DB** (single step with confirmation):
- File: `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs`
- Lines: ~35-50
- Shows: Confirmation dialog for destructive operation

**Upgrade-DB** (simple notification):
- File: `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs`
- Lines: ~25-35
- Shows: Basic start/complete notifications

## Essential Documentation

- **Comprehensive Guide**: `DATABASE_CLI_INTERACTION_GUIDE.md`
- **Microsoft Docs**: [Aspire Interaction Service](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/interaction-service)
- **Source Files**:
  - `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs`
  - `src/services/src/Demoulas.ProfitSharing.AppHost/Helpers/CommandHelper.cs`

---

**Quick Reference Version 1.0** | October 2, 2025
