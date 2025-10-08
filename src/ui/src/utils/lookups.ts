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

/**
 * Returns a label for a termination code
 * @param terminationCode - Single character termination code
 * @returns Termination reason description or empty string if not found
 */
export const getTerminationCodeLabel = (terminationCode: string): string => {
  const terminationCodes: Record<string, string> = {
    A: "LEFT ON OWN",
    B: "PERSONAL OR FAMILY REASON",
    C: "COULD NOT WORK AVAILABLE HOURS",
    D: "STEALING",
    E: "NOT FOLLOWING COMPANY POLICY",
    F: "FMLA-EXPIRED",
    G: "TERMINATED-PRIVATE",
    H: "JOB ABANDONMENT",
    I: "HEALTH REASONS – NON FMLA",
    J: "LAYOFF NO WORK",
    K: "SCHOOL OR SPORTS",
    L: "MOVE OUT OF AREA",
    M: "POOR PERFORMANCE",
    N: "OFF FOR SUMMER",
    O: "WORKMAN'S COMPENSATION",
    P: "INJURED",
    Q: "TRANSFERRED",
    R: "RETIRED",
    S: "COMPETITION",
    T: "ANOTHER JOB",
    U: "WOULD NOT REHIRE",
    V: "NEVER REPORTED",
    W: "RETIRED & RECEIVING PENSION",
    X: "MILITARY",
    Y: "FMLA-APPROVED",
    Z: "DECEASED"
  };

  return terminationCodes[terminationCode] ?? "";
};

/**
 * Returns a label for a payment flag
 * @param paymentFlag - Single character payment flag ("Y", "H", "D", "C", or "O")
 * @returns Payment flag description or empty string if not found
 */
export const getPaymentFlagLabel = (paymentFlag: string): string => {
  const paymentFlags: Record<string, string> = {
    Y: "Yes",
    H: "Hold",
    D: "Delete",
    C: "Manual Check",
    O: "Override"
  };

  return paymentFlags[paymentFlag] ?? "";
};

/**
 * Returns a label for a reason code
 * @param reasonCode - Single character reason code ("H", "P", "R", "M", "Q", or "A")
 * @returns Reason description or empty string if not found
 */
export const getReasonLabel = (reasonCode: string): string => {
  const reasons: Record<string, string> = {
    H: "Hardship",
    P: "Pay Direct",
    R: "Rollover",
    M: "Monthly",
    Q: "Quarterly",
    A: "Annually"
  };

  return reasons[reasonCode] ?? "";
};
