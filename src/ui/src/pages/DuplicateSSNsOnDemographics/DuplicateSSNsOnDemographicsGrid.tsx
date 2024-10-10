import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetDuplicateSSNsOnDemographicsColumns } from "./DuplicateSSNsOnDemographicsGridColumns";

const DuplicateSSNsOnDemographicsGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const { duplicateSSNsData } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetDuplicateSSNsQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDuplicateSSNsOnDemographicsColumns(), []);

  return (
    <>
      {duplicateSSNsData?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`DUPLICATE SSNs ON DEMOGRAPHICS (${duplicateSSNsData?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: duplicateSSNsData?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!duplicateSSNsData && duplicateSSNsData.response.results.length > 0 && (
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
          recordCount={duplicateSSNsData.response.total}
        />
      )}
    </>
  );
};

export default DuplicateSSNsOnDemographicsGrid;
