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
import pay426Utils from "../Pay427Utils";

const UnderEighteenGrid = () => {
  const navigate = useNavigate();
  const [trigger, { data, isFetching }] = useLazyGetYearEndProfitSharingReportQuery();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const baseParams = {
    isYearEnd: true,
    maximumAgeInclusive: 17,
    includeActiveEmployees: true,
    includeInactiveEmployees: true,
    includeEmployeesTerminatedThisYear: false,
    includeTerminatedEmployees: false,
    includeBeneficiaries: false,
    includeEmployeesWithPriorProfitSharingAmounts: true,
    includeEmployeesWithNoPriorProfitSharingAmounts: true,
  };

  useEffect(() => {
    if (hasToken) {
      trigger({
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        },
        ...baseParams
      });
    }
  }, [trigger, hasToken, profitYear, pageNumber, pageSize, sortParams]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
      const t = () => { 
          trigger({
            profitYear: profitYear,
            pagination: {
              skip: 0,
              take: pageSize,
              sortBy: update.sortBy,
              isSortDescending: update.isSortDescending
            },
            ...baseParams
          }
        );
      }
  
      pay426Utils.sortEventHandler(
        update,
        sortParams,
        setSortParams,
        setPageNumber,
        t
      );
    }

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
          {`UNDER 18 REPORT (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={CAPTIONS.PAY426_ACTIVE_UNDER_18}
        isLoading={isLoading}
        handleSortChanged={sortEventHandler}
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

export default UnderEighteenGrid;
