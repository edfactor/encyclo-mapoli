import { Box, CircularProgress, Typography } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import React, { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router-dom";
import {
  useLazyGetYearEndProfitSharingReportFrozenQuery,
  useLazyGetYearEndProfitSharingReportLiveQuery
} from "reduxstore/api/YearsEndApi";
import { FilterParams } from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { RootState } from "../../../reduxstore/store";
import { GetProfitSharingReportGridColumns } from "./GetProfitSharingReportGridColumns";
import presets from "./presets";

interface ReportGridProps {
  params: FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
  isFrozen: boolean;
}

const ReportGrid: React.FC<ReportGridProps> = ({ params, onLoadingChange, isFrozen }) => {
  const navigate = useNavigate();
  const [triggerLive, { isFetching: isFetchingLive }] = useLazyGetYearEndProfitSharingReportLiveQuery();
  const [triggerFrozen, { isFetching: isFetchingFrozen }] = useLazyGetYearEndProfitSharingReportFrozenQuery();
  const trigger = isFrozen ? triggerFrozen : triggerLive;
  const isFetching = isFrozen ? isFetchingFrozen : isFetchingLive;
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const liveData = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReportLive);
  const frozenData = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReportFrozen);
  const data = isFrozen ? frozenData : liveData;

  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "employeeName",
    initialSortDescending: false,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (hasToken && params) {
          const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
          trigger({
            ...params,
            profitYear: profitYear,
            pagination: {
              skip: pageNum * pageSz,
              take: pageSz,
              sortBy: sortPrms.sortBy,
              isSortDescending: sortPrms.isSortDescending
            },
            reportId: matchingPreset ? Number(matchingPreset.id) : 0
          });
        }
      },
      [hasToken, params, profitYear, trigger]
    )
  });
  /*
  const getCurrentPresetId = () => {
    const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
    return matchingPreset ? matchingPreset.id : "default";
  };
  */
  // Notify parent component about loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  const getReportTitle = () => {
    const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));

    if (matchingPreset) {
      return matchingPreset.description.toUpperCase();
    }

    return "N/A";
  };

  useEffect(() => {
    if (hasToken && params) {
      const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));

      trigger({
        ...params,
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: "employeeName",
          isSortDescending: false
        },
        reportId: matchingPreset ? Number(matchingPreset.id) : 0
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [trigger, hasToken, profitYear, params]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    handleSortChange(update);
  };

  const columnDefs = useMemo(
    () => GetProfitSharingReportGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const pinnedTopRowData = useMemo(() => {
    if (!data) return [];

    return [
      {
        employeeName: `TOTAL EMPS: ${data.numberOfEmployees || 0}`,
        wages: data.wagesTotal || 0,
        hours: data.hoursTotal || 0,
        points: data.pointsTotal || 0,
        balance: data.balanceTotal || 0,
        isNew: data.numberOfNewEmployees || 0
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
          {`${getReportTitle()} (${data?.response?.total || 0} records)`}
        </Typography>
      </div>

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <DSMGrid
            preferenceKey="PAY426N_REPORT"
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
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, pageSize);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                handlePaginationChange(0, value);
              }}
              recordCount={data.response.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default ReportGrid;
