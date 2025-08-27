import * as constants from "../constants";
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
