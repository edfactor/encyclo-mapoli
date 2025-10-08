import { Error as ErrorIcon, Warning as WarningIcon } from "@mui/icons-material";
import React, { useEffect } from "react";
import { useLazyGetDuplicateSsnExistsQuery } from "../reduxstore/api/LookupsApi";

interface DuplicateSsnGuardProps {
  /** Show alert banner inline (default: true) */
  showAlert?: boolean;
  /**
   * Mode determines behavior when duplicates are detected:
   * - 'error': Blocks page usage (prerequisitesComplete = false) - default behavior
   * - 'warning': Shows warning but allows page usage (prerequisitesComplete = true)
   */
  mode?: "error" | "warning";
  children: (ctx: { prerequisitesComplete: boolean; hasDuplicates: boolean; refresh: () => void }) => React.ReactNode;
}

/**
 * DuplicateSsnGuard queries the lookup endpoint to see if duplicate SSNs exist.
 * - If duplicates exist and mode='error' (default): displays persistent error banner and blocks page usage
 * - If duplicates exist and mode='warning': displays warning banner but allows page usage
 * - The child render prop receives:
 *   - `prerequisitesComplete`: false when duplicates exist in error mode, true otherwise
 *   - `hasDuplicates`: true when duplicates are detected (regardless of mode)
 *   - `refresh`: callback to re-query the backend
 */
const DuplicateSsnGuard: React.FC<DuplicateSsnGuardProps> = ({ showAlert = true, mode = "error", children }) => {
  const [trigger, { data }] = useLazyGetDuplicateSsnExistsQuery();

  useEffect(() => {
    // initial call
    trigger();
  }, [trigger]);

  const hasDuplicates = data === true;
  // In error mode, block the page. In warning mode, allow page usage.
  const prerequisitesComplete = mode === "warning" ? true : !hasDuplicates;

  const isErrorMode = mode === "error";
  const alertClassName = `missive-alert ${isErrorMode ? "missive-error" : "missive-warning"} duplicate-ssn-alert`;
  const AlertIcon = isErrorMode ? ErrorIcon : WarningIcon;
  const iconColor = isErrorMode ? "#d32f2f" : "#ed6c02";
  const alertTitle = isErrorMode ? "Duplicate SSNs Detected" : "Duplicate SSNs Warning";
  const alertText = isErrorMode
    ? "There are duplicate SSNs in the system. These must be resolved before this page can be used."
    : "There are duplicate SSNs in the system. Please be aware that this may cause issues. Consider resolving them when possible.";

  return (
    <>
      {showAlert && hasDuplicates && (
        <div className={alertClassName}>
          <div className="duplicate-ssn-alert-content">
            <AlertIcon
              className="duplicate-ssn-alert-icon"
              style={{ color: iconColor }}
            />
            <div>
              <strong>{alertTitle}</strong>
              <p className="duplicate-ssn-alert-text">{alertText}</p>
            </div>
          </div>
        </div>
      )}
      {children({ prerequisitesComplete, hasDuplicates, refresh: () => trigger() })}
    </>
  );
};

export default DuplicateSsnGuard;
