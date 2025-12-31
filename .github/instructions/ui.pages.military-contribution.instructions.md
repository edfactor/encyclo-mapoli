---
applyTo: "src/ui/src/pages/DecemberActivities/MilitaryContribution/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/MilitaryContribution/**/*.*"
---

# Military Contribution

## Purpose

Allows authorized users to search for employees and manage their military contribution records as part of December year-end activities. Supports:

- Searching employees by SSN or Badge Number
- Viewing existing military contributions
- Adding new regular or supplemental contributions
- Read-only mode and frozen year protection

## Key Components

| File                                     | Responsibility                                      |
| ---------------------------------------- | --------------------------------------------------- |
| `MilitaryContribution.tsx`               | Page container with dialog management               |
| `MilitaryContributionSearchFilter.tsx`   | Employee search form (SSN/Badge mutually exclusive) |
| `MilitaryContributionFormGrid.tsx`       | Contributions grid with member details              |
| `MilitaryContributionFormGridColumns.ts` | AG Grid column definitions                          |
| `MilitaryContributionForm.tsx`           | Add contribution dialog form                        |
| `hooks/useMilitaryContribution.ts`       | State and API orchestration hook                    |

## State Management

### Local State (useReducer)

```typescript
interface MilitaryState {
  search: { isSearching; searchCompleted; memberFound; searchParams; error };
  contributions: { data; isLoading; error };
}
```

### Redux State

- `inquirySlice`: Stores selected member details
- `militarySlice`: Caches fetched contributions data

## API Endpoints

| Endpoint                             | Method | Purpose                            |
| ------------------------------------ | ------ | ---------------------------------- |
| `/api/military`                      | GET    | Fetch contributions for badge/year |
| `/api/military`                      | POST   | Create new contribution            |
| `/api/inquiry/master-inquiry/search` | GET    | Search for members                 |

## Key Patterns

- Mutual exclusion in search fields
- Immediate contribution fetch after member search
- Backend error mapping for user-friendly messages
- Monthly pay frequency guard (disables add for monthly-paid)
