import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { YearEndProfitSharingReportRequest } from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumn";

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
    sortBy: "Badge",
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
      isYearEnd: true,
      minimumAgeInclusive: 18,
      minimumHoursInclusive: 1000,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesWithPriorProfitSharingAmounts: false,
      includeEmployeesWithNoPriorProfitSharingAmounts: false,
      profitYear: yearEndProfitSharingReportQueryParams?.profitYear ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize },
      maximumAgeInclusive: 0,
      maximumHoursInclusive: 0,
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
        <div style={{ padding: "0 24px 0 24px" }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5" }}>
            {`PROFIT-ELIGIBLE REPORT (5)`}
          </Typography>
        </div>
      )}
      {!!yearEndProfitSharingReport && yearEndProfitSharingReport.response.results.length && (
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
