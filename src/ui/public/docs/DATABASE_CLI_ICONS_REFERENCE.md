# Database CLI Command Icons Reference

## Updated Command Icons

All Database CLI commands now have distinctive icons that visually represent their function:

| Command                        | Icon | Icon Name                  | Description                                                       |
| ------------------------------ | ---- | -------------------------- | ----------------------------------------------------------------- |
| **Upgrade Database**           | ⬆️   | `ArrowUp` (Filled)         | Upward arrow representing database schema upgrades and migrations |
| **Drop and Recreate Database** | ⚠️🗄️ | `DatabaseWarning` (Filled) | Database with warning symbol for destructive operation            |
| **Import from READY**          | ⬇️   | `ArrowDownload` (Filled)   | Download arrow representing data import from external system      |
| **Import Navigation**          | 🧭   | `Navigation` (Filled)      | Navigation compass representing navigation data import            |
| **Nuclear Option**             | 🔥   | `Fire` (Filled)            | Fire icon representing the most destructive operation! 💥         |

## Visual Preview in Aspire Dashboard

When you run `aspire run`, you'll see these icons in the Database-Cli resource commands:

```
Database-Cli
├─ ⬆️  Upgrade database
├─ ⚠️  Drop and recreate database
├─ ⬇️  Import from READY
├─ 🧭  Import from navigation
└─ 🔥  Full Nuclear Reset
```

## Icon Rationale

### Upgrade Database - `ArrowUp` 🎯

- **Why**: Represents moving "up" to a newer database version
- **Meaning**: Non-destructive, progressive operation
- **User Psychology**: Positive upward motion = improvement

### Drop and Recreate - `DatabaseWarning` ⚠️

- **Why**: Combines database symbol with warning indicator
- **Meaning**: Destructive database operation requiring caution
- **User Psychology**: Warning symbol makes users think twice

### Import from READY - `ArrowDownload` 📥

- **Why**: Classic download/import arrow
- **Meaning**: Data flowing into the system from external source
- **User Psychology**: Familiar import metaphor

### Import Navigation - `Navigation` 🧭

- **Why**: Navigation compass directly represents the data type
- **Meaning**: Importing navigation/menu structure
- **User Psychology**: Clear semantic connection to "navigation"

### Nuclear Option - `Fire` 🔥💥

- **Why**: NUCLEAR = FIRE = DANGER = TOTAL DESTRUCTION!
- **Meaning**: Most extreme, irreversible, destructive operation
- **User Psychology**: Fire universally signals danger and complete destruction
- **Fun Factor**: You asked for a nuclear explosion, and fire is as close as Fluent UI gets! 😄

## Alternative Icons Considered

If you want to experiment with different icons, here are some alternatives from the Fluent UI library:

### For Nuclear Option:

- `Fire` 🔥 (chosen - most dramatic!)
- `Warning` ⚠️ (too subtle)
- `Delete` 🗑️ (not dramatic enough)
- `ErrorCircle` ⭕ (error, not destruction)
- `BombSolid` 💣 (if it existed - perfect!)

### For Drop-Recreate:

- `DatabaseWarning` ⚠️🗄️ (chosen)
- `Delete` 🗑️ (too generic)
- `Refresh` 🔄 (misleading - suggests safe operation)

### For Upgrade:

- `ArrowUp` ⬆️ (chosen)
- `ArrowUpload` ⬆️📤 (similar but implies external upload)
- `DatabaseArrowUp` (perfect if it existed!)

### For Imports:

- `ArrowDownload` ⬇️ (chosen for READY)
- `ArrowImport` ⬇️📥 (alternative)
- `Navigation` 🧭 (chosen for navigation data)
- `Location` 📍 (alternative for navigation)

## Fluent UI Icon Resources

- **Icon Explorer**: https://storybooks.fluentui.dev/react/?path=/docs/icons-catalog--docs
- **Search All Icons**: Type to filter over 2,000+ available icons
- **Icon Variants**:
  - `Regular` - Outline style (lighter visual weight)
  - `Filled` - Solid style (more prominent, chosen for all commands)

## Implementation Details

All icons use the `Filled` variant for consistency and visual prominence in the dashboard UI.

```csharp
// Example: Nuclear Option with Fire icon
commandOptions: new CommandOptions
{
    IconName = "Fire",
    IconVariant = IconVariant.Filled
}
```

## Testing the New Icons

1. **Start Aspire Dashboard**:

   ```bash
   aspire run
   ```

2. **Navigate to Database-Cli resource**

3. **Observe the icons** next to each command name

4. **Hover over commands** to see the icons in the UI

The icons make it much easier to:

- Quickly identify commands at a glance
- Understand command severity/danger level
- Distinguish between similar operations
- Make the UI more visually appealing

## Future Icon Enhancements

Potential improvements:

- Add color coding (if Aspire supports it)
- Animated icons for running operations
- Different icon states (idle vs. running)
- Custom SVG icons for even more specific metaphors

---

**Icon Guide Version 1.0** | October 2, 2025  
**All icons use Fluent UI icon library** | Filled variant for prominence
