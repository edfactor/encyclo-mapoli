import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MissiveResponse } from "reduxstore/types";
import MasterInquiryMemberDetails from "./MasterInquiryMemberDetails";
import { useMissiveAlerts } from "./hooks/useMissiveAlerts";
import { MASTER_INQUIRY_MESSAGES } from "./utils/MasterInquiryMessages";

interface StandaloneMemberDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
}

/*
 * Standalone component that fetches and displays member details.
 * This is a wrapper around MasterInquiryMemberDetails that handles its own data fetching.
 * Used by components that need to display member details not using the master inquiry workflow.
 */
const StandaloneMemberDetails: React.FC<StandaloneMemberDetailsProps> = ({ memberType, id, profitYear }) => {
  const [memberDetails, setMemberDetails] = useState<any>(null);
  const [triggerMemberDetails, { isFetching }] = useLazyGetProfitMasterInquiryMemberQuery();

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  const missives = useSelector((state: RootState) => state.lookups.missives);
  const { addAlert, addAlerts } = useMissiveAlerts();

  useEffect(() => {
    if (memberType && id) {
      triggerMemberDetails({
        memberType,
        id: Number(id),
        profitYear: profitYear ?? undefined
      })
        .unwrap()
        .then((details) => {
          setMemberDetails(details);

          if (details.missives && missives) {
            const localMissives: MissiveResponse[] = details.missives
              .map((missiveId: number) => missives.find((m: MissiveResponse) => m.id === missiveId))
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
  }, [
    memberType,
    id,
    profitYear,
    triggerMemberDetails,
    missives,
    masterInquiryRequestParams?.memberType,
    addAlert,
    addAlerts
  ]);

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
