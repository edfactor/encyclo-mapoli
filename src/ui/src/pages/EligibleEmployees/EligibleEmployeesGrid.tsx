import { Typography } from "@mui/material";
import { useMemo, useState, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEligibleEmployeesQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetEligibleEmployeesColumns } from "./EligibleEmployeesGridColumn";

interface EligibleEmployeesGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const EligibleEmployeesGrid: React.FC<EligibleEmployeesGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { eligibleEmployees, eligibleEmployeesQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetEligibleEmployeesColumns(), []);

  const [triggerSearch, { isFetching }] = useLazyGetEligibleEmployeesQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: eligibleEmployeesQueryParams?.profitYear ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [eligibleEmployeesQueryParams?.profitYear, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  return (
    <>
      {eligibleEmployees?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT-ELIGIBLE REPORT (${eligibleEmployees?.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"ELIGIBLE_EMPLOYEES"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: eligibleEmployees?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!eligibleEmployees && eligibleEmployees.response.results.length > 0 && (
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
          recordCount={eligibleEmployees.response.total}
        />
      )}
    </>
  );
};

export default EligibleEmployeesGrid;
