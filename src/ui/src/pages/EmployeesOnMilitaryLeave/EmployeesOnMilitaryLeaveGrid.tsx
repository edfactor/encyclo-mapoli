import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEmployeesOnMilitaryLeaveQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMilitaryAndRehireColumns } from "./EmployeesOnMilitaryLeaveGridColumns";

interface EmployeesOnMilitaryLeaveGridProps {
  profitYearCurrent: number | null;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const EmployeesOnMilitaryLeaveGrid: React.FC<EmployeesOnMilitaryLeaveGridProps> = ({
  profitYearCurrent,
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });
  const [triggerSearch, { isFetching }] = useLazyGetEmployeesOnMilitaryLeaveQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYearCurrent ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [profitYearCurrent, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  const { militaryAndRehire: employeesOnMilitaryLeave } = useSelector((state: RootState) => state.yearsEnd);

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
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={employeesOnMilitaryLeave.response.total}
        />
      )}
    </>
  );
};

export default EmployeesOnMilitaryLeaveGrid;
