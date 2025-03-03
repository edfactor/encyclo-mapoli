import { Typography } from "@mui/material";
import { useState, useMemo, useCallback } from "react";
import { useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "./NegativeEtvaForSSNsOnPayprofitGridColumn";
import { NegativeEtvaForSSNsOnPayProfit } from "reduxstore/types";
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

  const dummyETVAData: NegativeEtvaForSSNsOnPayProfit[] = [
    {
      employeeBadge: 700127,
      ssn: 123456789,
      etvaValue: -1234.56
    },
    {
      employeeBadge: 234567,
      ssn: 234567890,
      etvaValue: -42.1
    },
    {
      employeeBadge: 345678,
      ssn: 345678901,
      etvaValue: -999.99
    },
    {
      employeeBadge: 456789,
      ssn: 456789012,
      etvaValue: -5000.0
    },
    {
      employeeBadge: 567890,
      ssn: 567890123,
      etvaValue: -1.5
    },
    {
      employeeBadge: 678901,
      ssn: 678901234,
      etvaValue: -750.25
    },
    {
      employeeBadge: 789012,
      ssn: 789012345,
      etvaValue: -3333.33
    },
    {
      employeeBadge: 890123,
      ssn: 890123456,
      etvaValue: -15.75
    },
    {
      employeeBadge: 901234,
      ssn: 901234567,
      etvaValue: -2500.0
    },
    {
      employeeBadge: 12345,
      ssn: 12345678,
      etvaValue: -175.8
    }
  ];

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
              rowData: dummyETVAData,
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
