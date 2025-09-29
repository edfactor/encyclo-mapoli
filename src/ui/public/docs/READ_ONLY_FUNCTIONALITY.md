# Read-Only Functionality Documentation

## Overview

The profit-sharing application implements comprehensive read-only restrictions for users with ITDEVOPS and AUDITOR roles. This system ensures data integrity while maintaining full audit visibility by disabling modification actions while preserving all search, filter, and viewing capabilities.

## Architecture

### Backend Components

#### NavigationService
- **Location**: `src/services/src/Demoulas.ProfitSharing.Navigation/Services/NavigationService.cs`
- **Purpose**: Determines user read-only status based on role assignments
- **Key Method**: `GetNavigationAsync()` - Returns navigation data including `IsReadOnly` flag
- **Database Integration**: Queries `NavigationRoles` table to check role permissions

#### Database Schema
- **Table**: `NavigationRoles`
- **Key Field**: `IsReadOnly` (boolean) - Flags roles as read-only
- **Read-Only Roles**: 
  - `ITDEVOPS` - IT operations with audit access
  - `AUDITOR` - Compliance auditing role

### Frontend Components

#### useReadOnlyNavigation Hook
- **Location**: `src/ui/src/hooks/useReadOnlyNavigation.ts`
- **Purpose**: Centralized read-only state detection for React components
- **Implementation**:
  ```typescript
  export const useReadOnlyNavigation = (): boolean => {
    const { data: navigationData } = useGetNavigationQuery();
    return navigationData?.isReadOnly ?? false;
  };
  ```
- **Usage**: Import and call in components that need read-only behavior

## Implementation Patterns

### Standard Implementation Pattern

All read-only implementations follow this consistent pattern:

```typescript
// 1. Import the hook
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";

// 2. Use the hook in component
const MyComponent = () => {
  const isReadOnly = useReadOnlyNavigation();
  
  // 3. Implement read-only logic with tooltip
  const actionButton = (
    <Button
      disabled={isReadOnly}
      onClick={isReadOnly ? undefined : handleAction}>
      Action
    </Button>
  );

  // 4. Wrap with tooltip when disabled
  if (isReadOnly) {
    return (
      <Tooltip title="You are in read-only mode and cannot perform this action.">
        <span>{actionButton}</span>
      </Tooltip>
    );
  }
  
  return actionButton;
};
```

### Key Implementation Rules

1. **Always wrap disabled buttons in `<span>`** - Required for Material-UI tooltip functionality
2. **Use descriptive tooltip messages** - Explain why the action is unavailable
3. **Disable click handlers** - Set `onClick={isReadOnly ? undefined : handler}`
4. **Preserve visual state** - Maintain button appearance with disabled styling
5. **Keep search/filter active** - Only disable modification actions

## Implemented Pages

### 1. UnForfeit (PREVPROF)
**Files**: 
- `src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitGrid.tsx`
- `src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitDetailsGridColumns.tsx`

**Restricted Actions**:
- Save individual forfeiture adjustments
- Bulk save operations
- Row selection for modifications

### 2. Termination 
**Files**:
- `src/ui/src/pages/DecemberActivities/Termination/TerminationGrid.tsx`
- `src/ui/src/pages/DecemberActivities/Termination/TerminationDetailsGridColumns.tsx`

**Restricted Actions**:
- Save suggested forfeit amounts
- Bulk save operations
- Edit forfeit values

### 3. Executive Hours and Dollars (TPR008-09)
**Files**:
- `src/ui/src/pages/FiscalClose/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars.tsx`
- `src/ui/src/pages/FiscalClose/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollarsGrid.tsx`
- `src/ui/src/pages/FiscalClose/ManageExecutiveHoursAndDollars/SearchAndAddExecutive.tsx`

**Restricted Actions**:
- Save changes to hours/dollars
- Add new executives
- Add executives from search modal

### 4. Military Contributions
**Files**:
- `src/ui/src/pages/DecemberActivities/MilitaryEntryAndModification/MilitaryEntryAndModification.tsx`

**Restricted Actions**:
- Add new military contributions
- Open contribution dialog

### 5. Master Update (PAY444|PAY447)
**Files**:
- `src/ui/src/pages/ProfitShareEditUpdate/ProfitShareEditUpdate.tsx`

**Restricted Actions**:
- Apply changes
- Revert changes

### 6. Update Summary (PAY450)
**Files**:
- `src/ui/src/pages/PaymasterUpdate/Pay450Summary.tsx`

