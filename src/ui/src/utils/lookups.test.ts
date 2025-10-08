import { describe, expect, it } from "vitest";
import {
  getPayFrequencyLabel,
  getPaymentFlagLabel,
  getReasonLabel,
  getStatusLabel,
  getTaxCodeLabel,
  getTerminationCodeLabel
} from "./lookups";

describe("getTaxCodeLabel", () => {
  it("should return label for tax code 0", () => {
    expect(getTaxCodeLabel("0")).toBe("0: Unknown");
  });

  it("should return label for tax code 1", () => {
    expect(getTaxCodeLabel("1")).toBe("1: Early (Premature) dist no known exception");
  });

  it("should return label for tax code 2", () => {
    expect(getTaxCodeLabel("2")).toBe("2: Early (Premature) dist exception applies");
  });

  it("should return label for tax code 3", () => {
    expect(getTaxCodeLabel("3")).toBe("3: Disability");
  });

  it("should return label for tax code 4", () => {
    expect(getTaxCodeLabel("4")).toBe("4: Death");
  });

  it("should return label for tax code 5", () => {
    expect(getTaxCodeLabel("5")).toBe("5: Prohibited transaction");
  });

  it("should return label for tax code 6", () => {
    expect(getTaxCodeLabel("6")).toBe("6: Section 1035 exchange");
  });

  it("should return label for tax code 7", () => {
    expect(getTaxCodeLabel("7")).toBe("7: Normal distribution");
  });

  it("should return label for tax code 8", () => {
    expect(getTaxCodeLabel("8")).toBe("8: Excess contributions + earnings/deferrals");
  });

  it("should return label for tax code 9", () => {
    expect(getTaxCodeLabel("9")).toBe("9: PS 58 cost");
  });

  it("should return label for tax code A", () => {
    expect(getTaxCodeLabel("A")).toBe("A: Qualifies for 5- or 10-year averaging");
  });

  it("should return label for tax code B", () => {
    expect(getTaxCodeLabel("B")).toBe("B: Qualifies for death benefit exclusion");
  });

  it("should return label for tax code C", () => {
    expect(getTaxCodeLabel("C")).toBe("C: Qualifies for both A and B");
  });

  it("should return label for tax code D", () => {
    expect(getTaxCodeLabel("D")).toBe("D: Excess contributions + earnings deferrals");
  });

  it("should return label for tax code E", () => {
    expect(getTaxCodeLabel("E")).toBe("E: Excess annual additions under section 415");
  });

  it("should return label for tax code F", () => {
    expect(getTaxCodeLabel("F")).toBe("F: Charitable gift annuity");
  });

  it("should return label for tax code G", () => {
    expect(getTaxCodeLabel("G")).toBe("G: Direct rollover to IRA");
  });

  it("should return label for tax code H", () => {
    expect(getTaxCodeLabel("H")).toBe("H: Direct rollover to plan/tax sheltered annuity");
  });

  it("should return label for tax code P", () => {
    expect(getTaxCodeLabel("P")).toBe("P: Excess contributions + earnings/deferrals");
  });

  it("should return empty string for unrecognized tax code", () => {
    expect(getTaxCodeLabel("X")).toBe("");
  });

  it("should return empty string for lowercase letter", () => {
    expect(getTaxCodeLabel("a")).toBe("");
  });

  it("should return empty string for empty string", () => {
    expect(getTaxCodeLabel("")).toBe("");
  });

  it("should return empty string for special characters", () => {
    expect(getTaxCodeLabel("@")).toBe("");
    expect(getTaxCodeLabel("#")).toBe("");
  });

  it("should handle all valid numeric codes", () => {
    const numericCodes = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    numericCodes.forEach((code) => {
      const result = getTaxCodeLabel(code);
      expect(result).toContain(code + ":");
      expect(result.length).toBeGreaterThan(2);
    });
  });

  it("should handle all valid letter codes", () => {
    const letterCodes = ["A", "B", "C", "D", "E", "F", "G", "H", "P"];
    letterCodes.forEach((code) => {
      const result = getTaxCodeLabel(code);
      expect(result).toContain(code + ":");
      expect(result.length).toBeGreaterThan(2);
    });
  });
});

