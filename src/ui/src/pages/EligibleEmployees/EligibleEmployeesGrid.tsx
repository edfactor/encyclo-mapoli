import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetEligibleEmployeesColumns } from "./EligibleEmployeesGridColumn";

const EligibleEmployeesGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const { eligibleEmployees } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNegativeEVTASSNQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetEligibleEmployeesColumns(), []);

  return (
    <>
      {eligibleEmployees?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT-ELIGIBLE REPORT (${eligibleEmployees?.response.total || 0})`}
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
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={eligibleEmployees.response.total}
        />
      )}
    </>
  );
};

export default EligibleEmployeesGrid;
