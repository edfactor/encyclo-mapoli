import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router";
import { useLazyGetProfitSharingLabelsQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";

import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination, ISortParams } from "smart-ui-library";
import { GetProfallGridColumns } from "./ProfallGridColumns";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

const ProfallGrid = () => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const profitSharingLabels = useSelector((state: RootState) => state.yearsEnd.profitSharingLabels);
  const [getProfitSharingLabels, { isFetching }] = useLazyGetProfitSharingLabelsQuery();

  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (profitYear) {
      fetchData();
    }
  }, [profitYear, pageNumber, pageSize, sortParams]);

  const fetchData = useCallback(() => {
    const yearToUse = profitYear || new Date().getFullYear();
    const skip = pageNumber * pageSize;
    getProfitSharingLabels({
      profitYear: yearToUse,
      pagination: {
        take: pageSize,
        skip: skip,
        sortBy: sortParams.sortBy || "badgeNumber",
        isSortDescending: sortParams.isSortDescending
      }
    })
  }, [profitYear, pageNumber, pageSize, sortParams, getProfitSharingLabels]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  // Need a useEffect to reset the page number when data changes
  useEffect(() => {
    if (profitSharingLabels?.results) {
      setPageNumber(0);
    }
  }, [profitSharingLabels]);

  const columnDefs = useMemo(() => GetProfallGridColumns(handleNavigationForButton), [handleNavigationForButton]);
  
  const rowData = useMemo(() => {
    return profitSharingLabels?.results || [];
  }, [profitSharingLabels]);

  const recordCount = profitSharingLabels?.total || 0;

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`PROFALL REPORT (${recordCount} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"PROFALL_REPORT"}
        isLoading={isFetching}
        handleSortChanged={sortEventHandler}
        providedOptions={{
          rowData: rowData,
          columnDefs: columnDefs
        }}
      />
      {recordCount > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(0);
          }}
          recordCount={recordCount}
        />
      )}
    </>
  );
};

export default ProfallGrid;