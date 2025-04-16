import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency, Pagination } from "smart-ui-library";
import { GetDistributionsAndForfeituresColumns } from "./DistributionAndForfeituresGridColumns";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { CAPTIONS } from "../../../constants";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";

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
    sortBy: "employeeName",
    isSortDescending: false
  });

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { distributionsAndForfeitures, distributionsAndForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsAndForfeituresQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYear || 0,
      ...(distributionsAndForfeituresQueryParams?.startMonth && {
        startMonth: distributionsAndForfeituresQueryParams?.startMonth
      }),
      ...(distributionsAndForfeituresQueryParams?.endMonth && {
        endMonth: distributionsAndForfeituresQueryParams?.endMonth
      }),
      includeOutgoingForfeitures: distributionsAndForfeituresQueryParams?.includeOutgoingForfeitures ?? false,
      pagination: { skip: pageNumber * pageSize, take: pageSize, sortBy: sortParams.sortBy, isSortDescending: sortParams.isSortDescending }
    };

    await triggerSearch(request, false);
  }, [
    distributionsAndForfeituresQueryParams?.endMonth,
    distributionsAndForfeituresQueryParams?.includeOutgoingForfeitures,
    distributionsAndForfeituresQueryParams?.startMonth,
    profitYear,
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch
  ]);

  useEffect(() => {
    if (hasToken && (initialSearchLoaded || sortParams)) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken, setInitialSearchLoaded]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDistributionsAndForfeituresColumns(), []);

  return (
    <>
      {distributionsAndForfeitures?.response && (
        <>

          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.distributionTotal || 0)]]}
              leftColumnHeaders={["Distributions"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.stateTaxTotal || 0)]]}
              leftColumnHeaders={["StateTaxs"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.federalTaxTotal || 0)]]}
              leftColumnHeaders={["FederalTaxs"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(distributionsAndForfeitures.forfeitureTotal || 0)]]}
              leftColumnHeaders={["Forfeitures"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>
          
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`DISTRIBUTIONS AND FORFEITURES (${distributionsAndForfeitures?.response.total || 0} ${distributionsAndForfeitures?.response.total === 1 ? 'Record' : 'Records'})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}
            isLoading={false}
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
