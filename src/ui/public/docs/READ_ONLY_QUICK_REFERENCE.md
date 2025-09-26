# Read-Only Implementation Quick Reference

## 🚀 Quick Start

### 1. Basic Implementation (3 steps)
```typescript
// Step 1: Import the hook
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";

// Step 2: Use in component
const isReadOnly = useReadOnlyNavigation();

// Step 3: Apply to buttons
const saveButton = (
  <Button disabled={isReadOnly} onClick={isReadOnly ? undefined : handleSave}>
    Save
  </Button>
);

// Step 4: Add tooltip for disabled state
if (isReadOnly) {
  return (
    <Tooltip title="You are in read-only mode and cannot save changes.">
      <span>{saveButton}</span>
    </Tooltip>
  );
}
return saveButton;
```

## 📋 Checklist

### Implementation Checklist
- [ ] Import `useReadOnlyNavigation` hook
- [ ] Call hook: `const isReadOnly = useReadOnlyNavigation();`  
- [ ] Disable button: `disabled={isReadOnly}`
- [ ] Disable click: `onClick={isReadOnly ? undefined : handler}`
- [ ] Wrap in tooltip: `<Tooltip><span>{button}</span></Tooltip>`
- [ ] Use standard message: "You are in read-only mode and cannot [action]."

### Testing Checklist  
- [ ] Test with ITDEVOPS role (should be read-only)
- [ ] Test with AUDITOR role (should be read-only)
- [ ] Test with normal user (should be editable)
- [ ] Verify tooltips appear on hover
- [ ] Confirm search/filter still works
- [ ] Check button visual state (disabled styling)

## 🎨 Common Patterns

### Simple Button
```typescript
const MyButton = ({ onAction }) => {
  const isReadOnly = useReadOnlyNavigation();
  
  const button = (
    <Button 
      disabled={isReadOnly}
      onClick={isReadOnly ? undefined : onAction}>
      Action
    </Button>
  );
  
  return isReadOnly ? (
    <Tooltip title="You are in read-only mode and cannot perform this action.">
      <span>{button}</span>
    </Tooltip>
  ) : button;
};
```

### Modal/Dialog Trigger
```typescript
const AddButton = ({ onOpenModal }) => {
  const isReadOnly = useReadOnlyNavigation();
  
  return isReadOnly ? (
    <Tooltip title="You are in read-only mode and cannot add items.">
      <span>
        <Button disabled startIcon={<AddOutlined />}>
          Add Item
        </Button>
      </span>
    </Tooltip>
  ) : (
    <Button onClick={onOpenModal} startIcon={<AddOutlined />}>
      Add Item  
    </Button>
  );
};
```

### Conditional Rendering
```typescript
const ActionComponent = () => {
  const isReadOnly = useReadOnlyNavigation();
  
  const handleAction = () => {
    if (isReadOnly) return; // Extra safety check
    // Perform action
  };
  
  // Component implementation...
};
```

### Passing to Child Components
```typescript
// Parent component
const ParentComponent = () => {
  const isReadOnly = useReadOnlyNavigation();
  
  return (
    <ChildComponent 
      data={data}
      onSave={handleSave}
      isReadOnly={isReadOnly}  // Pass down
    />
  );
};

// Child component
interface ChildProps {
  data: any;
  onSave: () => void;
  isReadOnly?: boolean;
}

const ChildComponent = ({ data, onSave, isReadOnly = false }: ChildProps) => {
  // Use isReadOnly prop instead of hook
};
```

## 💬 Standard Messages

### Button Actions
- **Save**: "You are in read-only mode and cannot save changes."
- **Add**: "You are in read-only mode and cannot add [items]."
- **Edit**: "You are in read-only mode and cannot edit [items]."
- **Delete**: "You are in read-only mode and cannot delete [items]."
- **Update**: "You are in read-only mode and cannot update [items]."

### Bulk Operations
- **Bulk Save**: "You are in read-only mode and cannot perform bulk operations."
- **Bulk Add**: "You are in read-only mode and cannot add multiple [items]."

### Specific Actions
- **Add Executive**: "You are in read-only mode and cannot add executives."
- **Add Forfeiture**: "You are in read-only mode and cannot add forfeitures."
- **Military Contribution**: "You are in read-only mode and cannot add military contributions."

## ⚠️ Common Mistakes

### ❌ Wrong - Missing span wrapper
```typescript
// Tooltip won't work on disabled Material-UI button
<Tooltip title="Message">
  <Button disabled>Action</Button>  
</Tooltip>
```

### ✅ Correct - With span wrapper
```typescript
<Tooltip title="Message">
  <span>
    <Button disabled>Action</Button>
  </span>
</Tooltip>
```

### ❌ Wrong - Click handler still active
```typescript
<Button disabled={isReadOnly} onClick={handleAction}>
  Action
</Button>
```

### ✅ Correct - Click handler disabled
```typescript
<Button 
  disabled={isReadOnly} 
  onClick={isReadOnly ? undefined : handleAction}>
  Action  
</Button>
```

### ❌ Wrong - Disabling search/filter
```typescript
// Don't disable viewing functionality
<TextField disabled={isReadOnly} />
<SearchComponent disabled={isReadOnly} />
```

### ✅ Correct - Only disable modifications
```typescript
// Keep search/filter active
<TextField value={searchTerm} onChange={setSearchTerm} />
<Button disabled={isReadOnly} onClick={isReadOnly ? undefined : handleSave}>
  Save Results
</Button>
```

## 🔧 Troubleshooting

### Hook not working?
1. Check import path: `"../../hooks/useReadOnlyNavigation"`
2. Verify hook is called inside component function
3. Check navigation API response in browser DevTools

### Tooltip not appearing?
1. Ensure button is wrapped in `<span>` when disabled
2. Import `Tooltip` from `@mui/material`
3. Check that `title` prop has content

### Button still clickable?
1. Set `onClick={isReadOnly ? undefined : handler}`
2. Add safety check in handler: `if (isReadOnly) return;`

### Inconsistent behavior?
1. Verify user role in NavigationRoles database table
2. Test with different user accounts
3. Check browser network tab for API responses

## 📚 File Locations

### Hook Location
```
src/ui/src/hooks/useReadOnlyNavigation.ts
```

### Example Implementations
```
src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitGrid.tsx
src/ui/src/pages/FiscalClose/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars.tsx
src/ui/src/pages/ForfeituresAdjustment/ForfeituresAdjustmentPanel.tsx
```

### Backend Service
```
src/services/src/Demoulas.ProfitSharing.Navigation/Services/NavigationService.cs
```

---

**Need help?** Check the full documentation in [READ_ONLY_FUNCTIONALITY.md](./READ_ONLY_FUNCTIONALITY.md)