describe("getPayFrequencyLabel", () => {
  it("should return Monthly for M", () => {
    expect(getPayFrequencyLabel("M")).toBe("Monthly");
  });

  it("should return Weekly for W", () => {
    expect(getPayFrequencyLabel("W")).toBe("Weekly");
  });

  it("should return empty string for invalid frequency", () => {
    expect(getPayFrequencyLabel("X")).toBe("");
  });

  it("should return empty string for lowercase letters", () => {
    expect(getPayFrequencyLabel("m")).toBe("");
    expect(getPayFrequencyLabel("w")).toBe("");
  });

  it("should return empty string for empty string", () => {
    expect(getPayFrequencyLabel("")).toBe("");
  });

  it("should return empty string for numeric values", () => {
    expect(getPayFrequencyLabel("1")).toBe("");
  });
});

describe("getStatusLabel", () => {
  it("should return Active for A", () => {
    expect(getStatusLabel("A")).toBe("Active");
  });

  it("should return Inactive for I", () => {
    expect(getStatusLabel("I")).toBe("Inactive");
  });

  it("should return Deceased for D", () => {
    expect(getStatusLabel("D")).toBe("Deceased");
  });

  it("should return Terminated for T", () => {
    expect(getStatusLabel("T")).toBe("Terminated");
  });

  it("should return empty string for invalid status", () => {
    expect(getStatusLabel("X")).toBe("");
  });

  it("should return empty string for lowercase letters", () => {
    expect(getStatusLabel("a")).toBe("");
    expect(getStatusLabel("i")).toBe("");
    expect(getStatusLabel("d")).toBe("");
    expect(getStatusLabel("t")).toBe("");
  });

  it("should return empty string for empty string", () => {
    expect(getStatusLabel("")).toBe("");
  });

  it("should return empty string for numeric values", () => {
    expect(getStatusLabel("1")).toBe("");
  });

  it("should handle all valid status codes", () => {
    const statusCodes = ["A", "I", "D", "T"];
    statusCodes.forEach((code) => {
      const result = getStatusLabel(code);
      expect(result.length).toBeGreaterThan(0);
      expect(["Active", "Inactive", "Deceased", "Terminated"]).toContain(result);
    });
  });
});

describe("getTerminationCodeLabel", () => {
  it("should return label for termination code A", () => {
    expect(getTerminationCodeLabel("A")).toBe("LEFT ON OWN");
  });

  it("should return label for termination code B", () => {
    expect(getTerminationCodeLabel("B")).toBe("PERSONAL OR FAMILY REASON");
  });

  it("should return label for termination code F", () => {
    expect(getTerminationCodeLabel("F")).toBe("FMLA-EXPIRED");
  });

  it("should return label for termination code R", () => {
    expect(getTerminationCodeLabel("R")).toBe("RETIRED");
  });

  it("should return label for termination code Z", () => {
    expect(getTerminationCodeLabel("Z")).toBe("DECEASED");
  });

  it("should return empty string for invalid termination code", () => {
    expect(getTerminationCodeLabel("0")).toBe("");
    expect(getTerminationCodeLabel("1")).toBe("");
  });

  it("should return empty string for lowercase letters", () => {
    expect(getTerminationCodeLabel("a")).toBe("");
    expect(getTerminationCodeLabel("z")).toBe("");
  });

  it("should return empty string for empty string", () => {
    expect(getTerminationCodeLabel("")).toBe("");
  });

  it("should handle all valid termination codes A-Z", () => {
    const terminationCodes = [
      "A",
      "B",
      "C",
      "D",
      "E",
      "F",
      "G",
      "H",
      "I",
      "J",
      "K",
      "L",
      "M",
      "N",
      "O",
      "P",
      "Q",
      "R",
      "S",
      "T",
      "U",
      "V",
      "W",
      "X",
      "Y",
      "Z"
    ];
    terminationCodes.forEach((code) => {
      const result = getTerminationCodeLabel(code);
      expect(result.length).toBeGreaterThan(0);
      expect(result).not.toBe("");
    });
  });

  it("should return specific labels for key termination codes", () => {
    expect(getTerminationCodeLabel("D")).toBe("STEALING");
    expect(getTerminationCodeLabel("H")).toBe("JOB ABANDONMENT");
    expect(getTerminationCodeLabel("M")).toBe("POOR PERFORMANCE");
    expect(getTerminationCodeLabel("W")).toBe("RETIRED & RECEIVING PENSION");
    expect(getTerminationCodeLabel("Y")).toBe("FMLA-APPROVED");
  });
});

