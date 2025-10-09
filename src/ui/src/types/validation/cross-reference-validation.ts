/**
 * Type definitions for Master Update Cross-Reference Validation
 * These types correspond to the backend DTOs for displaying validation results in the UI.
 *
 * @see PS-MASTER_UPDATE_CROSSREF_VALIDATION_IMPLEMENTATION.md
 */

/**
 * Individual field validation result showing if a specific report field matches its archived value
 */
export interface CrossReferenceValidation {
  /** Field name being validated (e.g., "DistributionTotals") */
  fieldName: string;

  /** Report code this field comes from (e.g., "PAY443", "QPAY129") */
  reportCode: string;

  /** Whether the field's current value matches the expected archived value */
  isValid: boolean;

  /** Current value from the report (if available) */
  currentValue: number | null;

  /** Expected value from archived checksums (if available) */
  expectedValue: number | null;

  /** Difference between current and expected (if mismatch) */
  variance: number | null;

  /** Human-readable message about validation status */
  message: string | null;

  /** When the expected value was archived */
  archivedAt: string | null;

  /** Additional notes or context about this validation */
  notes: string | null;
}

/**
 * Group of related validations organized by category (e.g., "Total Distributions")
 */
export interface CrossReferenceValidationGroup {
  /** Name of the validation group (e.g., "Total Distributions") */
  groupName: string;

  /** Description of what this group validates */
  description: string | null;

  /** Whether all validations in this group passed */
  isValid: boolean;

  /** Individual field validations in this group */
  validations: CrossReferenceValidation[];

  /** Summary message for this group's status */
  summary: string | null;

  /** Priority level: "Critical" (blocks Master Update), "High" (warns), "Medium", "Low" */
  priority: "Critical" | "High" | "Medium" | "Low";

  /** The validation rule being checked (e.g., "PAY444.DISTRIB = PAY443.DistributionTotals = QPAY129.Distributions") */
  validationRule: string | null;
}

/**
 * Complete validation result for Master Update operation including all validation groups
 */
export interface MasterUpdateCrossReferenceValidationResponse {
  /** Profit year being validated */
  profitYear: number;

  /** Whether all validations passed */
  isValid: boolean;

  /** Overall validation message */
  message: string;

  /** All validation groups (Distributions, Forfeitures, Contributions, Earnings) */
  validationGroups: CrossReferenceValidationGroup[];

  /** Total number of individual validations performed */
  totalValidations: number;

  /** Number of validations that passed */
  passedValidations: number;

  /** Number of validations that failed */
  failedValidations: number;

  /** List of report codes that were validated */
  validatedReports: string[];

  /** Whether Master Update should be blocked due to Critical failures */
  blockMasterUpdate: boolean;

  /** List of critical issues that block Master Update */
  criticalIssues: string[];

  /** List of warnings that don't block but should be reviewed */
  warnings: string[];

  /** When this validation was performed */
  validatedAt: string;
}

/**
 * Extended ProfitShareMasterResponse that includes cross-reference validation
 */
export interface ProfitShareMasterResponseWithValidation {
  reportName: string;
  beneficiariesEffected?: number;
  employeesEffected?: number;
  etvasEffected?: number;

  /** Cross-reference validation results (if available) */
  crossReferenceValidation?: MasterUpdateCrossReferenceValidationResponse;
}
