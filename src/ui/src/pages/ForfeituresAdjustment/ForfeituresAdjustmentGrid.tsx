import { Typography } from "@mui/material";
import { useCallback, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { DSMGrid, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { GetForfeituresAdjustmentColumns } from "./ForfeituresAdjustmentGridColumns";

interface ISortParams {
  sortBy: string;
  isSortDescending: boolean;
}

interface ForfeituresAdjustmentGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const mockData = [
  {
    clientNumber: "XXX",
    employeeNumber: "XXXXX",
    startingBalance: 50000.00,
    forfeitureAmount: 10000.00,
    netBalance: 40000.00,
    netVested: 40000.00
  }
];

const totalsRow = {
  startingBalance: 50000.00,
  forfeitureAmount: 10000.00,
  netBalance: 40000.00,
  netVested: 40000.00
};

const ForfeituresAdjustmentGrid: React.FC<ForfeituresAdjustmentGridProps> = ({ 
  initialSearchLoaded, 
  setInitialSearchLoaded 
}) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "clientNumber",
    isSortDescending: false
  });

  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (badgeNumber: string) => {
      navigate(`/master-inquiry/${badgeNumber}`);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetForfeituresAdjustmentColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {initialSearchLoaded && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Forfeiture Adjustments (${mockData.length} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.FORFEITURES_ADJUSTMENT}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: mockData,
              pinnedTopRowData: [totalsRow],
              columnDefs: columnDefs
            }}
          />
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setPageNumber(value - 1);
              setInitialSearchLoaded(true);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              setPageSize(value);
              setPageNumber(1);
              setInitialSearchLoaded(true);
            }}
            recordCount={mockData.length}
          />
        </>
      )}
    </>
  );
};

export default ForfeituresAdjustmentGrid; 