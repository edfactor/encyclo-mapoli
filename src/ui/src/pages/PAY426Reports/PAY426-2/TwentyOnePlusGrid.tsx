import { Typography } from "@mui/material";
import { useMemo, useEffect, useCallback, useState } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { CAPTIONS } from "../../../constants";

const TwentyOnePlusGrid = () => {
  const navigate = useNavigate();

  const [trigger, { data, isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useDecemberFlowProfitYear();

  useEffect(() => {
    if (hasToken) {
      trigger({
        isYearEnd: true,
        minimumAgeInclusive: 21,
        maximumAgeInclusive: 200,
        minimumHoursInclusive: 999.9,
        maximumHoursInclusive: 4000,
        includeActiveEmployees: true,
        includeInactiveEmployees: true,
        includeEmployeesTerminatedThisYear: false,
        includeTerminatedEmployees: false,
        includeBeneficiaries: false,
        includeEmployeesWithPriorProfitSharingAmounts: true,
        includeEmployeesWithNoPriorProfitSharingAmounts: true,
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }});
    }
  }, [trigger, hasToken, profitYear, pageNumber, pageSize, sortParams]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitSharingReportGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`ACTIVE/INACTIVE 21+ REPORT (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={CAPTIONS.PAY426_ACTIVE_21_PLUS}
        isLoading={isLoading}
        handleSortChanged={(_params) => {}}
        providedOptions={{
          rowData: data?.response?.results || [],
          columnDefs: columnDefs
        }}
      />
      {!!data && data.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => setPageNumber(value - 1)}
          pageSize={pageSize}
          setPageSize={setPageSize}
          recordCount={data.response.total}
        />
      )}
    </>
  );
};

export default TwentyOnePlusGrid;
