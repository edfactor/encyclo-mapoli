/**
 * Returns a formatted label for a tax code ID
 * @param taxCodeId - Single character tax code ID
 * @returns Formatted string "ID: Description" or empty string if not found
 */
export const getTaxCodeLabel = (taxCodeId: string): string => {
  const taxCodes: Record<string, string> = {
    "0": "0: Unknown",
    "1": "1: Early (Premature) dist no known exception",
    "2": "2: Early (Premature) dist exception applies",
    "3": "3: Disability",
    "4": "4: Death",
    "5": "5: Prohibited transaction",
    "6": "6: Section 1035 exchange",
    "7": "7: Normal distribution",
    "8": "8: Excess contributions + earnings/deferrals",
    "9": "9: PS 58 cost",
    A: "A: Qualifies for 5- or 10-year averaging",
    B: "B: Qualifies for death benefit exclusion",
    C: "C: Qualifies for both A and B",
    D: "D: Excess contributions + earnings deferrals",
    E: "E: Excess annual additions under section 415",
    F: "F: Charitable gift annuity",
    G: "G: Direct rollover to IRA",
    H: "H: Direct rollover to plan/tax sheltered annuity",
    P: "P: Excess contributions + earnings/deferrals"
  };

  return taxCodes[taxCodeId] ?? "";
};

/**
 * Returns a label for a pay frequency ID
 * @param frequencyId - Single character frequency ID ("M" or "W")
 * @returns "Monthly", "Weekly", or empty string if not found
 */
export const getPayFrequencyLabel = (frequencyId: string): string => {
  const frequencies: Record<string, string> = {
    M: "Monthly",
    W: "Weekly"
  };

  return frequencies[frequencyId] ?? "";
};

/**
 * Returns a label for a status ID
 * @param statusId - Single character status ID ("A", "I", "D", or "T")
 * @returns "Active", "Inactive", "Deceased", "Terminated", or empty string if not found
 */
export const getStatusLabel = (statusId: string): string => {
  const statuses: Record<string, string> = {
    A: "Active",
    I: "Inactive",
    D: "Deceased",
    T: "Terminated"
  };

  return statuses[statusId] ?? "";
};
