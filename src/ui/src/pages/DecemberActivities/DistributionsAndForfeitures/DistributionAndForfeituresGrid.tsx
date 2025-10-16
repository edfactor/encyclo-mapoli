import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { DSMGrid, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { useLazyGetDistributionsAndForfeituresQuery } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import { GetDistributionsAndForfeituresColumns } from "./DistributionAndForfeituresGridColumns";

interface DistributionsAndForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const DistributionsAndForfeituresGrid: React.FC<DistributionsAndForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [showTooltip, setShowTooltip] = useState(false);
  const [hoverTimeout, setHoverTimeout] = useState<NodeJS.Timeout | null>(null);
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { distributionsAndForfeitures, distributionsAndForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsAndForfeituresQuery();

  // Make the initial page size configurable via state so it can be updated if needed
  const [initialPageSize, setInitialPageSize] = useState<number>(25);

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize,
      initialSortBy: "employeeName, date",
      initialSortDescending: false,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: any) => {
          if (hasToken && initialSearchLoaded) {
            const request = {
              profitYear: profitYear || 0,
              ...(distributionsAndForfeituresQueryParams?.startDate && {
                startDate: distributionsAndForfeituresQueryParams?.startDate
              }),
              ...(distributionsAndForfeituresQueryParams?.endDate && {
                endDate: distributionsAndForfeituresQueryParams?.endDate
              }),
              ...(distributionsAndForfeituresQueryParams?.state && {
                state: distributionsAndForfeituresQueryParams?.state
              }),
              ...(distributionsAndForfeituresQueryParams?.taxCode && {
                taxCode: distributionsAndForfeituresQueryParams?.taxCode
              }),
              pagination: {
                skip: pageNum * pageSz,
                take: pageSz,
                sortBy: sortPrms.sortBy,
                isSortDescending: sortPrms.isSortDescending
              }
            };
            await triggerSearch(request, false);
          }
        },
        [
          hasToken,
          initialSearchLoaded,
          profitYear,
          distributionsAndForfeituresQueryParams?.startDate,
          distributionsAndForfeituresQueryParams?.endDate,
          distributionsAndForfeituresQueryParams?.state,
          distributionsAndForfeituresQueryParams?.taxCode,
          triggerSearch
        ]
      )
    });

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const handlePopoverOpen = () => {
    if (hoverTimeout) {
      clearTimeout(hoverTimeout);
      setHoverTimeout(null);
    }
    setShowTooltip(true);
  };

  const handlePopoverClose = () => {
    const timeout = setTimeout(() => {
      setShowTooltip(false);
    }, 100); // Small delay to prevent flickering
    setHoverTimeout(timeout);
  };

  const open = showTooltip;

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYear || 0,
      ...(distributionsAndForfeituresQueryParams?.startDate && {
        startDate: distributionsAndForfeituresQueryParams?.startDate
      }),
      ...(distributionsAndForfeituresQueryParams?.endDate && {
        endDate: distributionsAndForfeituresQueryParams?.endDate
      }),
      ...(distributionsAndForfeituresQueryParams?.state && {
        state: distributionsAndForfeituresQueryParams?.state
      }),
      ...(distributionsAndForfeituresQueryParams?.taxCode && {
        taxCode: distributionsAndForfeituresQueryParams?.taxCode
      }),
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };

    await triggerSearch(request, false);
  }, [
    distributionsAndForfeituresQueryParams?.endDate,
    distributionsAndForfeituresQueryParams?.startDate,
    distributionsAndForfeituresQueryParams?.state,
    distributionsAndForfeituresQueryParams?.taxCode,
    profitYear,
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch
  ]);

  // Reset pagination when search filters change (not when paginating through results)
  const prevQueryParams = useRef<any>(null);
  useEffect(() => {
    const currentQueryParams = {
      startDate: distributionsAndForfeituresQueryParams?.startDate,
      endDate: distributionsAndForfeituresQueryParams?.endDate,
      state: distributionsAndForfeituresQueryParams?.state,
      taxCode: distributionsAndForfeituresQueryParams?.taxCode,
      profitYear
    };

    if (
      prevQueryParams.current &&
      (prevQueryParams.current.startDate !== currentQueryParams.startDate ||
        prevQueryParams.current.endDate !== currentQueryParams.endDate ||
        prevQueryParams.current.state !== currentQueryParams.state ||
        prevQueryParams.current.taxCode !== currentQueryParams.taxCode ||
        prevQueryParams.current.profitYear !== currentQueryParams.profitYear)
    ) {
      resetPagination();
    }

    prevQueryParams.current = currentQueryParams;
  }, [
    distributionsAndForfeituresQueryParams?.startDate,
    distributionsAndForfeituresQueryParams?.endDate,
    distributionsAndForfeituresQueryParams?.state,
    distributionsAndForfeituresQueryParams?.taxCode,
    profitYear,
    resetPagination
  ]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, onSearch, hasToken]);

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (hoverTimeout) {
        clearTimeout(hoverTimeout);
      }
    };
  }, [hoverTimeout]);

  const sortEventHandler = (update: any) => handleSortChange(update);
  const columnDefs = useMemo(() => GetDistributionsAndForfeituresColumns(), []);

  return (
    <>
      {distributionsAndForfeitures?.response && (
        <>
          <div className="sticky top-0 z-10 flex items-start gap-2 bg-white py-2">
            <div className="flex-1">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.distributionTotal || 0)]]}
                leftColumnHeaders={["Distributions"]}
                topRowHeaders={[]}
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}></TotalsGrid>
            </div>
            <div className="relative flex-1">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.stateTaxTotal || 0)]]}
                leftColumnHeaders={["State Taxes"]}
                topRowHeaders={[]}
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}></TotalsGrid>
              {distributionsAndForfeitures.stateTaxTotals &&
                Object.keys(distributionsAndForfeitures.stateTaxTotals).length > 0 && (
                  <div
                    className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                    onMouseEnter={handlePopoverOpen}
                    onMouseLeave={handlePopoverClose}>
                    <InfoOutlinedIcon
                      className="cursor-pointer text-green-500"
                      fontSize="small"
                    />
                    {open && (
                      <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] max-w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
                        <div className="p-2 px-4 pb-4">
                          <Typography
                            variant="subtitle2"
                            sx={{ p: 1 }}>
                            State Tax Breakdown
                          </Typography>
                          <table className="w-full border-collapse text-[0.95rem]">
                            <thead>
                              <tr>
                                <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">State</th>
                                <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                  Tax Total
                                </th>
                              </tr>
                            </thead>
                            <tbody>
                              {Object.entries(distributionsAndForfeitures.stateTaxTotals).map(
                                ([state, total], index, array) => (
                                  <tr key={state}>
                                    <td
                                      className={`px-2 py-1 text-left ${index < array.length - 1 ? "border-b border-gray-100" : ""}`}>
                                      {state}
                                    </td>
                                    <td
                                      className={`px-2 py-1 text-right ${index < array.length - 1 ? "border-b border-gray-100" : ""}`}>
                                      {numberToCurrency(total as number)}
                                    </td>
                                  </tr>
                                )
                              )}
                            </tbody>
                          </table>
                        </div>
                      </div>
                    )}
                  </div>
                )}
            </div>
            <div className="flex-1">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.federalTaxTotal || 0)]]}
                leftColumnHeaders={["Federal Taxes"]}
                topRowHeaders={[]}
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}></TotalsGrid>
            </div>
            <div className="flex-1">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.forfeitureTotal || 0)]]}
                leftColumnHeaders={["Forfeitures"]}
                topRowHeaders={[]}
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}></TotalsGrid>
            </div>
          </div>

          <ReportSummary report={distributionsAndForfeitures} />
          <DSMGrid
            preferenceKey={CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: distributionsAndForfeitures?.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!distributionsAndForfeitures && distributionsAndForfeitures.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setInitialPageSize(value);
            handlePaginationChange(0, value);
            setInitialSearchLoaded(true);
          }}
          recordCount={distributionsAndForfeitures.response.total}
        />
      )}
    </>
  );
};

export default DistributionsAndForfeituresGrid;
