import { SMART_PS_QA_IMPERSONATION } from "../constants";
import EnvironmentUtils from "./environmentUtils";

/**
 * Checks if user has specific roles based on the SMART_{APP}_{ENV}_{Role} pattern
 *
 * @param userGroups - Array of user roles/groups to check against
 * @param roleName - The specific role name to check for
 * @returns boolean indicating if user has the specified role
 */
export function checkUserGroupsForRole(userGroups: string[], roleName: string): boolean {
  const prefix = "SMART";
  const app = "PS";
  const currentEnv = EnvironmentUtils.envMode.toUpperCase();
  const rolePattern = `${prefix}_${app}_${currentEnv}_${roleName}`;

  return userGroups.some((group) => group === rolePattern);
}

/**
 * Specific function to check for impersonation role
 *
 * @param userGroups - Array of user roles/groups
 * @returns boolean indicating if user has impersonation role
 */
export function checkImpersonationRole(userGroups: string[]): boolean {
  // Legacy check
  if (userGroups.includes(SMART_PS_QA_IMPERSONATION)) {
    return true;
  }

  // New format check
  return checkUserGroupsForRole(userGroups, "Impersonation");
}
