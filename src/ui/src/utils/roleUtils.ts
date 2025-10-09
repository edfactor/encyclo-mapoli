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
 * Validates if a set of impersonation roles can be selected together.
 * Business rules:
 * - By default, only one role can be selected at a time
 * - Exception: Executive-Administrator can be combined with Finance-Manager,
 *   Distributions-Clerk, Hardship-Administrator, or System-Administrator
 *
 * @param currentRoles - Currently selected roles
 * @param newRole - New role being added
 * @returns Array of valid roles after applying validation rules
 */
export function validateImpersonationRoles(
  currentRoles: ImpersonationRoles[],
  newRole: ImpersonationRoles
): ImpersonationRoles[] {
  // If no current roles, just return the new role
  if (currentRoles.length === 0) {
    return [newRole];
  }

  // Check if the new role is Executive-Administrator
  const isAddingExecutiveAdmin = newRole === ImpersonationRoles.ExecutiveAdministrator;

  // Check if current roles contain Executive-Administrator
  const hasExecutiveAdmin = currentRoles.includes(ImpersonationRoles.ExecutiveAdministrator);

  // Check if new role is combinable with Executive-Administrator
  const isNewRoleCombinable = COMBINABLE_WITH_EXECUTIVE_ADMIN.includes(newRole);

  // Check if all current roles (excluding Executive-Administrator) are combinable
  const currentCombinableRoles = currentRoles.filter((role) => role !== ImpersonationRoles.ExecutiveAdministrator);
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

  // Case 3: Any other scenario - replace all roles with the new role
  return [newRole];
}

/**
 * Validates removal of a role from the current selection.
 * Ensures that when Executive-Administrator is removed, only one of the remaining
 * combinable roles stays selected (if multiple exist).
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
