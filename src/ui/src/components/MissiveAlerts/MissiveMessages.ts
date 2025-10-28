import { MissiveResponse } from "reduxstore/types";

export const FORFEITURES_ADJUSTMENT_MESSAGES = {
  MEMBER_NOT_FOUND: {
    id: 950,
    severity: "Error" as const,
    message: "Member Not Found",
    description: "Member not found."
  } as MissiveResponse
};

export const MASTER_INQUIRY_MESSAGES = {
  MEMBER_NOT_FOUND: {
    id: 900,
    severity: "Error" as const,
    message: "Member Not Found",
    description: "The member you are searching for does not exist in the system."
  } as MissiveResponse,

  BENEFICIARY_NOT_FOUND: {
    id: 904,
    severity: "Error" as const,
    message: "Beneficiary not found",
    description: "The beneficiary you are searching for does not exist in the system."
  } as MissiveResponse,

  NO_RESULTS_FOUND: {
    id: 901,
    severity: "Error" as const,
    message: "No Results Found",
    description: "The search did not return any results. Please try a different search criteria."
  } as MissiveResponse,

  BENEFICIARY_FOUND: (ssn: string | number): MissiveResponse => ({
    id: 902,
    severity: "info",
    message: `Beneficiary ${ssn} Found`,
    description: "This member is a beneficiary and not an employee."
  }),

  MILITARY_VESTED_WARNING: (vestedPercentage: number): MissiveResponse => ({
    id: 903,
    severity: "warning",
    message: "Military entry has affected vested percentage",
    description: `Vested percentage now at ${vestedPercentage * 100}%.`
  })
};

export const DISTRIBUTION_INQUIRY_MESSAGES = {
  MEMBER_NOT_FOUND: {
    id: 910,
    severity: "Error" as const,
    message: "Member Not Found",
    description: "The member you are searching for does not exist in the system."
  } as MissiveResponse,

  DISTRIBUTION_SAVED_SUCCESS: {
    id: 911,
    severity: "success" as const,
    message: "Distribution Saved Successfully",
    description: "The distribution has been saved successfully."
  } as MissiveResponse
};

export const EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES = {
  EXECUTIVE_HOURS_SAVED_SUCCESS: {
    id: 970,
    severity: "success" as const,
    message: "Executive Hours and Dollars Saved",
    description: "All executive hours and dollars have been saved successfully."
  } as MissiveResponse,

  EXECUTIVE_HOURS_SAVE_ERROR: {
    id: 971,
    severity: "error" as const,
    message: "Save Failed",
    description: "Failed to save executive hours and dollars. Please try again."
  } as MissiveResponse
};
