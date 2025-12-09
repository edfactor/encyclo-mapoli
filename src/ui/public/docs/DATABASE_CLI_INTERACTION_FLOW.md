# Database CLI Nuclear-Option Interaction Flow

## Visual Workflow

```
┌─────────────────────────────────────────────────────────────────┐
│                     ASPIRE DASHBOARD                            │
│  Database-Cli Resource → Commands → "Full Nuclear Reset"       │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ User clicks
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│  ⚠️  CONFIRMATION DIALOG                                        │
│                                                                 │
│  WARNING: This will completely destroy and recreate             │
│  the database!                                                  │
│                                                                 │
│  This operation will:                                           │
│   1. Drop the entire database (all data permanently lost)       │
│   2. Recreate the database schema from migrations               │
│   3. Import data from READY system                              │
│   4. Import navigation data                                     │
│                                                                 │
│  ⚠️ This action cannot be undone!                               │
│                                                                 │
│  Are you absolutely sure you want to proceed?                   │
│                                                                 │
│     [Yes, Destroy and Recreate]    [Cancel]                     │
└─────────────────────────────────────────────────────────────────┘
                              │
                    ┌─────────┴─────────┐
                    │                   │
            User Confirms         User Cancels
                    │                   │
                    │                   ▼
                    │         ┌──────────────────────┐
                    │         │ ℹ️  NOTIFICATION      │
                    │         │ Operation Cancelled  │
                    │         │ Nuclear Option was   │
                    │         │ cancelled by user.   │
                    │         └──────────────────────┘
                    │                   │
                    │                   ▼
                    │              [STOP - No operation runs]
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│  🚀 NOTIFICATION (Info)                                         │
│  Starting Nuclear Option                                        │
│  Beginning full database reset. This may take several minutes...│
└─────────────────────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│  STEP 1/3: DROP & RECREATE DATABASE                             │
├─────────────────────────────────────────────────────────────────┤
│  📍 Starting: Step 1/3: Drop & Recreate Database                │
│     [Operation executes: dotnet run --launch-profile            │
│      drop-recreate-db]                                          │
│  ✅ Completed: Step 1/3: Drop & Recreate Database               │
│     Database operation completed successfully                   │
└─────────────────────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│  STEP 2/3: IMPORT FROM READY                                    │
├─────────────────────────────────────────────────────────────────┤
│  📍 Starting: Step 2/3: Import from READY                       │
│     [Operation executes: dotnet run --launch-profile            │
│      import-from-ready]                                         │
│  ✅ Completed: Step 2/3: Import from READY                      │
│     Database operation completed successfully                   │
└─────────────────────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│  STEP 3/3: IMPORT NAVIGATION                                    │
├─────────────────────────────────────────────────────────────────┤
│  📍 Starting: Step 3/3: Import Navigation                       │
│     [Operation executes: dotnet run --launch-profile            │
│      import-from-navigation]                                    │
│  ✅ Completed: Step 3/3: Import Navigation                      │
│     Database operation completed successfully                   │
└─────────────────────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│  ✅ FINAL SUCCESS NOTIFICATION                                  │
│  Nuclear Option Complete!                                       │
│  Database has been successfully reset. All steps completed.     │
│                                                                 │
│  [View Logs] → /console/Database-Cli                            │
└─────────────────────────────────────────────────────────────────┘

```

## Error Handling Flow

```
┌─────────────────────────────────────────────────────────────────┐
│  STEP 1/3: DROP & RECREATE DATABASE                             │
├─────────────────────────────────────────────────────────────────┤
│  📍 Starting: Step 1/3: Drop & Recreate Database                │
│     [Operation executes but encounters error]                   │
│  ❌ Failed: Step 1/3: Drop & Recreate Database                  │
│     Database operation failed: Step 1/3: Drop & Recreate        │
│     Database                                                    │
│     Error: Connection to database refused...                    │
└─────────────────────────────────────────────────────────────────┘
                    │
                    ▼
         [OPERATION STOPS - Remaining steps not executed]
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│  Command returns failure result:                                │
│  "Nuclear Option failed at step 1: [error details]"             │
└─────────────────────────────────────────────────────────────────┘

```