describe("getPaymentFlagLabel", () => {
  it("should return label for payment flag Y", () => {
    expect(getPaymentFlagLabel("Y")).toBe("Yes");
  });

  it("should return label for payment flag H", () => {
    expect(getPaymentFlagLabel("H")).toBe("Hold");
  });

  it("should return label for payment flag D", () => {
    expect(getPaymentFlagLabel("D")).toBe("Delete");
  });

  it("should return label for payment flag C", () => {
    expect(getPaymentFlagLabel("C")).toBe("Manual Check");
  });

  it("should return label for payment flag O", () => {
    expect(getPaymentFlagLabel("O")).toBe("Override");
  });

  it("should return empty string for invalid payment flag", () => {
    expect(getPaymentFlagLabel("X")).toBe("");
    expect(getPaymentFlagLabel("Z")).toBe("");
    expect(getPaymentFlagLabel("1")).toBe("");
  });

  it("should return empty string for lowercase letters", () => {
    expect(getPaymentFlagLabel("y")).toBe("");
    expect(getPaymentFlagLabel("h")).toBe("");
    expect(getPaymentFlagLabel("d")).toBe("");
  });

  it("should return empty string for empty string", () => {
    expect(getPaymentFlagLabel("")).toBe("");
  });

  it("should handle all valid payment flags", () => {
    const paymentFlags = ["Y", "H", "D", "C", "O"];
    paymentFlags.forEach((flag) => {
      const result = getPaymentFlagLabel(flag);
      expect(result.length).toBeGreaterThan(0);
      expect(result).not.toBe("");
    });
  });
});

describe("getReasonLabel", () => {
  it("should return label for reason code H", () => {
    expect(getReasonLabel("H")).toBe("Hardship");
  });

  it("should return label for reason code P", () => {
    expect(getReasonLabel("P")).toBe("Pay Direct");
  });

  it("should return label for reason code R", () => {
    expect(getReasonLabel("R")).toBe("Rollover");
  });

  it("should return label for reason code M", () => {
    expect(getReasonLabel("M")).toBe("Monthly");
  });

  it("should return label for reason code Q", () => {
    expect(getReasonLabel("Q")).toBe("Quarterly");
  });

  it("should return label for reason code A", () => {
    expect(getReasonLabel("A")).toBe("Annually");
  });

  it("should return empty string for invalid reason code", () => {
    expect(getReasonLabel("X")).toBe("");
    expect(getReasonLabel("Z")).toBe("");
    expect(getReasonLabel("1")).toBe("");
  });

  it("should return empty string for lowercase letters", () => {
    expect(getReasonLabel("h")).toBe("");
    expect(getReasonLabel("p")).toBe("");
    expect(getReasonLabel("r")).toBe("");
  });

  it("should return empty string for empty string", () => {
    expect(getReasonLabel("")).toBe("");
  });

  it("should handle all valid reason codes", () => {
    const reasonCodes = ["H", "P", "R", "M", "Q", "A"];
    reasonCodes.forEach((code) => {
      const result = getReasonLabel(code);
      expect(result.length).toBeGreaterThan(0);
      expect(result).not.toBe("");
    });
  });

  it("should return specific labels for all reason codes", () => {
    expect(getReasonLabel("H")).toBe("Hardship");
    expect(getReasonLabel("P")).toBe("Pay Direct");
    expect(getReasonLabel("R")).toBe("Rollover");
    expect(getReasonLabel("M")).toBe("Monthly");
    expect(getReasonLabel("Q")).toBe("Quarterly");
    expect(getReasonLabel("A")).toBe("Annually");
  });
});
