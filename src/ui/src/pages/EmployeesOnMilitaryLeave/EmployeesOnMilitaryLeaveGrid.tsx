import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetEmployeesOnMilitaryLeaveQuery, useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMilitaryAndRehireColumns } from "./EmployeesOnMilitaryLeaveGridColumns";

const EmployeesOnMilitaryLeaveGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const { militaryAndRehire: employeesOnMilitaryLeave } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetEmployeesOnMilitaryLeaveQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetMilitaryAndRehireColumns(), []);

  return (
    <>
      {employeesOnMilitaryLeave?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Employees on Military Leave (${employeesOnMilitaryLeave?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: employeesOnMilitaryLeave?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!employeesOnMilitaryLeave && employeesOnMilitaryLeave.response.results.length > 0 && (
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
          recordCount={employeesOnMilitaryLeave.response.total}
        />
      )}
    </>
  );
};

export default EmployeesOnMilitaryLeaveGrid;
