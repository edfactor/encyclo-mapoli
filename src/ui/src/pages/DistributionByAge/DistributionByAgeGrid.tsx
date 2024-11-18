import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import {  useLazyGetDistributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetDistributionsByAgeColumns } from "./DistributionByAgeGridColumns";

const DistributionByAgeGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { distributionsByAge } = useSelector((state: RootState) => state.yearsEnd);
  const [dummy1, { isLoading }] = useLazyGetDistributionsByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDistributionsByAgeColumns(), []);

  return (
    <>
      {distributionsByAge?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${distributionsByAge.reportName} (${distributionsByAge?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={isLoading}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: distributionsByAge?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
    </>
  );
};

export default DistributionByAgeGrid;
