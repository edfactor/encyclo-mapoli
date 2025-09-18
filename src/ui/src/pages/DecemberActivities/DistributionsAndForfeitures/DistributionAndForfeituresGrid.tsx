import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Typography } from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";
import { CAPTIONS } from "../../../constants";
import { GetDistributionsAndForfeituresColumns } from "./DistributionAndForfeituresGridColumns";

interface DistributionsAndForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const DistributionsAndForfeituresGrid: React.FC<DistributionsAndForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "employeeName, date",
    isSortDescending: false
  });
  const [showTooltip, setShowTooltip] = useState(false);
  const [hoverTimeout, setHoverTimeout] = useState<NodeJS.Timeout | null>(null);

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

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { distributionsAndForfeitures, distributionsAndForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsAndForfeituresQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYear || 0,
      ...(distributionsAndForfeituresQueryParams?.startDate && {
        startDate: distributionsAndForfeituresQueryParams?.startDate
      }),
      ...(distributionsAndForfeituresQueryParams?.endDate && {
        endDate: distributionsAndForfeituresQueryParams?.endDate
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
    profitYear,
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch
  ]);

  // Need a useEffect on a change in distributionsAndForfeitures to reset the page number
  const prevDistributionsAndForfeitures = useRef<any>(null);
  useEffect(() => {
    if (
      distributionsAndForfeitures !== prevDistributionsAndForfeitures.current &&
      distributionsAndForfeitures?.response?.results &&
      distributionsAndForfeitures.response.results.length !==
        prevDistributionsAndForfeitures.current?.response?.results?.length
    ) {
      setPageNumber(0);
    }
    prevDistributionsAndForfeitures.current = distributionsAndForfeitures;
  }, [distributionsAndForfeitures]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken, setInitialSearchLoaded]);

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (hoverTimeout) {
        clearTimeout(hoverTimeout);
      }
    };
  }, [hoverTimeout]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDistributionsAndForfeituresColumns(), []);
  const stateTaxTotals = distributionsAndForfeitures?.stateTaxTotals || {};

  return (
    <>
      {distributionsAndForfeitures?.response && (
        <>
          <div className="flex items-start gap-4 py-2 sticky top-0 z-10 bg-white">
            <div className="flex-1 min-w-0">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.distributionTotal || 0)]]}
                leftColumnHeaders={["Distributions"]}
                topRowHeaders={[]}></TotalsGrid>
            </div>
            <div className="flex items-center relative flex-[1.1] min-w-[140px]">
              <div className="flex-1 min-w-0 pr-2">
                <TotalsGrid
                  displayData={[[numberToCurrency(distributionsAndForfeitures.stateTaxTotal || 0)]]}
                  leftColumnHeaders={["State Taxes"]}
                  topRowHeaders={[]}></TotalsGrid>
              </div>
              {distributionsAndForfeitures.stateTaxTotals &&
                Object.keys(distributionsAndForfeitures.stateTaxTotals).length > 0 && (
                  <div
                    className="relative inline-block"
                    onMouseEnter={handlePopoverOpen}
                    onMouseLeave={handlePopoverClose}>
                    <InfoOutlinedIcon
                      className="cursor-pointer ml-1 text-green-500"
                      fontSize="small"
                    />
                    {open && (
                      <div className="absolute top-full left-0 z-[1000] bg-white border border-gray-300 rounded shadow-lg max-h-[300px] max-w-[350px] overflow-auto mt-1">
                        <div className="p-2 px-4 pb-4">
                          <Typography
                            variant="subtitle2"
                            sx={{ p: 1 }}>
                            State Tax Breakdown
                          </Typography>
                          <table className="w-full border-collapse text-[0.95rem]">
                            <thead>
                              <tr>
                                <th className="px-2 py-1 text-left border-b border-gray-300 font-semibold">State</th>
                                <th className="px-2 py-1 text-right border-b border-gray-300 font-semibold">Tax Total</th>
                              </tr>
                            </thead>
                            <tbody>
                              {Object.entries(distributionsAndForfeitures.stateTaxTotals).map(([state, total], index, array) => (
                                <tr key={state}>
                                  <td className={`px-2 py-1 text-left ${index < array.length - 1 ? 'border-b border-gray-100' : ''}`}>{state}</td>
                                  <td className={`px-2 py-1 text-right ${index < array.length - 1 ? 'border-b border-gray-100' : ''}`}>{numberToCurrency(total as number)}</td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      </div>
                    )}
                  </div>
                )}
            </div>
            <div className="flex-1 min-w-0">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.federalTaxTotal || 0)]]}
                leftColumnHeaders={["Federal Taxes"]}
                topRowHeaders={[]}></TotalsGrid>
            </div>
            <div className="flex-1 min-w-0">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.forfeitureTotal || 0)]]}
                leftColumnHeaders={["Forfeitures"]}
                topRowHeaders={[]}></TotalsGrid>
            </div>
          </div>

          <ReportSummary report={distributionsAndForfeitures} />
          <DSMGrid
            preferenceKey={CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
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
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={distributionsAndForfeitures.response.total}
        />
      )}
    </>
  );
};

export default DistributionsAndForfeituresGrid;
