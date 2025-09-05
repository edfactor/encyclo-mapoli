import React, { useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEmployeesOnMilitaryLeaveQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { CAPTIONS } from "../../../constants";
import { GetRehireColumns } from "./EmployeesOnMilitaryLeaveGridColumns";

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
          sortBy: "badgeNumber", // Default sortBy value
          isSortDescending: false // Default sort order
        }
      };

      await triggerSearch(request, false);
    };

    fetchData();
  }, [pageNumber, pageSize, triggerSearch]);

  const { rehire: employeesOnMilitaryLeave } = useSelector((state: RootState) => state.yearsEnd);

  // Need a useEffect on a change in employeesOnMilitaryLeave to reset the page number
  const prevEmployeesOnMilitaryLeave = useRef<any>(null);
  useEffect(() => {
    if (
      employeesOnMilitaryLeave !== prevEmployeesOnMilitaryLeave.current &&
      employeesOnMilitaryLeave?.response?.results &&
      employeesOnMilitaryLeave.response.results.length !==
        prevEmployeesOnMilitaryLeave.current?.response?.results?.length
    ) {
      setPageNumber(0);
    }
    prevEmployeesOnMilitaryLeave.current = employeesOnMilitaryLeave;
  }, [employeesOnMilitaryLeave]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetRehireColumns(), []);

  return (
    <>
      {employeesOnMilitaryLeave?.response && (
        <>
          <ReportSummary report={employeesOnMilitaryLeave} />
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
