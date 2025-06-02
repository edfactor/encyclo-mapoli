import { Typography } from "@mui/material";
import { useMemo, useEffect, useState } from "react";
import { useLazyGetProfitMasterInquiryMemberDetailsQuery } from "reduxstore/api/InquiryApi";
import { DSMGrid, Pagination} from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { CAPTIONS } from "../../constants";

interface MasterInquiryGridProps {
  initialSearchLoaded?: boolean;
  setInitialSearchLoaded?: (loaded: boolean) => void;
  memberType?: number;
  id?: number;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({
                                                               memberType,
                                                               id,
                                                             }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  // Only render if both memberType and id are present
  if (memberType === undefined || id === undefined) {
    return null;
  }

  const [
    triggerMemberDetails,
    {
      data: memberDetailsData,
      isFetching: isFetchingMemberDetails,
      isError: isErrorMemberDetails,
      error: errorMemberDetails,
    },
  ] = useLazyGetProfitMasterInquiryMemberDetailsQuery();

  useEffect(() => {
    triggerMemberDetails({ memberType, id });
  }, [memberType, id, triggerMemberDetails]);

  if (isFetchingMemberDetails) {
    return <Typography>Loading member details...</Typography>;
  }

  if (isErrorMemberDetails) {
    return (
      <Typography color="error">
        Error loading member details: {errorMemberDetails && JSON.stringify(errorMemberDetails)}
      </Typography>
    );
  }

  // Debug: Show the actual API response shape
  console.log("Member Details Data:", memberDetailsData);
  
    <pre style={{ maxHeight: 300, overflow: 'auto', background: '#f5f5f5', fontSize: 12 }}>
      {JSON.stringify(memberDetailsData, null, 2)}
    </pre>
  

return (
    <>
      {!!memberDetailsData && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Master Inquiry (${memberDetailsData?.total || 0} ${memberDetailsData?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.MASTER_INQUIRY}
            isLoading={isFetchingMemberDetails}
            providedOptions={{
              rowData: memberDetailsData?.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!memberDetailsData && memberDetailsData.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={memberDetailsData.total}
        />
      )}
    </>
  );
};

export default MasterInquiryGrid;