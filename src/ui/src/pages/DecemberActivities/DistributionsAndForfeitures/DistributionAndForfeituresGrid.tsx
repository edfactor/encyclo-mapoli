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
import "./DistributionAndForfeituresGrid.css";
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
          <div className="totals-flex-container sticky top-0 z-10 flex bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.distributionTotal || 0)]]}
              leftColumnHeaders={["Distributions"]}
              topRowHeaders={[]}></TotalsGrid>
            <div className="totals-flex-popover">
              <TotalsGrid
                displayData={[[numberToCurrency(distributionsAndForfeitures.stateTaxTotal || 0)]]}
                leftColumnHeaders={["State Taxes"]}
                topRowHeaders={[]}></TotalsGrid>
              {distributionsAndForfeitures.stateTaxTotals &&
                Object.keys(distributionsAndForfeitures.stateTaxTotals).length > 0 && (
                  <>
                    <div
                      className="state-tax-info-container"
                      onMouseEnter={handlePopoverOpen}
                      onMouseLeave={handlePopoverClose}>
                      <InfoOutlinedIcon
                        className="state-tax-info-icon"
                        fontSize="small"
                        style={{ cursor: "pointer", marginLeft: 4 }}
                      />
                      {open && (
                        <div className="state-tax-tooltip">
                          <div className="state-tax-popover-table">
                            <Typography
                              variant="subtitle2"
                              sx={{ p: 1 }}>
                              State Tax Breakdown
                            </Typography>
                            <table>
                              <thead>
                                <tr>
                                  <th>State</th>
                                  <th>Tax Total</th>
                                </tr>
                              </thead>
                              <tbody>
                                {Object.entries(distributionsAndForfeitures.stateTaxTotals).map(([state, total]) => (
                                  <tr key={state}>
                                    <td>{state}</td>
                                    <td>{numberToCurrency(total as number)}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </div>
                      )}
                    </div>
                  </>
                )}
            </div>
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.federalTaxTotal || 0)]]}
              leftColumnHeaders={["Federal Taxes"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.forfeitureTotal || 0)]]}
              leftColumnHeaders={["Forfeitures"]}
              topRowHeaders={[]}></TotalsGrid>
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
