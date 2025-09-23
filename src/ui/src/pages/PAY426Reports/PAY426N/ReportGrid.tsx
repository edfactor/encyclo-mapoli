import { Box, CircularProgress, Typography } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router-dom";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { FilterParams } from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { RootState } from "../../../reduxstore/store";
import pay426Utils from "../Pay427Utils";
import { GetProfitSharingReportGridColumns } from "./GetProfitSharingReportGridColumns";
import presets from "./presets";

interface ReportGridProps {
  params: FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const ReportGrid: React.FC<ReportGridProps> = ({ params, onLoadingChange }) => {
  const navigate = useNavigate();

  const [pageNumberByPreset, setPageNumberByPreset] = useState<{ [key: string]: number }>({});
  const [pageSizeByPreset, setPageSizeByPreset] = useState<{ [key: string]: number }>({});

  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "employeeName",
    isSortDescending: false
  });

  const [trigger, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();

  const data = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReport);

  const getCurrentPresetId = () => {
    const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
    return matchingPreset ? matchingPreset.id : "default";
  };

  const getCurrentPageNumber = () => {
    const presetId = getCurrentPresetId();
    return pageNumberByPreset[presetId] ?? 0;
  };

  const getCurrentPageSize = () => {
    const presetId = getCurrentPresetId();
    return pageSizeByPreset[presetId] ?? 25;
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
      const currentPageNumber = getCurrentPageNumber();
      const currentPageSize = getCurrentPageSize();
      console.log("Fetching data with params:", {
        profitYear: profitYear,
        pagination: {
          skip: currentPageNumber * currentPageSize,
          take: currentPageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        },
        ...params,
        reportId: matchingPreset ? Number(matchingPreset.id) : 0
      });
      trigger({
        profitYear: profitYear,
        pagination: {
          skip: currentPageNumber * currentPageSize,
          take: currentPageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        },
        ...params,
        reportId: matchingPreset ? Number(matchingPreset.id) : 0
      });
    }
  }, [trigger, hasToken, profitYear, pageNumberByPreset, pageSizeByPreset, sortParams, params]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));
    const currentPageSize = getCurrentPageSize();
    const presetId = getCurrentPresetId();
    const t = () => {
      trigger({
        profitYear: profitYear,
        pagination: {
          skip: 0,
          take: currentPageSize,
          sortBy: update.sortBy,
          isSortDescending: update.isSortDescending
        },
        ...params,
        reportId: matchingPreset ? Number(matchingPreset.id) : 0
      });
    };

    const setPageNumberForPreset = (value: React.SetStateAction<number>) => {
      setPageNumberByPreset((prev) => ({
        ...prev,
        [presetId]: typeof value === "function" ? value(prev[presetId] ?? 0) : value
      }));
    };

    pay426Utils.sortEventHandler(update, sortParams, setSortParams, setPageNumberForPreset, t);
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
              pageNumber={getCurrentPageNumber()}
              setPageNumber={(value: number) => {
                const presetId = getCurrentPresetId();
                setPageNumberByPreset((prev) => ({ ...prev, [presetId]: value - 1 }));
              }}
              pageSize={getCurrentPageSize()}
              setPageSize={(value: number) => {
                const presetId = getCurrentPresetId();
                setPageSizeByPreset((prev) => ({ ...prev, [presetId]: value }));
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
