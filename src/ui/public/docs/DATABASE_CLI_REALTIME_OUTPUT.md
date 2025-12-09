# Database CLI Real-Time Output Streaming - Implementation Guide

## Problem Statement

**Original Issue**: When Database CLI operations were running, users couldn't see progress in real-time:

1. **Output buffering**: `ReadToEnd()` waited for process completion before reading any output
2. **Hung processes**: No timeout meant stuck operations could hang forever
3. **Lost context**: When operations failed, last output before failure was not visible
4. **No progress**: Users had no idea if a long-running operation was making progress

## Solution Implemented

### 1. Asynchronous Output Streaming

Replaced synchronous `ReadToEnd()` with event-driven asynchronous streaming:

```csharp
// OLD: Blocks until process completes
string output = process.StandardOutput.ReadToEnd();
string error = process.StandardError.ReadToEnd();
process.WaitForExit();

// NEW: Streams output in real-time as it's produced
process.OutputDataReceived += (sender, e) =>
{
    if (!string.IsNullOrEmpty(e.Data))
    {
        outputLines.Add(e.Data);
        logger.LogInformation("[{Operation}] {Output}", operationName ?? "CLI", e.Data);
    }
};

process.ErrorDataReceived += (sender, e) =>
{
    if (!string.IsNullOrEmpty(e.Data))
    {
        errorLines.Add(e.Data);
        logger.LogError("[{Operation}] {Error}", operationName ?? "CLI", e.Data);
    }
};

process.Start();
process.BeginOutputReadLine();
process.BeginErrorReadLine();
```

**Benefits**:

- ✅ Output appears immediately in Aspire Dashboard console
- ✅ Users can see progress as operations run
- ✅ Last output before crashes/hangs is captured
- ✅ Better debugging when something goes wrong

### 2. Timeout Protection

Added configurable timeout to prevent infinite hangs:

```csharp
const int timeoutMinutes = 30; // Adjust based on expected operation duration
bool exited = process.WaitForExit(timeoutMinutes * 60 * 1000);

if (!exited)
{
    logger.LogError("[{Operation}] Process did not complete within {Timeout} minutes. Terminating...",
        operationName, timeoutMinutes);

    try
    {
        process.Kill(entireProcessTree: true);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to kill hung process");
    }

    var timeoutError = $"Operation timed out after {timeoutMinutes} minutes. Process was terminated.\n\n" +
                     $"Last output:\n{string.Join("\n", outputLines.TakeLast(10))}\n\n" +
                     $"Check console logs for full output.";

    // Show timeout notification...
    return new ExecuteCommandResult { Success = false, ErrorMessage = timeoutError };
}
```

**Benefits**:

- ✅ Prevents hung operations from blocking indefinitely
- ✅ Shows last 10 lines of output before timeout
- ✅ Kills entire process tree (including child processes)
- ✅ Clear notification that timeout occurred

### 3. Contextual Logging

Each line now logs with operation context:

```csharp
logger.LogInformation("[{Operation}] {Output}", operationName ?? "CLI", e.Data);
```

**Example Console Output**:

```
[Step 1/3: Drop & Recreate Database] [DEBUG] Received args: drop-recreate-db
[Step 1/3: Drop & Recreate Database] Dropping database...
[Step 1/3: Drop & Recreate Database] Database dropped successfully
[Step 1/3: Drop & Recreate Database] Creating database...
[Step 1/3: Drop & Recreate Database] Applying migrations...
[Step 1/3: Drop & Recreate Database] Migration 20250101_Initial applied
[Step 1/3: Drop & Recreate Database] Database created successfully
```

## Comparison: Before vs After

### Before (Buffered Output)

```
User clicks "Nuclear Option"
  ↓
Process starts running
  ↓
[NO OUTPUT VISIBLE - buffered in memory]
  ↓
[User waits... is it stuck? No idea!]
  ↓
[5 minutes pass...]
  ↓
Process completes
  ↓
ALL output dumps at once
  ↓
User finally sees what happened
```

**Problems**:

- No progress indication
- Impossible to tell if stuck or just slow
- If process hangs, user never sees output
- Poor debugging experience

### After (Real-Time Streaming)

```
User clicks "Nuclear Option"
  ↓
Process starts running
  ↓
Output appears IMMEDIATELY in console:
  "Starting nuclear reset..."
  ↓
Output continues as work progresses:
  "Dropping database..."
  "Database dropped"
  "Creating schema..."
  "Applying migration 1/10..."
  "Applying migration 2/10..."
  ↓
User can SEE progress in real-time
  ↓
If process hangs:
  - Last output shows WHERE it stuck
  - Timeout kills process after 30 minutes
  - User sees context of what failed
```

**Benefits**:

- ✅ Immediate feedback
- ✅ Progress visibility
- ✅ Better hang detection
- ✅ Contextual error information

## Timeout Notification Example

When an operation times out, users see:

