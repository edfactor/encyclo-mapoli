# Badge Column Visual Link Styling

## Date: October 17, 2025

## Requirement

**User Feedback**: "We need to keep the badge/psn number LOOKing like a link so the user knows what to click on"

## Solution

Even though the badge column doesn't render as an actual link (to prevent navigation), it should still **look** like a link for UX purposes.

### Implementation

Custom cell renderer that applies link-like styling:

```tsx
// MasterInquiryMemberGridColumns.tsx
const badgeColumn = createBadgeColumn({
  psnSuffix: true,
  renderAsLink: false, // Don't navigate
});

// Make it look like a link
badgeColumn.cellRenderer = (params: ICellRendererParams) => {
  const badgeNumber = params.data?.badgeNumber;
  const psnSuffix = params.data?.psnSuffix;

  if (!badgeNumber || badgeNumber === 0) return "";

  const displayValue = psnSuffix ? `${badgeNumber}${psnSuffix}` : badgeNumber;

  return `<span style="color: #1976d2; text-decoration: underline; cursor: pointer; font-weight: 500;">${displayValue}</span>`;
};
```

### Styling Details

- **Color**: `#1976d2` (MUI primary blue - standard link color)
- **Text Decoration**: `underline` (makes it look clickable)
- **Cursor**: `pointer` (hand cursor on hover)
- **Font Weight**: `500` (medium weight for emphasis)

### User Experience

**Visual Appearance:**

```
Badge      Name              SSN
123456     John Smith        ***-**-1234  ← Blue, underlined, looks clickable
234567     Jane Doe          ***-**-5678  ← Blue, underlined, looks clickable
```

**Behavior:**

- ✅ Looks like a link (blue, underlined)
- ✅ Cursor changes to pointer on hover
- ✅ Clicking it selects the member (row click)
- ✅ Does NOT navigate/redirect
- ✅ No URL change

### Benefits

1. **Visual Affordance** - Users know where to click
2. **Familiar Pattern** - Blue underlined text = clickable
3. **Consistent UX** - Matches web standards
4. **No Navigation** - Still prevents the redirect issue
5. **Professional Look** - Polished, intentional design

### Comparison

| Aspect              | Before Fix | After Fix (No Style) | After Fix (With Style) |
| ------------------- | ---------- | -------------------- | ---------------------- |
| Looks Like Link     | ✅ Yes     | ❌ No                | ✅ Yes                 |
| Causes Navigation   | ❌ Yes     | ✅ No                | ✅ No                  |
| User Knows to Click | ✅ Yes     | ❌ No                | ✅ Yes                 |
| Professional UX     | Partial    | Partial              | ✅ Yes                 |

## Testing

### Visual Test

```
1. Open Master Inquiry
2. Search for members
3. Look at badge column:
   - Should be blue ✓
   - Should be underlined ✓
   - Should look clickable ✓
4. Hover over badge:
   - Cursor should change to pointer ✓
5. Click badge:
   - Should select member ✓
   - Should NOT navigate ✓
```

### Accessibility

- Screen readers will still announce badge numbers
- Keyboard navigation works (row selection)
- Visual indication matches behavior (clickable but not a link)

## Files Modified

- `src/ui/src/pages/MasterInquiry/MasterInquiryMemberGridColumns.tsx`

## Related Fixes

This complements:

- **renderAsLink: false** - Prevents navigation
- **Row click handler** - Enables selection
- **Custom styling** - Maintains visual affordance

Together, they create the perfect balance:

- Looks like a link (user knows what to click)
- Acts like selection (no navigation)
- Feels professional (polished UX)

---

**Bottom Line**: Badge numbers look clickable and blue/underlined like links, but clicking them performs in-page selection instead of navigation. Best of both worlds!

**Implementation Date**: October 17, 2025
