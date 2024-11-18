import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDuplicateNamesAndBirthdaysQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetDuplicateNamesAndBirthdayColumns } from "./DuplicateNamesAndBirthdaysGridColumns";

const DuplicateNamesAndBirthdaysGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { duplicateNamesAndBirthday } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetDuplicateNamesAndBirthdaysQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDuplicateNamesAndBirthdayColumns(), []);

  return (
    <>
      {duplicateNamesAndBirthday?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`DUPLICATE NAMES AND BIRTHDAYS (${duplicateNamesAndBirthday?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: duplicateNamesAndBirthday?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!duplicateNamesAndBirthday && duplicateNamesAndBirthday.response.results.length > 0 && (
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
          recordCount={duplicateNamesAndBirthday.response.total}
        />
      )}
    </>
  );
};

export default DuplicateNamesAndBirthdaysGrid;
