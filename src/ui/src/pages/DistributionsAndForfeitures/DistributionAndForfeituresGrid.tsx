import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetDistributionsAndForfeituresColumns } from "./DistributionAndForfeituresGridColumns";

const DistributionsAndForfeituresGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { distributionsAndForfeitures } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetDistributionsAndForfeituresQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDistributionsAndForfeituresColumns(), []);

  return (
    <>
      {distributionsAndForfeitures?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`DISTRIBUTIONS AND FORFEITURES (${distributionsAndForfeitures?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: distributionsAndForfeitures?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!distributionsAndForfeitures && distributionsAndForfeitures.response.results.length > 0 && (
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
          recordCount={distributionsAndForfeitures.response.total}
        />
      )}
    </>
  );
};

export default DistributionsAndForfeituresGrid;
