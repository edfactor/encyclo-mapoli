import { Typography } from "@mui/material";
import { useMemo, useState, useEffect, useCallback } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetDuplicateSSNsOnDemographicsColumns } from "./DuplicateSSNsOnDemographicsGridColumns";
import { CAPTIONS } from "../../../constants";

const DuplicateSSNsOnDemographicsGrid: React.FC = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { duplicateSSNsData } = useSelector((state: RootState) => state.yearsEnd);
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetDuplicateSSNsQuery();

  const onSearch = useCallback(async () => {
    const request = {
      pagination: { skip: pageNumber * pageSize, take: pageSize, sortBy: sortParams.sortBy, isSortDescending: sortParams.isSortDescending },
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearch]);

  useEffect(() => {
    if ( hasToken) {
      onSearch();
    }
  }, [pageNumber, pageSize, sortParams, onSearch]);

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
              {`(${duplicateSSNsData?.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.DUPLICATE_SSNS}
            isLoading={isFetching}
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
