import { Typography } from "@mui/material";
import { useMemo, useEffect, useCallback, useState } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import { CAPTIONS, PAY426_REPORT_IDS } from "../../../constants";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import pay426Utils from "../Pay427Utils";

interface TermedWithPriorGridProps {
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const TermedWithPriorGrid: React.FC<TermedWithPriorGridProps> = ({ pageNumberReset, setPageNumberReset }) => {
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
    reportId: PAY426_REPORT_IDS.TERMINATED_WITH_PRIOR_PS,
    isYearEnd: true,
    minimumAgeInclusive: 18,
    maximumHoursInclusive: 1000,
    includeActiveEmployees: false,
    includeInactiveEmployees: false,
    includeEmployeesTerminatedThisYear: true,
    includeTerminatedEmployees: true,
    includeBeneficiaries: false,
    includeEmployeesWithPriorProfitSharingAmounts: true,
    includeEmployeesWithNoPriorProfitSharingAmounts: false,
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

  useEffect(() => {
    if (pageNumberReset) {
      setPageNumber(0);
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset]);

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

  const pinnedTopRowData = useMemo(() => {
    if (!data) return [];
    
   console.log("API data:", data);
    return [
      {
        employeeName: `TOTAL EMPS: ${data.numberOfEmployeesInPlan || 0}`,
        wages: data.wagesTotal || 0,
        hours: data.hoursTotal || 0,
        points: data.pointsTotal || 0,
        balance: data.balanceTotal || 0,
        isNew: data.numberOfNewEmployees || 0,
      },
      {
        employeeName: "No Wages",
        wages: 0,
        hours: 0,
        points: 0,
        balance: 0
      }
    ];
  }, [data]);

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`TERMED WITH PRIOR PS REPORT (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={CAPTIONS.PAY426_TERMINATED_PRIOR}
        isLoading={isFetching}
        handleSortChanged={sortEventHandler}
        providedOptions={{
          rowData: data?.response?.results || [],
          columnDefs: columnDefs,
          pinnedTopRowData: pinnedTopRowData
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

export default TermedWithPriorGrid;