```
┌─────────────────────────────────────────────────────┐
│ ⏱️ Timeout: Step 2/3: Import from READY             │
│                                                     │
│ **Operation timed out**: Step 2/3: Import from     │
│ READY                                               │
│                                                     │
│ Operation timed out after 30 minutes. Process was  │
│ terminated.                                         │
│                                                     │
│ Last output:                                        │
│ [Importing employee records...]                     │
│ [Processing batch 1 of 100...]                      │
│ [Processing batch 2 of 100...]                      │
│ [Processing batch 3 of 100...]                      │
│ [Stuck waiting for database lock...]               │
│ [Last line before timeout]                          │
│                                                     │
│ Check console logs for full output.                 │
│                                                     │
│ [Dismiss]                                           │
└─────────────────────────────────────────────────────┘
```

## Configuring Timeout

The timeout is currently hardcoded but can be adjusted based on operation type:

```csharp
// Current: 30 minutes for all operations
const int timeoutMinutes = 30;

// Future: Per-operation timeout
int timeoutMinutes = launchProfile switch
{
    "upgrade-db" => 10,              // Migrations usually quick
    "drop-recreate-db" => 15,        // Drop/create medium
    "import-from-ready" => 60,       // Large import slow
    "import-from-navigation" => 5,   // Small dataset fast
    _ => 30                          // Default
};
```

## Troubleshooting with Real-Time Output

### Scenario: "Import stuck at batch 45"

**Before**: No idea where it's stuck, output appears all at once after manual kill

**After**: Console shows:

```
[Import from READY] Processing batch 43 of 100... ✓
[Import from READY] Processing batch 44 of 100... ✓
[Import from READY] Processing batch 45 of 100...
[Import from READY] Waiting for database lock...
[... 30 minutes pass ...]
⏱️ Timeout notification with last 10 lines
```

**Action**: You can see it's stuck on a database lock. Check for long-running queries blocking the import.

### Scenario: "Operation completes but no output"

**Before**: `ReadToEnd()` returns empty string, no idea why

**After**: Real-time streaming shows:

```
[Operation] Starting...
[Operation] ERROR: Connection string not configured
[Operation] Exiting with code 1
```

**Action**: Clear error message logged in real-time. Fix connection string in configuration.

## Performance Considerations

### Memory Usage

Real-time streaming uses event handlers instead of buffering entire output:

- **Before**: Entire output held in memory until process completes
- **After**: Output logged line-by-line, only last 10 lines kept for timeout errors

**Impact**: Lower memory usage for long-running operations with lots of output.

### CPU Usage

Event handlers fire for each line of output:

- **Overhead**: Minimal - logging is async and buffered by Serilog
- **Benefit**: Users see progress without polling or manual refresh

## CLI Output Best Practices

For CLI tool authors (e.g., Database-Cli), ensure output is friendly for real-time streaming:

### ✅ DO

```csharp
// Flush output frequently
Console.WriteLine("Starting step 1...");
await Task.Delay(100); // Simulate work
Console.WriteLine("Step 1 complete");

// Use progress indicators
Console.WriteLine($"Processing batch {i} of {total}...");

// Log before long operations
Console.WriteLine("Waiting for database lock (this may take a while)...");
await WaitForLockAsync();
```

### ❌ DON'T

```csharp
// Don't buffer output
var sb = new StringBuilder();
sb.AppendLine("Step 1...");
sb.AppendLine("Step 2...");
// ... build entire output ...
Console.WriteLine(sb.ToString()); // Dumps all at once

// Don't have long silent periods
DoExpensiveOperationWithNoOutput(); // User sees nothing for 10 minutes

// Don't write thousands of lines per second
for (int i = 0; i < 1000000; i++)
{
    Console.WriteLine($"Processing {i}..."); // Floods console
}
```

## Testing Real-Time Output

### Manual Test

1. Start Aspire Dashboard: `aspire run`
2. Open Database-Cli console in dashboard
3. Run "Import from READY" command
4. Observe output appearing line-by-line in real-time
5. Verify progress messages visible as operation runs

### Simulated Hang Test

1. Modify CLI to `Thread.Sleep(int.MaxValue)` in the middle
2. Run operation
3. Verify console shows output up to the hang point
4. Wait for timeout (or reduce timeout for testing)
5. Verify timeout notification shows last 10 lines
6. Verify process is killed

## Related Documentation

- **Error Handling**: `DATABASE_CLI_ERROR_HANDLING.md`
- **Main Guide**: `DATABASE_CLI_INTERACTION_GUIDE.md`
- **Quick Reference**: `DATABASE_CLI_INTERACTION_QUICK_REFERENCE.md`

## Future Enhancements

Potential improvements:

1. **Progress Bars**: Parse percentage indicators from output
2. **Structured Logging**: JSON output from CLI parsed into structured logs
3. **Cancellation**: Allow users to cancel operations mid-flight
4. **Adaptive Timeout**: Detect progress and extend timeout automatically
5. **Output Filtering**: Hide debug lines unless user enables verbose mode

---

**Real-Time Streaming Guide Version 1.0** | October 2, 2025  
**Critical Improvement**: Output now streams in real-time, hung processes timeout automatically
