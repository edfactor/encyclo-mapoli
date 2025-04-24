import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router-dom";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { GetBeneficiariesGridColumns } from "./BeneficiariesGridColumns";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import pay426Utils from "../Pay427Utils";

const BeneficiariesGrid = () => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const [trigger, { data, isFetching, error }] = useLazyGetYearEndProfitSharingReportQuery();

  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const baseParams = {
    isYearEnd: true,
    includeActiveEmployees: false,
    includeInactiveEmployees: false,
    includeEmployeesTerminatedThisYear: false,
    includeTerminatedEmployees: false,
    includeBeneficiaries: true,
    includeEmployeesWithPriorProfitSharingAmounts: false,
    includeEmployeesWithNoPriorProfitSharingAmounts: false,
  }

  useEffect(() => {
    if (hasToken) {
      trigger({
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: "badgeNumber",
          isSortDescending: true
        },
        ...baseParams
      });
    }
  }, [trigger, pageNumber, pageSize, profitYear, hasToken]);

  const getPinnedTopRowData = useMemo(() => {
    if (!data) return [];

    const beneficiaryCount = data.numberOfEmployees || 0;
    const wagesTotal = data.wagesTotal || 0;

    // @D - TODO - Temporary client side calculation, should this be coming from the API??
    let balanceTotal = 0;
    if (data.response?.results) {
      balanceTotal = data.response.results.reduce((total, curr) => total + (curr.balance || 0), 0);
    }

    return [
      {
        employeeName: `Total Non-EMPs Beneficiaries`,
        storeNumber: beneficiaryCount,
        wages: wagesTotal,
        balance: balanceTotal
      },
      {
        employeeName: "No Wages",
        storeNumber: 0,
        wages: 0,
        balance: 0
      }
    ];
  }, [data]);

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

  const columnDefs = useMemo(() => GetBeneficiariesGridColumns(handleNavigationForButton), [handleNavigationForButton]);

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`NON-EMPLOYEE BENEFICIARIES (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={CAPTIONS.PAY426_NON_EMPLOYEE}
        isLoading={isFetching}
        handleSortChanged={sortEventHandler}
        providedOptions={{
          rowData: data?.response?.results || [],
          pinnedTopRowData: getPinnedTopRowData,
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

export default BeneficiariesGrid;
