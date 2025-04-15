import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router";
import { useLazyGetProfitSharingLabelsQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";

import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetProfallGridColumns } from "./ProfallGridColumns";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

const ProfallGrid = () => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [getSortBy, setSortBy] = useState<string | undefined>("badgeNumber");
  const [getIsSortDescending, setIsSortDescending] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const profitSharingLabels = useSelector((state: RootState) => state.yearsEnd.profitSharingLabels);
  const [getProfitSharingLabels] = useLazyGetProfitSharingLabelsQuery();

  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (profitYear) {
      fetchData();
    }
  }, [profitYear, pageNumber, pageSize, getSortBy, getIsSortDescending]);

  const fetchData = useCallback(() => {
    setIsLoading(true);
    const yearToUse = profitYear || new Date().getFullYear();
    const skip = pageNumber * pageSize;
    getProfitSharingLabels({
      profitYear: yearToUse,
      pagination: {
        take: pageSize,
        skip: skip,
        sortBy: getSortBy || "badgeNumber",
        isSortDescending: getIsSortDescending
      }
    }).then(() => {
      setIsLoading(false);
    })
      .catch(() => {
        setIsLoading(false);
      });
  }, [profitYear, pageNumber, pageSize, getSortBy, getIsSortDescending, getProfitSharingLabels]);

  const handleSortChanged = useCallback((params: any) => {
    if (params.sortModel.length > 0) {
      const sort = params.sortModel[0];
      setSortBy(sort.colId);
      setIsSortDescending(sort.sort === "desc");
      setPageNumber(0);
    } else {
      setSortBy("badgeNumber");
      setIsSortDescending(false);
      setPageNumber(0);
    }
  }, []);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

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
        isLoading={isLoading}
        handleSortChanged={handleSortChanged}
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