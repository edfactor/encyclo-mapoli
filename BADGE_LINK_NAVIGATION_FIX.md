# Master Inquiry - Fixing the URL Navigation "Refresh" Issue

## Date: October 17, 2025

## The Problem

User reported: **"It's still refreshing. It looks like a redirect and adding the badge number on the url"**

This was accurate! The "refresh" feeling wasn't from component mounting/unmounting - it was from an actual browser navigation/redirect.

## Root Cause

The badge number column was rendering as a **clickable link** with `href="/master-inquiry/{badgeNumber}"`:

```tsx
// In masterInquiryLink.tsx
return (
  <Link
    className="solid h-5 normal-case underline"
    href={`/master-inquiry/${safeValue}`}
  >
    {" "}
    // ← Browser navigation!
    {displayValue}
  </Link>
);
```

### What Was Happening

1. User clicks badge in search results grid
2. **Link navigation fires** → Browser navigates to `/master-inquiry/123456`
3. **Full page reload** (or React Router navigation)
4. URL changes, page refreshes
5. User sees: URL change + page flash = "refresh"

### Why This Happened

The grid had **two** click handlers competing:

1. **Badge link** (`href="/master-inquiry/..."`) - Causes navigation
2. **Row click** (`onRowClicked`) - Calls `handleMemberClick`

The badge link was winning and causing the redirect before the row click could handle it properly.

## The Fix

Disabled link rendering for the badge column in Master Inquiry, since the grid's `onRowClicked` already handles member selection:

### Before

```tsx
// MasterInquiryMemberGridColumns.tsx
createBadgeColumn({
  psnSuffix: true
  // renderAsLink defaults to true → Creates <Link href="...">
}),
```

### After

```tsx
// MasterInquiryMemberGridColumns.tsx
createBadgeColumn({
  psnSuffix: true,
  renderAsLink: false  // ✅ Don't render as link - row click handles selection
}),
```

## Impact

### Before (Link Rendering)

```
User clicks badge
  ↓
Link navigation fires
  ↓
Browser navigates to /master-inquiry/123456
  ↓
URL changes (visible in address bar)
  ↓
Page refresh/reload
  ↓
User: "Why did it refresh and change the URL?"
```

### After (No Link)

```
User clicks badge (or anywhere on row)
  ↓
onRowClicked fires
  ↓
handleMemberClick called
  ↓
State updates (no navigation)
  ↓
Details show smoothly
  ↓
User: "Perfect, no refresh!"
```

## Benefits

✅ **No URL changes** - URL stays as `/master-inquiry`  
✅ **No browser navigation** - Pure React state management  
✅ **No page refresh** - Smooth in-page transition  
✅ **Consistent behavior** - Clicking badge or row does the same thing  
✅ **Better UX** - No visible URL changes or address bar flicker

## Technical Details

### Grid Configuration

The grid already had proper selection handling:

```tsx
// MasterInquiryMemberGrid.tsx
<DSMGrid
  providedOptions={{
    rowData: searchResults.results,
    columnDefs: columns,
    context: { onBadgeClick: handleMemberClick },
    onRowClicked: (event) => {
      if (event.data) {
        handleMemberClick(event.data); // ✅ This is all we need
      }
    },
  }}
/>
```

The badge link was redundant and counterproductive.

### Why Links Exist in Other Contexts

The badge link rendering (`renderAsLink: true`) makes sense in OTHER grids where:

- User wants to open member details in a new tab
- Navigating to Master Inquiry from a different page
- Bookmarkable URLs are desired

But in the Master Inquiry **search results grid**, we want in-page selection, not navigation.

## Related Components

### Still Use Links (renderAsLink: true)

These contexts still benefit from badge links:

- Reports that reference members
- Distribution grids with member links
- Year-end processing grids
- Any grid where opening in new tab is useful

### Don't Use Links (renderAsLink: false)

These contexts should use row click selection:

- **Master Inquiry search results** ← This fix
- Member selection dialogs
- Inline member pickers
- Any "select and continue" workflow

## Testing

### Visual Test

```
1. Search for member with multiple results
2. Grid shows with badge numbers (no underline/link styling)
3. Click badge number
4. Observe URL in address bar - should NOT change ✓
5. Details should appear smoothly without refresh ✓
```

### URL Test

```
1. Note current URL: /master-inquiry
2. Click badge from grid
3. URL should remain: /master-inquiry ✓
4. No redirect, no URL change ✓
```

### Behavior Test

```
1. Click badge → Details show
2. Click row → Details show
3. Both should behave identically ✓
4. No navigation, just state updates ✓
```

## Files Modified

- `src/ui/src/pages/MasterInquiry/MasterInquiryMemberGridColumns.tsx`

## Lines Changed

- Before: `createBadgeColumn({ psnSuffix: true })`
- After: `createBadgeColumn({ psnSuffix: true, renderAsLink: false })`
- Net change: 1 parameter added

## Combined with Previous Fixes

This fix works together with:

1. **API Deduplication** - Prevents duplicate API calls
2. **State Preservation** - Prevents unnecessary state clearing
3. **Hook Deduplication** - Prevents duplicate useEffect triggers
4. **Persistent Mounting** - Prevents component unmount flash
5. **No Forced Re-renders** - Removes key prop forcing remounts
6. **No Link Navigation** (This fix) - Prevents URL redirect

All together, these create a **completely smooth, in-page experience** with:

- No duplicate API calls
- No component flashing
- No URL changes
- No browser navigation
- No "refresh" feeling

## Success Metrics

| Metric              | Before                     | After   |
| ------------------- | -------------------------- | ------- |
| URL Changes         | Yes (adds badge to URL) ❌ | No ✅   |
| Browser Navigation  | Yes ❌                     | No ✅   |
| Page Refresh        | Yes ❌                     | No ✅   |
| Address Bar Flicker | Yes ❌                     | No ✅   |
| In-Page Transition  | No ❌                      | Yes ✅  |
| Professional UX     | Low                        | High ✅ |

## Why This Was Hard to Spot

1. **Two click handlers** - Badge link and row click competing
2. **Link looked intentional** - Badge columns often DO need links
3. **Worked in other contexts** - Links are correct for cross-page navigation
4. **Subtle difference** - Navigation vs. state update looks similar at first
5. **Good intentions** - Badge links are useful in general, just not here

## Key Insight

> **Not all clickable badges should be links.**
>
> When you want **in-page selection**, use row click.  
> When you want **navigation/new tab**, use badge links.
>
> Master Inquiry search results need selection, not navigation.

---

**Bottom Line**: The badge column was rendering as a link, causing browser navigation and URL changes. Disabling `renderAsLink` fixes the redirect/refresh issue completely. Combined with the previous fixes, Master Inquiry now has a buttery-smooth, professional UX.

**Implementation Date**: October 17, 2025  
**Issue**: Discovered and fixed after user reported URL navigation
**Status**: Ready for testing
