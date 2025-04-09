import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { DSMGrid } from "smart-ui-library";
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
          skip: 0,
          take: 25,
          sortBy: "badgeNumber",
          isSortDescending: true
        }
      });
    }
  }, [trigger, hasToken, profitYear]);

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
        preferenceKey={"ELIGIBLE_EMPLOYEES"}
        isLoading={isLoading}
        handleSortChanged={(_params) => {}}
        providedOptions={{
          rowData: data?.response?.results || [],
          columnDefs: columnDefs
        }}
      />
    </>
  );
};

export default EighteenToTwentyGrid;
