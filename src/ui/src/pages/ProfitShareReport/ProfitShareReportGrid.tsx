import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { YearEndProfitSharingReportRequest } from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumn";
import { CAPTIONS } from "../../constants";

interface ProfitShareReportGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ProfitShareReportGrid: React.FC<ProfitShareReportGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { yearEndProfitSharingReport, yearEndProfitSharingReportQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  const onSearch = useCallback(async () => {
    const request: YearEndProfitSharingReportRequest = {
      isYearEnd: false,
      minimumAgeInclusive: 18,
      maximumAgeInclusive: 98,
      minimumHoursInclusive: 1000,
      maximumHoursInclusive: 2000,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
      profitYear: yearEndProfitSharingReportQueryParams?.profitYear ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize },
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, triggerSearch, yearEndProfitSharingReportQueryParams?.profitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  return (
    <>
      {!!yearEndProfitSharingReport && (
        <DSMGrid
          preferenceKey={"ProfitShareReportGrid"}
          isLoading={isLoading}
          providedOptions={{
            rowData: yearEndProfitSharingReport?.response.results,
            columnDefs: columnDefs
          }}
        />
      )}
      {!!yearEndProfitSharingReport && yearEndProfitSharingReport.response.results.length > 0 && (
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
          recordCount={yearEndProfitSharingReport.response.total}
        />
      )}
    </>
  );
};

export default ProfitShareReportGrid;
