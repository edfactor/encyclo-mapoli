import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MissiveResponse, MasterInquiryDetail } from "reduxstore/types";
import { MASTER_INQUIRY_MESSAGES } from "../../components/MissiveAlerts/MissiveMessages";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import MasterInquiryMemberDetails from "./MasterInquiryMemberDetails";

interface StandaloneMemberDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
  refreshTrigger?: number; // Optional property to force refresh
}

/*
 * Standalone component that fetches and displays member details.
 * This is a wrapper around MasterInquiryMemberDetails that handles its own data fetching.
 * Used by components that need to display member details not using the master inquiry workflow.
 */
const StandaloneMemberDetails: React.FC<StandaloneMemberDetailsProps> = ({
  memberType,
  id,
  profitYear,
  refreshTrigger
}) => {
  const [memberDetails, setMemberDetails] = useState<MasterInquiryDetail | null>(null);
  const [triggerMemberDetails, { isFetching }] = useLazyGetProfitMasterInquiryMemberQuery();
  const prevRefreshTrigger = useRef<number | undefined>(refreshTrigger);

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  const reduxStoreMissives = useSelector((state: RootState) => state.lookups.missives);
  const { addAlert, addAlerts } = useMissiveAlerts();

  const fetchMemberDetails = useRef(() => {
    if (memberType && id) {
      triggerMemberDetails({
        memberType,
        id: Number(id),
        profitYear: profitYear ?? undefined
      })
        .unwrap()
        .then((details) => {
          setMemberDetails(details);

          // Process missives if present in the response
          if (details.missives && details.missives.length > 0) {
            if (Array.isArray(reduxStoreMissives) && reduxStoreMissives.length > 0) {
              // Cross-reference with Redux store missives
              const localMissives: MissiveResponse[] = details.missives
                .map((missiveId: number) => reduxStoreMissives.find((m: MissiveResponse) => m.id === missiveId))
                .filter(Boolean) as MissiveResponse[];

              if (localMissives.length > 0) {
                addAlerts(localMissives);
              } else {
                console.warn("Missive IDs from API not found in Redux store:", details.missives);
              }
            } else {
              console.warn("Missives lookup data not loaded yet. Cannot display alerts for:", details.missives);
            }
          }

          if (!details.isEmployee && masterInquiryRequestParams?.memberType === "all") {
            addAlert(MASTER_INQUIRY_MESSAGES.BENEFICIARY_FOUND(details.ssn));
          }
        })
        .catch((error) => {
          console.error("Failed to fetch member details:", error);
          setMemberDetails(null);
        });
    }
  });

  // Initial fetch and refetch when dependencies change
  useEffect(() => {
    fetchMemberDetails.current();
  }, [memberType, id, profitYear, triggerMemberDetails, masterInquiryRequestParams?.memberType, addAlert, addAlerts]);

  // Process missives when they become available for existing member details
  useEffect(() => {
    if (
      memberDetails?.missives &&
      memberDetails.missives.length > 0 &&
      Array.isArray(reduxStoreMissives) &&
      reduxStoreMissives.length > 0
    ) {
      const localMissives: MissiveResponse[] = memberDetails.missives
        .map((missiveId: number) => reduxStoreMissives.find((m: MissiveResponse) => m.id === missiveId))
        .filter(Boolean) as MissiveResponse[];

      if (localMissives.length > 0) {
        addAlerts(localMissives);
      }
    }
  }, [reduxStoreMissives, memberDetails?.missives, addAlerts]);

  // Refetch when refreshTrigger changes
  useEffect(() => {
    if (refreshTrigger !== undefined && refreshTrigger !== prevRefreshTrigger.current) {
      prevRefreshTrigger.current = refreshTrigger;
      fetchMemberDetails.current();
    }
  }, [refreshTrigger]);

  return (
    <MasterInquiryMemberDetails
      memberType={memberType}
      id={id}
      profitYear={profitYear}
      memberDetails={memberDetails}
      isLoading={isFetching}
    />
  );
};

export default StandaloneMemberDetails;
