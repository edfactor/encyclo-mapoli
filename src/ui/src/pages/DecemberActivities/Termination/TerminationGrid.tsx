import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, mmDDYYFormat, numberToCurrency, Pagination } from "smart-ui-library";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";
import { Typography } from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { ReportSummary } from "../../../components/ReportSummary";
import { TerminationRequest } from "reduxstore/types";

import { GetTerminationColumns } from "./TerminationGridColumn";

interface TerminationGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  searchParams: TerminationRequest | null;
}

const TerminationGrid: React.FC<TerminationGridSearchProps> = ({ initialSearchLoaded, setInitialSearchLoaded, searchParams }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const navigate = useNavigate();

  useEffect(() => {
    if (searchParams && hasToken) {
      const request = {
        ...searchParams,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };
      triggerSearch(request, false);
    }
  }, [searchParams, pageNumber, pageSize, sortParams, hasToken, triggerSearch]);

  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(() => GetTerminationColumns(handleNavigationForButton), [handleNavigationForButton]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {termination?.response && (
        <>
          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalEndingBalance || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalVested || 0)]]}
              leftColumnHeaders={["Vested Amount"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalForfeit || 0)]]}
              leftColumnHeaders={["Total Forfeitures"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalBeneficiaryAllocation || 0)]]}
              leftColumnHeaders={["Total Beneficiary Allocations"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>

          <ReportSummary report={termination} />
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: termination?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!termination && termination.response.results.length > 0 && (
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
          recordCount={termination.response.total}
        />
      )}
    </>
  );
};

export default TerminationGrid;
