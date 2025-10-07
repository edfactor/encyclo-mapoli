import { Error as ErrorIcon } from "@mui/icons-material";
import React, { useEffect } from "react";
import { useLazyGetDuplicateSsnExistsQuery } from "../reduxstore/api/LookupsApi";

interface DuplicateSsnGuardProps {
  /** Show alert banner inline (default: true) */
  showAlert?: boolean;
  children: (ctx: { prerequisitesComplete: boolean; refresh: () => void }) => React.ReactNode;
}

/**
 * DuplicateSsnGuard queries the lookup endpoint to see if duplicate SSNs exist.
 * - If duplicates exist it displays a persistent error banner (similar to "Page Locked for Modifications")
 * - The child render prop receives `prerequisitesComplete` which should be used to
 *   enable/disable buttons and a `refresh` callback to re-query the backend.
 */
const DuplicateSsnGuard: React.FC<DuplicateSsnGuardProps> = ({ showAlert = true, children }) => {
  const [trigger, { data, refetch }] = useLazyGetDuplicateSsnExistsQuery();

  useEffect(() => {
    // initial call
    trigger();
  }, [trigger]);

  const hasDuplicates = data === true;
  const prerequisitesComplete = !hasDuplicates;

  return (
    <>
      {showAlert && hasDuplicates && (
        <div className="missive-alert missive-error duplicate-ssn-alert">
          <div className="duplicate-ssn-alert-content">
            <ErrorIcon className="duplicate-ssn-alert-icon" />
            <div>
              <strong>Duplicate SSNs Detected</strong>
              <p className="duplicate-ssn-alert-text">
                There are duplicate SSNs in the system. These must be resolved before this page can be used.
              </p>
            </div>
          </div>
        </div>
      )}
      {children({ prerequisitesComplete, refresh: () => refetch() })}
    </>
  );
};

export default DuplicateSsnGuard;
