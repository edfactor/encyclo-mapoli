import * as constants from "../constants";
import { ImpersonationRoles } from "../types/common/enums";
import EnvironmentUtils from "./environmentUtils";

/**
 * Checks if user has specific roles based on the SMART_{APP}_{ENV}_{Role} or SMART-{APP}-{ENV}-{Role} patterns
 *
 * @param userGroups - Array of user roles/groups to check against
 * @param roleName - The specific role name to check for
 * @returns boolean indicating if user has the specified role
 */
export function checkUserGroupsForRole(userGroups: string[], roleName: string): boolean {
  const currentEnv = EnvironmentUtils.envMode.toUpperCase();

  // Create patterns for both underscore and hyphen formats
  const rolePatternUnderscore = `${constants.SMART_ROLE_PREFIX}_${constants.APP_NAME}_${currentEnv}_${roleName}`;
  const rolePatternHyphen = `${constants.SMART_ROLE_PREFIX}-${constants.APP_NAME}-${currentEnv}-${roleName}`;

  return userGroups.some((group) => group === rolePatternUnderscore || group === rolePatternHyphen);
}

/**
 * Specific function to check for impersonation role
 *
 * @param userGroups - Array of user roles/groups
 * @returns boolean indicating if user has impersonation role
 */
export function checkImpersonationRole(userGroups: string[]): boolean {
  return checkUserGroupsForRole(userGroups, constants.IMPERSONATION_ROLE_SUFFIX);
}

/**
 * Roles that can be combined with Executive-Administrator
 */
const COMBINABLE_WITH_EXECUTIVE_ADMIN = [
  ImpersonationRoles.FinanceManager,
  ImpersonationRoles.DistributionsClerk,
  ImpersonationRoles.HardshipAdministrator,
  ImpersonationRoles.ProfitSharingAdministrator
];

/**
 * Modifier roles that don't drive navigation and can only be combined with specific roles.
 * These roles enhance permissions but cannot be selected independently.
 */
const MODIFIER_ROLES = [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.SsnUnmasking];

/**
 * Roles that SSN-Unmasking can be paired with
 */
const COMBINABLE_WITH_SSN_UNMASKING = [
  ImpersonationRoles.ProfitSharingAdministrator,
  ImpersonationRoles.ExecutiveAdministrator
];

/**
 * Validates if a set of impersonation roles can be selected together.
 * Business rules:
 * - By default, only one role can be selected at a time
 * - Exception: Executive-Administrator can be combined with Finance-Manager,
 *   Distributions-Clerk, Hardship-Administrator, or System-Administrator
 * - Exception: SSN-Unmasking can only be paired with System-Administrator or Executive-Administrator
 *
 * @param currentRoles - Currently selected roles
 * @param newRole - New role being added
 * @returns Array of valid roles after applying validation rules
 */
