import { Health } from "../reduxstore/healthTypes";

/**
 * Gets the description from the first entry whose status matches the Health object's status.
 * @param health The Health object to check
 * @returns The description of the matching entry, or undefined if no match is found
 */
export const getHealthStatusDescription = (health: Health): string | undefined => {
  // If health is healthy, no need to check entries
  if (health.status === "Healthy") {
    return undefined;
  }

  // Look through all entries for a matching status
  for (const [_key, entry] of Object.entries(health.entries)) {
    if (entry.status === health.status && entry.description) {
      return entry.description;
    }
  }

  return undefined;
};
