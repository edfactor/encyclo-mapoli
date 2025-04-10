import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "./NegativeEtvaForSSNsOnPayprofitGridColumn";
import { Path, useNavigate } from "react-router";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { CAPTIONS } from "../../../constants";

interface NegativeEtvaForSSNsOnPayprofitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const NegativeEtvaForSSNsOnPayprofitGrid: React.FC<NegativeEtvaForSSNsOnPayprofitGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { negativeEtvaForSSNsOnPayprofit } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetNegativeEVTASSNQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYear || 0,
      pagination: { 
        skip: pageNumber * pageSize, 
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, triggerSearch, profitYear, sortParams, hasToken]);

  useEffect(() => {
    if (initialSearchLoaded && hasToken && profitYear) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch, hasToken, profitYear]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const navigate = useNavigate();
  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetNegativeEtvaForSSNsOnPayProfitColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  return (
    <>
      {negativeEtvaForSSNsOnPayprofit?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`(${negativeEtvaForSSNsOnPayprofit?.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.NEGATIVE_ETVA}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: negativeEtvaForSSNsOnPayprofit.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!negativeEtvaForSSNsOnPayprofit && negativeEtvaForSSNsOnPayprofit.response.results.length > 0 && (
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
          recordCount={negativeEtvaForSSNsOnPayprofit.response.total}
        />
      )}
    </>
  );
};

export default NegativeEtvaForSSNsOnPayprofitGrid;