**Restricted Actions**:
- Update enrollment records

### 7. Forfeitures Adjustment (008-12)
**Files**:
- `src/ui/src/pages/ForfeituresAdjustment/ForfeituresAdjustment.tsx`
- `src/ui/src/pages/ForfeituresAdjustment/ForfeituresAdjustmentPanel.tsx`

**Restricted Actions**:
- Add new forfeitures

## User Experience

### Read-Only User Experience
1. **Visual Feedback**: Buttons appear disabled with appropriate styling
2. **Informative Tooltips**: Hover over disabled buttons shows explanatory message
3. **Preserved Functionality**: All viewing, searching, and filtering remains active
4. **Consistent Messaging**: Standardized tooltip text across all pages

### Standard Tooltip Messages
- **General**: "You are in read-only mode and cannot [perform action]."
- **Save Operations**: "You are in read-only mode and cannot save changes."
- **Add Operations**: "You are in read-only mode and cannot add [items]."
- **Bulk Operations**: "You are in read-only mode and cannot perform bulk operations."

## Testing Guidelines

### Manual Testing Checklist
1. **Role Assignment**: Verify ITDEVOPS/AUDITOR roles trigger read-only mode
2. **Button States**: Confirm all modification buttons are disabled
3. **Tooltip Display**: Verify tooltips appear on hover with correct messages
4. **Click Prevention**: Ensure disabled buttons don't trigger actions
5. **Search/Filter**: Confirm viewing functionality remains active
6. **Visual Consistency**: Check disabled button styling across all pages

### Test User Accounts
- **ITDEVOPS Role**: For IT operations testing
- **AUDITOR Role**: For compliance audit testing
- **Standard User**: For comparison testing (full access)

## Security Considerations

### Defense in Depth
1. **Frontend Restrictions**: UI buttons disabled and click handlers removed
2. **Backend Validation**: API endpoints should validate user permissions
3. **Database Constraints**: Role-based access controls at data layer

### Audit Trail
- All read-only access is logged for compliance
- User actions (even blocked ones) are tracked
- Role assignments are auditable

## Maintenance

### Adding Read-Only to New Pages
1. Import `useReadOnlyNavigation` hook
2. Add hook call: `const isReadOnly = useReadOnlyNavigation();`
3. Identify modification actions (save, add, edit, delete)
4. Apply standard implementation pattern to each action
5. Test with read-only user roles
6. Update this documentation

### Modifying Existing Read-Only Pages
1. Follow existing patterns in the file
2. Maintain consistent tooltip messaging
3. Ensure proper button wrapping for tooltips
4. Test both read-only and normal user experiences

### Common Pitfalls
- **Forgetting span wrapper**: Tooltips won't work on disabled Material-UI buttons
- **Inconsistent messaging**: Use standard tooltip text patterns
- **Missing click handler disabling**: Set to `undefined` in read-only mode
- **Breaking search functionality**: Only disable modification actions

## Configuration

### Environment Variables
- Read-only behavior is determined by user role assignment
- No additional configuration flags required
- Roles are managed through the NavigationRoles database table

### Database Configuration
```sql
-- Example role configuration
INSERT INTO NavigationRoles (RoleName, IsReadOnly, Description)
VALUES 
  ('ITDEVOPS', 1, 'IT operations with read-only audit access'),
  ('AUDITOR', 1, 'Compliance auditing role with read-only access');
```

## Troubleshooting

### Common Issues
1. **Hook not working**: Verify `useReadOnlyNavigation` import and call
2. **Tooltip not showing**: Ensure disabled button is wrapped in `<span>`
3. **Button still clickable**: Check `onClick={isReadOnly ? undefined : handler}`
4. **Inconsistent behavior**: Verify role assignment in NavigationRoles table

### Debug Steps
1. Check browser network tab for navigation API response
2. Verify `isReadOnly` value in component using React DevTools
3. Confirm user role assignment in database
4. Test with different user accounts

## Related Documentation
- [TELEMETRY_GUIDE.md](./TELEMETRY_GUIDE.md) - Comprehensive telemetry implementation
- [TELEMETRY_QUICK_REFERENCE.md](./TELEMETRY_QUICK_REFERENCE.md) - Developer cheat sheet
- [Navigation Setup](../templates/Navigation-Setup.md) - Navigation system setup
- [Copilot Instructions](../.github/copilot-instructions.md) - Development guidelines

---

*Last Updated: September 26, 2025*  
*Version: 1.0*  
*Maintainer: Development Team*