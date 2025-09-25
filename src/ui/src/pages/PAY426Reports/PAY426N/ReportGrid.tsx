import { Box, CircularProgress, Typography } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import React, { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router-dom";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { FilterParams } from "reduxstore/types";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { RootState } from "../../../reduxstore/store";
import { GetProfitSharingReportGridColumns } from "./GetProfitSharingReportGridColumns";
import presets from "./presets";

interface ReportGridProps {
  params: FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const ReportGrid: React.FC<ReportGridProps> = ({ params, onLoadingChange }) => {
  const navigate = useNavigate();
  const [trigger, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const data = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReport);

  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "employeeName",
    initialSortDescending: false,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: any) => {
        if (hasToken && params) {
          const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
          trigger({
            profitYear: profitYear,
            pagination: {
              skip: pageNum * pageSz,
              take: pageSz,
              sortBy: sortPrms.sortBy,
              isSortDescending: sortPrms.isSortDescending
            },
            ...params,
            reportId: matchingPreset ? Number(matchingPreset.id) : 0
          });
        }
      },
      [hasToken, params, profitYear, trigger]
    )
  });

  const getCurrentPresetId = () => {
    const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
    return matchingPreset ? matchingPreset.id : "default";
  };

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
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: "employeeName",
          isSortDescending: false
        },
        ...params,
        reportId: matchingPreset ? Number(matchingPreset.id) : 0
      });
    }
  }, [trigger, hasToken, profitYear, params]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: any) => {
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
  }, [data, params]);

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
