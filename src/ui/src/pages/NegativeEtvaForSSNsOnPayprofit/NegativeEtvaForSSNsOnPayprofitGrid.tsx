import { Typography } from "@mui/material";
import { useState, useMemo, useCallback } from "react";
import { useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "./NegativeEtvaForSSNsOnPayprofitGridColumn";
import { Path, useNavigate } from "react-router";

const NegativeEtvaForSSNsOnPayprofitGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { negativeEtvaForSSNsOnPayprofit } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNegativeEVTASSNQuery();

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
              {`Negative ETVA For SSNs On Payprofit (${negativeEtvaForSSNsOnPayprofit?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
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
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={negativeEtvaForSSNsOnPayprofit.response.total}
        />
      )}
    </>
  );
};

export default NegativeEtvaForSSNsOnPayprofitGrid;
