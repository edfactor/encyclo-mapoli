import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitSharingReportGridColumns } from "./EighteenToTwentyGridColumns";
import { useNavigate, Path } from "react-router";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { CAPTIONS } from "../../../constants";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

const EighteenToTwentyGrid = () => {
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
        minimumAgeInclusive: 17,
        maximumAgeInclusive: 20,
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

  // Wrapper to pass react function to non-react class
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
          {`${CAPTIONS.PAY426_ACTIVE_18_20} (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={CAPTIONS.PAY426_ACTIVE_18_20}
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

export default EighteenToTwentyGrid;
