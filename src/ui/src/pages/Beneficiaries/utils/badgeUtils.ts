/**
 * Pure utility functions for badge and PSN handling
 */

interface BadgeParsing {
  badge: number;
  psn?: number;
}

const MAX_BADGE_LENGTH = 8; // 8+ digits is a PSN (badge + suffix)

/**
 * Parses a badge/PSN number string into separate badge and PSN components
 *
 * Badge numbers: 3-7 digits (employee badges)
 * PSN numbers: 8+ digits (badge + suffix)
 *
 * @example
 * parseBadgeAndPSN("1234567") => { badge: 1234567 }
 * parseBadgeAndPSN("12345678") => { badge: 1234567, psn: 8 }
 * parseBadgeAndPSN("123456789") => { badge: 1234567, psn: 89 }
 *
 * @param badgeInput - Badge or PSN number as string or number
 * @returns Object with separated badge and optional PSN suffix
 */
export function parseBadgeAndPSN(badgeInput: string | number): BadgeParsing {
  const badgeStr = String(badgeInput).trim();

  if (badgeStr.length <= MAX_BADGE_LENGTH - 1) {
    return { badge: Number(badgeStr) };
  }

  return {
    badge: parseInt(badgeStr.slice(0, MAX_BADGE_LENGTH - 1), 10),
    psn: parseInt(badgeStr.slice(MAX_BADGE_LENGTH - 1), 10)
  };
}

/**
 * Detects member type based on badge number length
 *
 * Used for filtering search results:
 * - 0 = All (empty or too short to determine)
 * - 1 = Employees (3-7 digits)
 * - 2 = Beneficiaries (8+ digits, has PSN suffix)
 *
 * @example
 * detectMemberTypeFromBadge("") => 0
 * detectMemberTypeFromBadge("123456") => 1
 * detectMemberTypeFromBadge("12345678") => 2
 *
 * @param badge - Badge number as string or number
 * @returns Member type: 0 (All), 1 (Employees), 2 (Beneficiaries)
 */
export function detectMemberTypeFromBadge(badge: string | number): 0 | 1 | 2 {
  const badgeStr = String(badge).trim();

  if (badgeStr.length === 0) return 0; // All
  if (badgeStr.length >= MAX_BADGE_LENGTH) return 2; // Beneficiaries (has PSN)
  return 1; // Employees
}

/**
 * Validates that badge identifiers are present and valid
 *
 * Checks:
 * - badgeNumber is not null/undefined and greater than 0
 * - psnSuffix is not null/undefined (can be 0, which is valid)
 *
 * @example
 * isValidBadgeIdentifiers(123456, 1) => true
 * isValidBadgeIdentifiers(123456, 0) => true
 * isValidBadgeIdentifiers(null, 1) => false
 * isValidBadgeIdentifiers(123456, undefined) => false
 *
 * @param badgeNumber - Employee badge number
 * @param psnSuffix - PSN suffix (can be 0)
 * @returns true if both identifiers are valid
 */
export function isValidBadgeIdentifiers(badgeNumber?: number, psnSuffix?: number): boolean {
  if (!badgeNumber || badgeNumber === null) return false;
  if (psnSuffix === null || psnSuffix === undefined) return false;
  return true;
}

/**
 * Decomposes PSN suffix into individual beneficiary level numbers
 *
 * PSN structure: Each digit represents a beneficiary level
 * - First digit: firstLevelBeneficiaryNumber
 * - Second digit: secondLevelBeneficiaryNumber
 * - Third digit: thirdLevelBeneficiaryNumber
 *
 * @example
 * decomposePSNSuffix(123) => { firstLevel: 0, secondLevel: 2, thirdLevel: 3 }
 * decomposePSNSuffix(5) => { firstLevel: 0, secondLevel: 0, thirdLevel: 5 }
 * decomposePSNSuffix(1234) => { firstLevel: 1, secondLevel: 2, thirdLevel: 4 }
 *
 * @param psnSuffix - PSN suffix number to decompose
 * @returns Object with individual beneficiary level numbers
 */
export function decomposePSNSuffix(psnSuffix: number): {
  firstLevel: number;
  secondLevel: number;
  thirdLevel: number;
} {
  return {
    firstLevel: Math.floor(psnSuffix / 1000) % 10,
    secondLevel: Math.floor(psnSuffix / 100) % 10,
    thirdLevel: Math.floor(psnSuffix / 10) % 10
  };
}
