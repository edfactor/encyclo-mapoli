import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router";
import { useLazyGetProfitSharingLabelsQuery } from "reduxstore/api/YearsEndApi";

import { useFakeTimeAwareYear } from "hooks/useFakeTimeAwareDate";
import useNavigationYear from "hooks/useNavigationYear";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { ProfitSharingLabel } from "../../../types";
import { GetProfallGridColumns } from "./ProfallGridColumns";

interface ProfallGridProps {
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfallGrid: React.FC<ProfallGridProps> = ({ pageNumberReset, setPageNumberReset }) => {
  const navigate = useNavigate();
  const profitSharingLabels = useSelector((state: RootState) => state.yearsEnd.profitSharingLabels);
  const securityState = useSelector((state: RootState) => state.security);
  const [getProfitSharingLabels, { isFetching }] = useLazyGetProfitSharingLabelsQuery();
  const profitYear = useNavigationYear();
  const currentYear = useFakeTimeAwareYear();

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.PROFALL_REPORT,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (profitYear && securityState.token) {
          const yearToUse = profitYear || currentYear;
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
      [profitYear, currentYear, securityState.token, getProfitSharingLabels]
    )
  });

  // Destructure for internal use
  const { pageNumber, pageSize, sortParams, resetPagination } = pagination;

  const fetchData = useCallback(() => {
    const yearToUse = profitYear || currentYear;
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
  }, [profitYear, currentYear, pageNumber, pageSize, sortParams, getProfitSharingLabels]);

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
    <DSMPaginatedGrid<ProfitSharingLabel>
      preferenceKey={GRID_KEYS.PROFALL_REPORT}
      data={rowData}
      columnDefs={columnDefs}
      totalRecords={recordCount}
      isLoading={isFetching}
      pagination={pagination}
      onSortChange={pagination.handleSortChange}
      showPagination={recordCount > 0}
      header={
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`PROFALL REPORT (${recordCount} records)`}
        </Typography>
      }
      slotClassNames={{ headerClassName: "px-6" }}
    />
  );
};

export default ProfallGrid;
