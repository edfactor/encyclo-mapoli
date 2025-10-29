import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { DSMGrid, ISortParams, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { useLazyGetDistributionsAndForfeituresQuery } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import { GetDistributionsAndForfeituresColumns } from "./DistributionsAndForfeituresGridColumns";

interface DistributionsAndForfeituresQueryParams {
  startDate?: string;
  endDate?: string;
  states?: string; // Stringified for comparison
  taxCodes?: string; // Stringified for comparison
  profitYear?: number;
}

interface DistributionsAndForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onLoadingChange?: (isLoading: boolean) => void;
}

const DistributionsAndForfeituresGrid: React.FC<DistributionsAndForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  onLoadingChange
}) => {
  const [showStateTaxTooltip, setShowStateTaxTooltip] = useState(false);
  const [showForfeitureTooltip, setShowForfeitureTooltip] = useState(false);
  const [stateTaxTimeout, setStateTaxTimeout] = useState<NodeJS.Timeout | null>(null);
  const [forfeitureTimeout, setForfeitureTimeout] = useState<NodeJS.Timeout | null>(null);
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
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (hasToken && initialSearchLoaded) {
            const request = {
              profitYear: profitYear || 0,
              ...(distributionsAndForfeituresQueryParams?.startDate && {
                startDate: distributionsAndForfeituresQueryParams?.startDate
              }),
              ...(distributionsAndForfeituresQueryParams?.endDate && {
                endDate: distributionsAndForfeituresQueryParams?.endDate
              }),
              ...(distributionsAndForfeituresQueryParams?.states &&
                distributionsAndForfeituresQueryParams.states.length > 0 && {
                  states: distributionsAndForfeituresQueryParams?.states
                }),
              ...(distributionsAndForfeituresQueryParams?.taxCodes &&
                distributionsAndForfeituresQueryParams.taxCodes.length > 0 && {
                  taxCodes: distributionsAndForfeituresQueryParams?.taxCodes
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
          distributionsAndForfeituresQueryParams?.states,
          distributionsAndForfeituresQueryParams?.taxCodes,
          triggerSearch
        ]
      )
    });

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const handleStateTaxPopoverOpen = () => {
    if (stateTaxTimeout) {
      clearTimeout(stateTaxTimeout);
      setStateTaxTimeout(null);
    }
    setShowStateTaxTooltip(true);
  };

  const handleStateTaxPopoverClose = () => {
    const timeout = setTimeout(() => {
      setShowStateTaxTooltip(false);
    }, 100);
    setStateTaxTimeout(timeout);
  };

  const handleForfeiturePopoverOpen = () => {
    if (forfeitureTimeout) {
      clearTimeout(forfeitureTimeout);
      setForfeitureTimeout(null);
    }
    setShowForfeitureTooltip(true);
  };

  const handleForfeiturePopoverClose = () => {
    const timeout = setTimeout(() => {
      setShowForfeitureTooltip(false);
    }, 100);
    setForfeitureTimeout(timeout);
  };

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYear || 0,
      ...(distributionsAndForfeituresQueryParams?.startDate && {
        startDate: distributionsAndForfeituresQueryParams?.startDate
      }),
      ...(distributionsAndForfeituresQueryParams?.endDate && {
        endDate: distributionsAndForfeituresQueryParams?.endDate
      }),
      states: distributionsAndForfeituresQueryParams?.states || [],
      taxCodes: distributionsAndForfeituresQueryParams?.taxCodes || [],
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
    distributionsAndForfeituresQueryParams?.states,
    distributionsAndForfeituresQueryParams?.taxCodes,
    profitYear,
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch
  ]);

  // Reset pagination when search filters change (not when paginating through results)
  const prevQueryParams = useRef<DistributionsAndForfeituresQueryParams | null>(null);
  useEffect(() => {
    const currentQueryParams = {
      startDate: distributionsAndForfeituresQueryParams?.startDate,
      endDate: distributionsAndForfeituresQueryParams?.endDate,
      states: JSON.stringify(distributionsAndForfeituresQueryParams?.states || []),
      taxCodes: JSON.stringify(distributionsAndForfeituresQueryParams?.taxCodes || []),
      profitYear
    };

    if (
      prevQueryParams.current &&
      (prevQueryParams.current.startDate !== currentQueryParams.startDate ||
        prevQueryParams.current.endDate !== currentQueryParams.endDate ||
        prevQueryParams.current.states !== currentQueryParams.states ||
        prevQueryParams.current.taxCodes !== currentQueryParams.taxCodes ||
        prevQueryParams.current.profitYear !== currentQueryParams.profitYear)
    ) {
      resetPagination();
    }

    prevQueryParams.current = currentQueryParams;
  }, [
    distributionsAndForfeituresQueryParams?.startDate,
    distributionsAndForfeituresQueryParams?.endDate,
    distributionsAndForfeituresQueryParams?.states,
    distributionsAndForfeituresQueryParams?.taxCodes,
    profitYear,
    resetPagination
  ]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, onSearch, hasToken]);

  // Notify parent of loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Cleanup timeouts on unmount
  useEffect(() => {
    return () => {
      if (stateTaxTimeout) {
        clearTimeout(stateTaxTimeout);
      }
      if (forfeitureTimeout) {
        clearTimeout(forfeitureTimeout);
      }
    };
  }, [stateTaxTimeout, forfeitureTimeout]);

  const sortEventHandler = (update: ISortParams) => handleSortChange(update);
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
                    onMouseEnter={handleStateTaxPopoverOpen}
                    onMouseLeave={handleStateTaxPopoverClose}>
                    <InfoOutlinedIcon
                      className="cursor-pointer text-green-500"
                      fontSize="small"
                    />
                    <div
                      className={`absolute left-0 top-full z-[1000] mt-1 max-h-[300px] max-w-[480px] overflow-auto rounded border border-gray-300 bg-white shadow-lg ${!showStateTaxTooltip ? "hidden" : ""}`}>
                      <div className="p-3 px-4 pb-4">
                        <Typography
                          variant="subtitle2"
                          sx={{ p: 1, pb: 0.5 }}>
                          State Tax Breakdown
                        </Typography>
                        <table className="w-full border-collapse text-[0.9rem]">
                          <thead>
                            <tr>
                              <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">State</th>
                              <th className="whitespace-nowrap border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                Tax Total
                              </th>
                            </tr>
                          </thead>
                          <tbody>
                            {Object.entries(distributionsAndForfeitures.stateTaxTotals).map(
                              ([state, total], index, array) => (
                                <tr key={state}>
                                  <td
                                    className={`whitespace-nowrap px-2 py-2 text-left ${index < array.length - 1 ? "border-b border-gray-100" : ""}`}>
                                    {state}
                                  </td>
                                  <td
                                    className={`whitespace-nowrap px-2 py-2 text-right ${index < array.length - 1 ? "border-b border-gray-100" : ""}`}>
                                    {numberToCurrency(total as number)}
                                  </td>
                                </tr>
                              )
                            )}
                          </tbody>
                        </table>
                      </div>
                    </div>
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
            <div className="relative flex-1">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.forfeitureTotal || 0)]]}
                leftColumnHeaders={["Forfeitures"]}
                topRowHeaders={[]}
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}></TotalsGrid>
              {(distributionsAndForfeitures.forfeitureRegularTotal ||
                distributionsAndForfeitures.forfeitureAdministrativeTotal ||
                distributionsAndForfeitures.forfeitureClassActionTotal) && (
                <div
                  className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                  onMouseEnter={handleForfeiturePopoverOpen}
                  onMouseLeave={handleForfeiturePopoverClose}>
                  <InfoOutlinedIcon
                    className="cursor-pointer text-blue-500"
                    fontSize="small"
                  />
                  <div
                    className={`absolute right-0 top-full z-[1000] mt-1 max-h-[300px] max-w-[480px] overflow-auto rounded border border-gray-300 bg-white shadow-lg ${!showForfeitureTooltip ? "hidden" : ""}`}>
                    <div className="p-3 px-4 pb-4">
                      <Typography
                        variant="subtitle2"
                        sx={{ p: 1, pb: 0.5 }}>
                        Forfeiture Breakdown
                      </Typography>
                      <table className="w-full border-collapse text-[0.9rem]">
                        <thead>
                          <tr>
                            <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Type</th>
                            <th className="whitespace-nowrap border-b border-gray-300 px-2 py-1 text-right font-semibold">
                              Total
                            </th>
                          </tr>
                        </thead>
                        <tbody>
                          {[
                            distributionsAndForfeitures.forfeitureRegularTotal && (
                              <tr key="regular">
                                <td className="whitespace-nowrap border-b border-gray-100 px-2 py-2 text-left">
                                  Regular
                                </td>
                                <td className="whitespace-nowrap border-b border-gray-100 px-2 py-2 text-right">
                                  {numberToCurrency(distributionsAndForfeitures.forfeitureRegularTotal)}
                                </td>
                              </tr>
                            ),
                            distributionsAndForfeitures.forfeitureAdministrativeTotal && (
                              <tr key="administrative">
                                <td className="whitespace-nowrap border-b border-gray-100 px-2 py-2 text-left">
                                  Administrative (A)
                                </td>
                                <td className="whitespace-nowrap border-b border-gray-100 px-2 py-2 text-right">
                                  {numberToCurrency(distributionsAndForfeitures.forfeitureAdministrativeTotal)}
                                </td>
                              </tr>
                            ),
                            distributionsAndForfeitures.forfeitureClassActionTotal && (
                              <tr key="classaction">
                                <td className="whitespace-nowrap px-2 py-2 text-left">Class Action (C)</td>
                                <td className="whitespace-nowrap px-2 py-2 text-right">
                                  {numberToCurrency(distributionsAndForfeitures.forfeitureClassActionTotal)}
                                </td>
                              </tr>
                            )
                          ].filter(Boolean)}
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              )}
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
