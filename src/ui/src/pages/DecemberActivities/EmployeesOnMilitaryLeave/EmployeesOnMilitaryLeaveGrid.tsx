import { Typography } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEmployeesOnMilitaryLeaveQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMilitaryAndRehireColumns } from "./EmployeesOnMilitaryLeaveGridColumns";
import { CAPTIONS } from "../../../constants";

const EmployeesOnMilitaryLeaveGrid: React.FC = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(100);
  const [_, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const [triggerSearch, { isFetching }] = useLazyGetEmployeesOnMilitaryLeaveQuery();

  useEffect(() => {
    const fetchData = async () => {
      const request = {
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: "Badge", // Default sortBy value
          isSortDescending: false // Default sort order
        }
      };

      await triggerSearch(request, false);
    };

    fetchData();
  }, [pageNumber, pageSize, triggerSearch]);

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
              {`(${employeesOnMilitaryLeave?.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.EMPLOYEES_MILITARY}
            isLoading={isFetching}
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
