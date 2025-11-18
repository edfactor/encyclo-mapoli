/**
 * Security utility for validating and sanitizing URL paths
 * Prevents open redirect attacks by validating paths against known routes
 */

import { ROUTES } from "../constants";

/**
 * Generates a set of all valid application routes
 * Used to whitelist paths for navigation
 */
const getValidRoutes = (): Set<string> => {
  const routeValues = Object.values(ROUTES);
  const validRoutes = new Set<string>();

  // Add all base routes
  routeValues.forEach((route) => {
    validRoutes.add(`/${route}`);
  });

  // Add special root routes
  validRoutes.add("/");

  return validRoutes;
};

/**
 * Validates that a path is safe for navigation
 * Checks for:
 * - Proper format (starts with /, no protocol, no backslashes, no parent directory traversal)
 * - Is either a known base route or a parameterized route
 *
 * @param path - The path to validate
 * @returns true if path is safe, false otherwise
 */
export const isSafePath = (path: string | undefined | null): boolean => {
  // Reject null/undefined/empty paths
  if (!path || typeof path !== "string" || path.trim().length === 0) {
    return false;
  }

  // Basic format validation
  const hasValidFormat =
    path.startsWith("/") && // Must start with /
    !path.startsWith("//") && // No protocol-like pattern
    !path.includes("://") && // No protocol
    !path.includes("\\") && // No backslashes
    !path.includes("..") && // No parent directory traversal
    !/(%2f|%2F)/.test(path) && // No encoded /
    !/(%5c|%5C)/.test(path); // No encoded \

  if (!hasValidFormat) {
    return false;
  }

  const validRoutes = getValidRoutes();

  // Check if exact match or parameterized route
  if (validRoutes.has(path)) {
    return true;
  }

  // Allow parameterized routes by checking if path starts with a valid base route
  // e.g., /add-distribution/{id}/{type} is valid if /add-distribution is valid
  for (const route of validRoutes) {
    if (route === "/" || route === "") continue; // Skip root for this check

    if (path.startsWith(route + "/")) {
      // Validate parameters are safe (alphanumeric, -, _)
      const paramPart = path.substring(route.length);
      if (/^(\/[a-zA-Z0-9\-_]+)*$/.test(paramPart)) {
        return true;
      }
    }
  }

  return false;
};

/**
 * Sanitizes a path by validating it, returning the path if valid or "/" if invalid
 * This ensures navigation always goes to a safe location
 *
 * @param path - The path to sanitize
 * @returns The validated path or "/" if invalid
 */
export const sanitizePath = (path: string | undefined | null): string => {
  return isSafePath(path) ? (path as string) : "/";
};

/**
 * Encodes path parameters to prevent injection attacks
 * Use this when building URLs with user input
 *
 * @param param - The parameter value to encode
 * @returns URL-safe encoded parameter
 */
export const encodePathParameter = (param: string): string => {
  // Validate parameter is reasonable length (prevent DOS)
  if (!param || param.length > 255) {
    return "";
  }

  // Only allow alphanumeric, hyphens, and underscores
  // Remove any special characters
  return param.replace(/[^a-zA-Z0-9\-_]/g, "");
};

/**
 * Validates route parameters against expected format
 * Useful for validating params from useParams hooks
 *
 * @param memberId - Member identifier (badge or SSN)
 * @param memberType - Member type parameter
 * @returns true if parameters are valid
 */
export const isValidRouteParams = (memberId: string | undefined, memberType: string | undefined): boolean => {
  if (!memberId || !memberType) {
    return false;
  }

  // memberId should be numeric (badge) or 9 digits (SSN)
  const isValidMemberId = /^\d+$/.test(memberId) || /^\d{3}-?\d{2}-?\d{4}$/.test(memberId);

  // memberType should be single word (alphabetic)
  const isValidMemberType = /^[a-zA-Z]+$/.test(memberType);

  return isValidMemberId && isValidMemberType;
};