export function validateImpersonationRoles(
  currentRoles: ImpersonationRoles[],
  newRole: ImpersonationRoles
): ImpersonationRoles[] {
  // If no current roles, cannot add modifier roles independently
  if (currentRoles.length === 0) {
    if (MODIFIER_ROLES.includes(newRole)) {
      return [];
    }
    return [newRole];
  }

  // Check if the new role is Executive-Administrator
  const isAddingExecutiveAdmin = newRole === ImpersonationRoles.ExecutiveAdministrator;

  // Check if the new role is SSN-Unmasking
  const isAddingSsnUnmasking = newRole === ImpersonationRoles.SsnUnmasking;

  // Check if current roles contain Executive-Administrator
  const hasExecutiveAdmin = currentRoles.includes(ImpersonationRoles.ExecutiveAdministrator);

  // Check if current roles contain SSN-Unmasking
  const hasSsnUnmasking = currentRoles.includes(ImpersonationRoles.SsnUnmasking);

  // Check if new role is combinable with Executive-Administrator
  const isNewRoleCombinable = COMBINABLE_WITH_EXECUTIVE_ADMIN.includes(newRole);

  // Check if all current roles (excluding modifier roles) are combinable
  const currentCombinableRoles = currentRoles.filter((role) => !MODIFIER_ROLES.includes(role));
  const areCurrentRolesCombinable = currentCombinableRoles.every((role) =>
    COMBINABLE_WITH_EXECUTIVE_ADMIN.includes(role)
  );

  // Case 1: Adding Executive-Administrator to existing combinable roles
  if (isAddingExecutiveAdmin && areCurrentRolesCombinable) {
    return [...currentRoles, newRole];
  }

  // Case 2: Adding a combinable role when Executive-Administrator is already selected
  if (hasExecutiveAdmin && isNewRoleCombinable) {
    return [...currentRoles, newRole];
  }

  // Case 3: Adding SSN-Unmasking to an allowed role (ProfitSharingAdministrator or ExecutiveAdministrator)
  if (isAddingSsnUnmasking && !hasSsnUnmasking) {
    const baseRole = currentRoles.filter((role) => !MODIFIER_ROLES.includes(role))[0];
    if (baseRole && COMBINABLE_WITH_SSN_UNMASKING.includes(baseRole)) {
      return [...currentRoles, newRole];
    }
    // If trying to add SSN-Unmasking to incompatible role, replace with SSN-Unmasking only (invalid state, handled by UI)
    return [];
  }

  // Case 4: If SSN-Unmasking is already selected and adding a new base role
  if (hasSsnUnmasking && !MODIFIER_ROLES.includes(newRole)) {
    if (COMBINABLE_WITH_SSN_UNMASKING.includes(newRole)) {
      return [newRole, ImpersonationRoles.SsnUnmasking];
    }
    // Replace if incompatible with SSN-Unmasking
    return [newRole];
  }

  // Case 5: Any other scenario - replace all roles with the new role
  return [newRole];
}

/**
 * Validates removal of a role from the current selection.
 * Ensures that when Executive-Administrator is removed, only one of the remaining
 * combinable roles stays selected (if multiple exist).
 * If the base role is removed while SSN-Unmasking is selected, removes SSN-Unmasking as well
 * unless it can be paired with ExecutiveAdministrator.
 *
 * @param currentRoles - Currently selected roles
 * @param roleToRemove - Role being removed
 * @returns Array of valid roles after removal
 */
export function validateRoleRemoval(
  currentRoles: ImpersonationRoles[],
  roleToRemove: ImpersonationRoles
): ImpersonationRoles[] {
  const remainingRoles = currentRoles.filter((role) => role !== roleToRemove);
  const hasSsnUnmasking = remainingRoles.includes(ImpersonationRoles.SsnUnmasking);
  const hasExecutiveAdmin = remainingRoles.includes(ImpersonationRoles.ExecutiveAdministrator);

  // If removing a base role (non-modifier) and SSN-Unmasking is selected
  if (!MODIFIER_ROLES.includes(roleToRemove) && hasSsnUnmasking) {
    // If Executive-Administrator remains, SSN-Unmasking can stay (it's compatible)
    if (hasExecutiveAdmin) {
      return remainingRoles;
    }
    // Otherwise, remove SSN-Unmasking since it has no compatible base role
    return remainingRoles.filter((role) => role !== ImpersonationRoles.SsnUnmasking);
  }

  // If removing Executive-Administrator and SSN-Unmasking is present
  if (roleToRemove === ImpersonationRoles.ExecutiveAdministrator && hasSsnUnmasking) {
    // Get remaining base roles (non-modifier roles)
    const baseRoles = remainingRoles.filter((role) => !MODIFIER_ROLES.includes(role));

    // If no base role is directly compatible with SSN-Unmasking, remove it
    if (baseRoles.length > 0 && !baseRoles.some((role) => COMBINABLE_WITH_SSN_UNMASKING.includes(role))) {
      return remainingRoles.filter((role) => role !== ImpersonationRoles.SsnUnmasking);
    }
  }

  // If removing Executive-Administrator and multiple combinable roles remain,
  // keep only the first combinable role
  if (roleToRemove === ImpersonationRoles.ExecutiveAdministrator && remainingRoles.length > 1) {
    const combinableRoles = remainingRoles.filter((role) => COMBINABLE_WITH_EXECUTIVE_ADMIN.includes(role));

    if (combinableRoles.length > 1) {
      return [combinableRoles[0]];
    }
  }

  return remainingRoles;
}
