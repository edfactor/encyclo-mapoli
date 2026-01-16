import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { Box, CircularProgress, Grid, IconButton, Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitSharingReportValidationQuery } from "reduxstore/api/ValidationApi";
import {
  useLazyGetYearEndProfitSharingReportFrozenQuery,
  useLazyGetYearEndProfitSharingReportLiveQuery
} from "reduxstore/api/YearsEndApi";
import { FilterParams } from "reduxstore/types";
import { ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { RootState } from "../../../../reduxstore/store";
import { ValidationResponse } from "../../../../types/validation/cross-reference-validation";
import { GetProfitSharingReportGridColumns } from "./GetProfitSharingReportGridColumns";
import presets from "./presets";

interface ReportGridProps {
  params: FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
  isFrozen: boolean;
  searchTrigger: number;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
  profitYear: number;
}

const ReportGrid: React.FC<ReportGridProps> = ({
  params,
  onLoadingChange,
  isFrozen,
  searchTrigger,
  isGridExpanded = false,
  onToggleExpand,
  profitYear
}) => {
  const [triggerLive, { isFetching: isFetchingLive }] = useLazyGetYearEndProfitSharingReportLiveQuery();
  const [triggerFrozen, { isFetching: isFetchingFrozen }] = useLazyGetYearEndProfitSharingReportFrozenQuery();
  const [triggerValidation] = useLazyGetProfitSharingReportValidationQuery();
  const [validationData, setValidationData] = useState<ValidationResponse | null>(null);
  const trigger = isFrozen ? triggerFrozen : triggerLive;
  const isFetching = isFrozen ? isFetchingFrozen : isFetchingLive;
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const liveData = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReportLive);
  const frozenData = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReportFrozen);
  const data = isFrozen ? frozenData : liveData;

  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "fullName",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.PAY426N_REPORT,
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
          sortBy: "fullName",
          isSortDescending: false
        },
        reportId: matchingPreset ? Number(matchingPreset.id) : 0
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [trigger, hasToken, profitYear, params, searchTrigger]);

  // Fetch validation checksum data when preset changes (only for presets 1-8)
  useEffect(() => {
    if (hasToken && params) {
      const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
      const presetNumber = matchingPreset ? Number(matchingPreset.id) : 0;

      if (presetNumber >= 1 && presetNumber <= 8) {
        triggerValidation({ profitYear, reportSuffix: presetNumber, useFrozenData: isFrozen })
          .unwrap()
          .then((response) => {
            setValidationData(response);
          })
          .catch(() => {
            setValidationData(null);
          });
      } else {
        setValidationData(null);
      }
    }
  }, [hasToken, isFrozen, params, profitYear, triggerValidation]);

  const sortEventHandler = (update: ISortParams) => {
    handleSortChange(update);
  };

  const columnDefs = useMemo(
    () => GetProfitSharingReportGridColumns(validationData),
    [validationData]
  );

  const pinnedTopRowData = useMemo(() => {
    if (!data) return [];

    return [
      {
        fullName: `TOTAL EMPS: ${data.numberOfEmployees || 0}`,
        wages: data.wagesTotal || 0,
        hours: data.hoursTotal || 0,
        points: data.pointsTotal || 0,
        balance: data.balanceTotal || 0,
        isNew: data.numberOfNewEmployees || 0,
        // Flag to identify pinned total row for cell renderers
        _isPinnedTotal: true
      },
      {
        badgeNumber: 0,
        fullName: "No Wages",
        storeNumber: 0,
        employeeTypeCode: "",
        employmentTypeName: "",
        dateOfBirth: null,
        age: "",
        ssn: "",
        wages: 0,
        hours: 0,
        points: 0,
        isUnder21: false,
        isNew: false,
        employeeStatus: "",
        balance: 0,
        yearsInPlan: 0,
        terminationDate: null
      }
    ];
  }, [data]);

  return (
    <>
      <Grid
        container
        justifyContent="space-between"
        alignItems="center"
        marginBottom={2}
        paddingX="24px">
        <Grid>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5" }}>
            {`${getReportTitle()} (${data?.response?.total || 0} records)`}
          </Typography>
        </Grid>
        <Grid>
          {onToggleExpand && (
            <IconButton
              onClick={onToggleExpand}
              sx={{ zIndex: 1 }}
              aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}>
              {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
            </IconButton>
          )}
        </Grid>
      </Grid>

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <DSMPaginatedGrid
          preferenceKey={GRID_KEYS.PAY426N_REPORT}
          data={data?.response?.results || []}
          columnDefs={columnDefs}
          totalRecords={data?.response?.total || 0}
          isLoading={isFetching}
          pagination={{
            pageNumber,
            pageSize,
            sortParams: { sortBy: "fullName", isSortDescending: false },
            handlePageNumberChange,
            handlePageSizeChange,
            handleSortChange
          }}
          onSortChange={sortEventHandler}
          heightConfig={{
            mode: "content-aware",
            heightPercentage: isGridExpanded ? 0.85 : 0.65,
            minHeight: 200,
            pinnedRowCount: 2 // Account for 2 pinned top rows (totals + "No Wages")
          }}
          gridOptions={{
            pinnedTopRowData: pinnedTopRowData,
            getRowStyle: (params) => {
              if (params.node.rowPinned) {
                return { background: "#f3f4f6" }; // Light grey background for pinned rows
              }
              return undefined;
            },
            onGridReady: (params) => {
              setTimeout(() => params.api.sizeColumnsToFit(), 100);
            },
            onFirstDataRendered: (params) => {
              params.api.sizeColumnsToFit();
            },
            onGridSizeChanged: (params) => {
              params.api.sizeColumnsToFit();
            }
          }}
          showPagination={!!data && data.response.results.length > 0}
        />
      )}
    </>
  );
};

export default ReportGrid;
