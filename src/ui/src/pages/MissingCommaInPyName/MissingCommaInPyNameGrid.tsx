import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetNamesMissingCommasQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMissingCommaInPyNameColumns } from "./MissingCommaInPyNameGridColumns";

const MissingCommaInPyNameGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { missingCommaInPYName } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNamesMissingCommasQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetMissingCommaInPyNameColumns(), []);

  return (
    <>
      {missingCommaInPYName?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`MISSING COMMA IN PY_NAME (${missingCommaInPYName?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: missingCommaInPYName?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!missingCommaInPYName && missingCommaInPYName.response.results.length > 0 && (
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
          recordCount={missingCommaInPYName.response.total}
        />
      )}
    </>
  );
};

export default MissingCommaInPyNameGrid;
