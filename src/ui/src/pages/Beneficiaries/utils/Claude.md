# Beneficiaries Utility Functions

This directory contains pure utility functions for the Beneficiaries page, extracted for reusability, testability, and maintainability.

## Files

### `badgeUtils.ts`

Pure utility functions for badge and PSN (Personnel Serial Number) handling.

**Functions:**

- `parseBadgeAndPSN(badgeInput)` - Separates combined badge/PSN into components

  - Badges: 1-7 digits
  - PSNs: 8+ digits (badge + suffix)

- `detectMemberTypeFromBadge(badge)` - Determines member type by badge length

  - Returns: 0 (All), 1 (Employees), 2 (Beneficiaries)

- `isValidBadgeIdentifiers(badgeNumber, psnSuffix)` - Validates required badge fields

  - Checks badgeNumber is not null/zero and psnSuffix is not null (can be 0)

- `decomposePSNSuffix(psnSuffix)` - Breaks down PSN into three beneficiary levels
  - Returns: { firstLevel, secondLevel, thirdLevel }

**Test Coverage:** 58 tests covering:

- Single/multi-digit parsing
- Boundary conditions (7 vs 8 digits)
- Type detection logic
- Null/undefined handling
- Edge cases (0, NaN, large numbers)
- Real-world examples

### `percentageUtils.ts`

Pure utility functions for percentage allocation validation (100% constraint).

**Functions:**

- `calculatePercentageSum(items, updatedId, newPercentage)` - Computes new total when one item changes

  - Returns: { sum, previousValue } for UI restoration on failure

- `validatePercentageAllocation(sum)` - Validates sum does not exceed 100%
  - Returns: { sum, valid, error? }

**Test Coverage:** 39 tests covering:

- Multi-item allocations
- Exact 100%, under, and over scenarios
- Decimal percentages
- Empty/single item lists
- Duplicate IDs
- Integration scenarios

## Usage

```typescript
import { parseBadgeAndPSN, decomposePSNSuffix, calculatePercentageSum, validatePercentageAllocation } from "./utils";

// Parse badge/PSN
const { badge, psn } = parseBadgeAndPSN("12345678"); // { badge: 1234567, psn: 8 }

// Decompose PSN
const { firstLevel, secondLevel, thirdLevel } = decomposePSNSuffix(234);

// Validate percentages
const { sum, previousValue } = calculatePercentageSum(items, id, 60);
const validation = validatePercentageAllocation(sum);
if (!validation.valid) {
  // Restore previous value on UI
  currentField.value = previousValue;
}
```

## Components Using These Utils

- `BeneficiaryInquirySearchFilter.tsx` - Badge parsing for search
- `CreateBeneficiary.tsx` - PSN decomposition for beneficiary creation
- `useBeneficiaryPercentageUpdate.ts` - Percentage validation in update hook

## Testing

Run tests:

```bash
npm test -- badgeUtils.test.ts percentageUtils.test.ts
```

Coverage: 100% lines, statements, and branches for both utility files