## Notification Stack Visualization

When all notifications are visible (info stacked at top):

```
╔═══════════════════════════════════════════════════════════════╗
║ ASPIRE DASHBOARD                                              ║
╠═══════════════════════════════════════════════════════════════╣
║                                                               ║
║  ┌───────────────────────────────────────────────────────┐   ║
║  │ ✅ Completed: Step 3/3: Import Navigation             │   ║
║  │    Database operation completed successfully          │   ║
║  └───────────────────────────────────────────────────────┘   ║
║                                                               ║
║  ┌───────────────────────────────────────────────────────┐   ║
║  │ ✅ Completed: Step 2/3: Import from READY             │   ║
║  │    Database operation completed successfully          │   ║
║  └───────────────────────────────────────────────────────┘   ║
║                                                               ║
║  ┌───────────────────────────────────────────────────────┐   ║
║  │ ✅ Completed: Step 1/3: Drop & Recreate Database      │   ║
║  │    Database operation completed successfully          │   ║
║  └───────────────────────────────────────────────────────┘   ║
║                                                               ║
║  [Database-Cli Console Logs]                                 ║
║  [Other Resources...]                                         ║
╚═══════════════════════════════════════════════════════════════╝
```

## Timeline Example

Typical execution timeline for Nuclear-Option:

```
Time    Event
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
00:00   User clicks "Full Nuclear Reset"
00:01   ⚠️  Confirmation dialog appears
00:05   User clicks "Yes, Destroy and Recreate"
00:06   🚀 "Starting Nuclear Option" notification
00:07   📍 "Starting: Step 1/3..." notification
00:10   [Database drop/recreate in progress...]
00:25   ✅ "Completed: Step 1/3..." notification
00:26   📍 "Starting: Step 2/3..." notification
00:30   [READY import in progress...]
02:45   ✅ "Completed: Step 2/3..." notification
02:46   📍 "Starting: Step 3/3..." notification
02:50   [Navigation import in progress...]
03:15   ✅ "Completed: Step 3/3..." notification
03:16   ✅ "Nuclear Option Complete!" notification
        (Total: ~3 minutes 16 seconds)
```

## Comparison: Before vs After

### Before Implementation

```
User clicks command
  ↓
[No visual feedback]
  ↓
[User checks console logs manually]
  ↓
[No indication of progress]
  ↓
[Console shows completion after several minutes]
```

### After Implementation

```
User clicks command
  ↓
Confirmation dialog (prevents accidents)
  ↓
"Starting" notification (immediate feedback)
  ↓
Progress notifications for each step (transparency)
  ↓
"Completed" notifications (clear success indicators)
  ↓
Final summary with link to logs (convenience)
```

## Notification Message Details

### Information Messages (ℹ️)

- **Color**: Blue
- **Auto-dismiss**: Yes (after 5 seconds typically)
- **Purpose**: Non-critical updates, operation start

### Success Messages (✅)

- **Color**: Green
- **Auto-dismiss**: Yes (after 5 seconds typically)
- **Purpose**: Operation completed successfully

### Warning Messages (⚠️)

- **Color**: Orange/Yellow
- **Auto-dismiss**: No (requires user interaction)
- **Purpose**: Confirmation dialogs, cautionary info

### Error Messages (❌)

- **Color**: Red
- **Auto-dismiss**: No (requires user dismissal)
- **Purpose**: Operation failures, errors

## User Experience Improvements

### Problem Solved

- **Before**: Users had no idea when long operations started or completed
- **After**: Clear visual feedback at every step

### Benefits

1. **Immediate Feedback**: Users know their action was registered
2. **Progress Tracking**: Users can see which step is executing
3. **Error Visibility**: Failures are obvious (not hidden in logs)
4. **Accident Prevention**: Confirmations prevent destructive mistakes
5. **Convenience**: Links to console logs when more detail needed

---

**Visual Guide Version 1.0** | October 2, 2025
