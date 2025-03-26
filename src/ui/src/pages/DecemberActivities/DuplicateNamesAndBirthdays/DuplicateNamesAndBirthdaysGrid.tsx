import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDuplicateNamesAndBirthdaysQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetDuplicateNamesAndBirthdayColumns } from "./DuplicateNamesAndBirthdaysGridColumns";

interface DuplicateNamesAndBirthdaysGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const DuplicateNamesAndBirthdaysGrid: React.FC<DuplicateNamesAndBirthdaysGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { duplicateNamesAndBirthdays, duplicateNamesAndBirthdaysQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [triggerSearch, { isLoading }] = useLazyGetDuplicateNamesAndBirthdaysQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: duplicateNamesAndBirthdaysQueryParams?.profitYear ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [duplicateNamesAndBirthdaysQueryParams?.profitYear, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDuplicateNamesAndBirthdayColumns(), []);

  return (
    <>
      {duplicateNamesAndBirthdays?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`DUPLICATE NAMES AND BIRTHDAYS (${duplicateNamesAndBirthdays?.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: duplicateNamesAndBirthdays?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!duplicateNamesAndBirthdays && duplicateNamesAndBirthdays.response.results.length > 0 && (
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
          recordCount={duplicateNamesAndBirthdays.response.total}
        />
      )}
    </>
  );
};

export default DuplicateNamesAndBirthdaysGrid;
