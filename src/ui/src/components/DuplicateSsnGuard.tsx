import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { useLazyGetDuplicateSsnExistsQuery } from "reduxstore/api/LookupsApi";
import { ApiMessageAlert, setMessage } from "smart-ui-library";
import { Messages } from "../utils/messageDictonary";

interface DuplicateSsnGuardProps {
  /** Show ApiMessageAlert inline (default: true) */
  showAlert?: boolean;
  /** Custom message key to use (defaults to ProfitSharePrerequisiteIncomplete) */
  messageKey?: string;
  children: (ctx: { prerequisitesComplete: boolean; refresh: () => void }) => React.ReactNode;
}

/**
 * DuplicateSsnGuard queries the lookup endpoint to see if duplicate SSNs exist.
 * - If duplicates exist it dispatches a message (using `setMessage`) with an error
 *   and renders `ApiMessageAlert` so pages can display it in the same way other
 *   prerequisite messages are shown.
 * - The child render prop receives `prerequisitesComplete` which should be used to
 *   enable/disable buttons and a `refresh` callback to re-query the backend.
 */
const DuplicateSsnGuard: React.FC<DuplicateSsnGuardProps> = ({ showAlert = true, messageKey, children }) => {
  const dispatch = useDispatch();
  const [trigger, { data, refetch }] = useLazyGetDuplicateSsnExistsQuery();

  useEffect(() => {
    // initial call
    trigger();
  }, [trigger]);

  useEffect(() => {
    const exists = data === true;
    if (exists) {
      const msg = {
        key: messageKey ?? Messages.ProfitSharePrerequisiteIncomplete.key,
        message: {
          type: "error",
          title: "Duplicate SSNs detected",
          message: `There are duplicate SSNs in the system. These must be resolved before this page can be used.`
        }
      } as any;

      dispatch(setMessage(msg));
    }
    // Note: we intentionally do not clear the message when duplicates go away so the
    // user can still see what happened; pages may choose to clear messages on save if required.
  }, [data, dispatch, messageKey]);

  const prerequisitesComplete = data !== true;

  return (
    <>
      {showAlert && (
        <ApiMessageAlert
          commonKey={messageKey ?? Messages.ProfitSharePrerequisiteIncomplete.key}
          delay={30000}
        />
      )}
      {children({ prerequisitesComplete, refresh: () => refetch() })}
    </>
  );
};

export default DuplicateSsnGuard;
