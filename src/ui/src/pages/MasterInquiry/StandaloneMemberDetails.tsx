import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MissiveResponse } from "reduxstore/types";
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
  const [memberDetails, setMemberDetails] = useState<any>(null);
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

          // We cannot cross references missives unless we have some in the redux store
          if (details.missives && Array.isArray(reduxStoreMissives) && reduxStoreMissives.length > 0) {
            const localMissives: MissiveResponse[] = details.missives
              .map((missiveId: number) => reduxStoreMissives.find((m: MissiveResponse) => m.id === missiveId))
              .filter(Boolean) as MissiveResponse[];

            if (localMissives.length > 0) {
              addAlerts(localMissives);
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
  }, [
    memberType,
    id,
    profitYear,
    triggerMemberDetails,
    reduxStoreMissives,
    masterInquiryRequestParams?.memberType,
    addAlert,
    addAlerts
  ]);

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
