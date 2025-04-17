import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

const PriorHoursGrid = () => {
  const navigate = useNavigate();
  const [trigger, { data, isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (hasToken) {
      trigger({
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumAgeInclusive: 200,
      minimumHoursInclusive: 0,
      maximumHoursInclusive: 999.99,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: false,
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
          {`ACTIVE/INACTIVE WITH PRIOR PS REPORT (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING}
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

export default PriorHoursGrid;
