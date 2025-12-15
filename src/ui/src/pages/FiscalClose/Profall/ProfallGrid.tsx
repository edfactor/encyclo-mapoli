import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { useLazyGetProfitSharingLabelsQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";

import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetProfallGridColumns } from "./ProfallGridColumns";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { GRID_KEYS } from "../../../constants";
import { useGridPagination, SortParams } from "../../../hooks/useGridPagination";

interface ProfallGridProps {
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfallGrid: React.FC<ProfallGridProps> = ({ pageNumberReset, setPageNumberReset }) => {
  const navigate = useNavigate();
  const profitSharingLabels = useSelector((state: RootState) => state.yearsEnd.profitSharingLabels);
  const securityState = useSelector((state: RootState) => state.security);
  const [getProfitSharingLabels, { isFetching }] = useLazyGetProfitSharingLabelsQuery();
  const profitYear = useFiscalCloseProfitYear();

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "badgeNumber",
      initialSortDescending: false,
      persistenceKey: GRID_KEYS.PROFALL_REPORT,
      onPaginationChange: useCallback(
        (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (profitYear && securityState.token) {
            const yearToUse = profitYear || new Date().getFullYear();
            const skip = pageNum * pageSz;
            getProfitSharingLabels({
              profitYear: yearToUse,
              useFrozenData: true,
              pagination: {
                take: pageSz,
                skip: skip,
                sortBy: sortPrms.sortBy || "badgeNumber",
                isSortDescending: sortPrms.isSortDescending
              }
            });
          }
        },
        [profitYear, securityState.token, getProfitSharingLabels]
      )
    });

  const fetchData = useCallback(() => {
    const yearToUse = profitYear || new Date().getFullYear();
    const skip = pageNumber * pageSize;
    getProfitSharingLabels({
      profitYear: yearToUse,
      // This needs to be the default as the page has no search filters
      // but this is required by the API
      useFrozenData: true,
      pagination: {
        take: pageSize,
        skip: skip,
        sortBy: sortParams.sortBy || "badgeNumber",
        isSortDescending: sortParams.isSortDescending
      }
    });
  }, [profitYear, pageNumber, pageSize, sortParams, getProfitSharingLabels]);

  useEffect(() => {
    if (profitYear && securityState.token) {
      fetchData();
    }
  }, [profitYear, securityState.token, fetchData]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  const columnDefs = useMemo(() => GetProfallGridColumns(handleNavigationForButton), [handleNavigationForButton]);

  const rowData = useMemo(() => {
    return profitSharingLabels?.results || [];
  }, [profitSharingLabels]);

  const recordCount = profitSharingLabels?.total || 0;

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`PROFALL REPORT (${recordCount} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={GRID_KEYS.PROFALL_REPORT}
        isLoading={isFetching}
        handleSortChanged={handleSortChange}
        providedOptions={{
          rowData: rowData,
          columnDefs: columnDefs
        }}
      />
      {recordCount > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={recordCount}
        />
      )}
    </>
  );
};

export default ProfallGrid;
