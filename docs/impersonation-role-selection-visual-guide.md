# Impersonation Role Selection - Visual Guide

## Role Combination Rules

```
┌─────────────────────────────────────────────────────────────────┐
│                    IMPERSONATION ROLES                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  SINGLE SELECTION (Default Behavior)                           │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━                          │
│                                                                 │
│  ┌──────────────┐                                              │
│  │   Auditor    │  ──▶  Only one can be selected              │
│  └──────────────┘                                              │
│                                                                 │
│  ┌──────────────┐                                              │
│  │  IT-DevOps   │  ──▶  Selecting another replaces this       │
│  └──────────────┘                                              │
│                                                                 │
│  ┌──────────────┐                                              │
│  │ IT-Operations│                                              │
│  └──────────────┘                                              │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  COMBINATION ALLOWED (Exception)                               │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━                            │
│                                                                 │
│  ┌──────────────────────┐                                      │
│  │ Executive-           │  ◀──▶  Can combine with:            │
│  │ Administrator        │                                      │
│  └──────────────────────┘                                      │
│            ║                                                    │
│            ╠═══▶ Finance-Manager                               │
│            ║                                                    │
│            ╠═══▶ Distributions-Clerk                           │
│            ║                                                    │
│            ╠═══▶ Hardship-Administrator                        │
│            ║                                                    │
│            ╚═══▶ System-Administrator                          │
│                  (ProfitSharingAdministrator)                  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Selection Flow Examples

### Example 1: Single Role Selection
```
Step 1: Select "Auditor"
┌──────────────┐
│  ✓ Auditor   │
└──────────────┘

Step 2: Select "IT-DevOps"
┌──────────────┐     ┌──────────────┐
│    Auditor   │ ──▶ │ ✓ IT-DevOps  │  (Auditor auto-deselected)
└──────────────┘     └──────────────┘
```

### Example 2: Combining with Executive-Administrator
```
Step 1: Select "Finance-Manager"
┌─────────────────────┐
│ ✓ Finance-Manager   │
└─────────────────────┘

Step 2: Add "Executive-Administrator"
┌─────────────────────┐     ┌────────────────────────┐
│ ✓ Finance-Manager   │ ──▶ │ ✓ Finance-Manager      │
└─────────────────────┘     │ ✓ Executive-Admin      │
                            └────────────────────────┘

Step 3: Can add more combinable roles
┌────────────────────────┐     ┌────────────────────────┐
│ ✓ Finance-Manager      │     │ ✓ Finance-Manager      │
│ ✓ Executive-Admin      │ ──▶ │ ✓ Executive-Admin      │
└────────────────────────┘     │ ✓ Distributions-Clerk  │
                               └────────────────────────┘
```

### Example 3: Breaking a Combination
```
Current State:
┌────────────────────────┐
│ ✓ Finance-Manager      │
│ ✓ Executive-Admin      │
└────────────────────────┘

Select "Auditor" (non-combinable):
┌────────────────────────┐     ┌──────────────┐
│   Finance-Manager      │     │              │
│   Executive-Admin      │ ──▶ │ ✓ Auditor    │  (All previous deselected)
└────────────────────────┘     └──────────────┘
```

### Example 4: Removing Executive-Administrator
```
Current State:
┌────────────────────────┐
│ ✓ Executive-Admin      │
│ ✓ Finance-Manager      │
│ ✓ Distributions-Clerk  │
└────────────────────────┘

Deselect "Executive-Administrator":
┌────────────────────────┐     ┌─────────────────────┐
│   Executive-Admin      │     │                     │
│ ✓ Finance-Manager      │ ──▶ │ ✓ Finance-Manager   │  (Only first kept)
│   Distributions-Clerk  │     │                     │
└────────────────────────┘     └─────────────────────┘
```

## Role Matrix

| Adding Role ↓ / Current Role → | None | Auditor | IT-DevOps | Finance-Mgr | Exec-Admin | Exec-Admin + Finance-Mgr |
|--------------------------------|------|---------|-----------|-------------|------------|--------------------------|
| **Auditor**                    | ✓    | -       | Replace   | Replace     | Replace    | Replace all              |
| **IT-DevOps**                  | ✓    | Replace | -         | Replace     | Replace    | Replace all              |
| **IT-Operations**              | ✓    | Replace | Replace   | Replace     | Replace    | Replace all              |
| **Finance-Manager**            | ✓    | Replace | Replace   | -           | Add        | Already selected         |
| **Distributions-Clerk**        | ✓    | Replace | Replace   | Replace     | Add        | Add                      |
| **Hardship-Admin**             | ✓    | Replace | Replace   | Replace     | Add        | Add                      |
| **System-Admin**               | ✓    | Replace | Replace   | Replace     | Add        | Add                      |
| **Executive-Admin**            | ✓    | Replace | Replace   | Add         | -          | Already selected         |

**Legend:**
- ✓ = Allow selection
- Replace = Deselect current, select new
- Add = Add to current selection
- Replace all = Deselect all current, select new

## Decision Tree

```
                    User selects a role
                           │
                           ▼
              ┌────────────────────────┐
              │ Any roles currently    │
              │ selected?              │
              └────────────────────────┘
                     │            │
                   NO│            │YES
                     │            │
                     ▼            ▼
              ┌──────────┐   ┌─────────────────────────┐
              │ Select   │   │ Is new role             │
              │ the role │   │ Executive-Administrator? │
              └──────────┘   └─────────────────────────┘
                                  │              │
                                YES│             │NO
                                  │              │
                                  ▼              ▼
                    ┌──────────────────────┐  ┌────────────────────────┐
                    │ Are current roles    │  │ Is Executive-Admin     │
                    │ all combinable?      │  │ currently selected?    │
                    └──────────────────────┘  └────────────────────────┘
                         │         │                │              │
                       YES│        │NO            YES│             │NO
                         │         │                │              │
                         ▼         ▼                ▼              ▼
                    ┌────────┐  ┌─────────┐  ┌──────────────┐  ┌─────────┐
                    │Add role│  │Replace  │  │Is new role   │  │Replace  │
                    └────────┘  │all roles│  │combinable?   │  │all roles│
                                └─────────┘  └──────────────┘  └─────────┘
                                                  │        │
                                                YES│       │NO
                                                  │        │
                                                  ▼        ▼
                                            ┌────────┐  ┌─────────┐
                                            │Add role│  │Replace  │
                                            └────────┘  │all roles│
                                                        └─────────┘
```

## Implementation Notes

### Key Functions

1. **validateImpersonationRoles(currentRoles, newRole)**
   - Returns: Validated array of roles after adding newRole

2. **validateRoleRemoval(currentRoles, roleToRemove)**
   - Returns: Validated array of roles after removing roleToRemove

### Constants

```typescript
const COMBINABLE_WITH_EXECUTIVE_ADMIN = [
  ImpersonationRoles.FinanceManager,
  ImpersonationRoles.DistributionsClerk,
  ImpersonationRoles.HardshipAdministrator,
  ImpersonationRoles.ProfitSharingAdministrator
];
```

---

**Last Updated**: October 3, 2025
