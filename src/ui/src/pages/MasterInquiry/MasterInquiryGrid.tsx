import { Typography } from "@mui/material";
import { useMemo, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberDetailsQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid } from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { CAPTIONS } from "../../constants";

interface MasterInquiryGridProps {
  initialSearchLoaded?: boolean;
  setInitialSearchLoaded?: (loaded: boolean) => void;
  memberType?: number;
  id?: number;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded, memberType, id }) => {
  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  // Only member details mode remains
  if (memberType !== undefined && id !== undefined) {
    const [triggerMemberDetails, { data: memberDetailsData, isFetching: isFetchingMemberDetails }] = useLazyGetProfitMasterInquiryMemberDetailsQuery();
    useEffect(() => {
      triggerMemberDetails({ memberType, id });
    }, [memberType, id, triggerMemberDetails]);

    return (
      <>
        {isFetchingMemberDetails && <Typography>Loading member details...</Typography>}
        {memberDetailsData && (
          <DSMGrid
            preferenceKey={CAPTIONS.MASTER_INQUIRY}
            isLoading={isFetchingMemberDetails}
            providedOptions={{
              rowData: memberDetailsData.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        )}
      </>
    );
  }

  // If not in member details mode, render nothing (or a placeholder if desired)
  return null;
};

export default MasterInquiryGrid;
