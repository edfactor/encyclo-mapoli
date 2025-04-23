import { Typography } from "@mui/material";
import { useMemo, useEffect, useCallback, useState } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

const TwentyOnePlusGrid = () => {
  const navigate = useNavigate();

  const [trigger, { data, isFetching }] = useLazyGetYearEndProfitSharingReportQuery();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "lastName",
    isSortDescending: false
  });
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();

  const baseParams = {
    isYearEnd: true,
    minimumAgeInclusive: 21,
    minimumHoursInclusive: 1000,
    includeActiveEmployees: true,
    includeInactiveEmployees: true,
    includeEmployeesTerminatedThisYear: false,
    includeTerminatedEmployees: false,
    includeBeneficiaries: false,
    includeEmployeesWithPriorProfitSharingAmounts: true,
    includeEmployeesWithNoPriorProfitSharingAmounts: true,
  }

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

  const columnDefs = useMemo(
    () => GetProfitSharingReportGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "employeeName") {
      if (sortParams.sortBy === "lastName") {
        update.isSortDescending = !sortParams.isSortDescending;
      }
      update.sortBy = "lastName";
    }

    if (update.sortBy === "") { 
      update.sortBy = "lastName";
      update.isSortDescending = false;
    } 

    setSortParams(update);
    setPageNumber(0);

    trigger({
      profitYear: profitYear,
      pagination: {
        skip: 0,
        take: pageSize,
        sortBy: update.sortBy,
        isSortDescending: update.isSortDescending
      },
      ...baseParams
    });
  }

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
        isLoading={isFetching}
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

export default TwentyOnePlusGrid;
