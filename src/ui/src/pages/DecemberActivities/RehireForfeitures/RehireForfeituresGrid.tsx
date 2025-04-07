import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { GetMilitaryAndRehireForfeituresColumns, GetDetailColumns } from "./RehireForfeituresGridColumns";

interface MilitaryAndRehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const RehireForfeituresGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
                                                                                        initialSearchLoaded,
                                                                                        setInitialSearchLoaded
                                                                                      }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: rehireForfeituresQueryParams?.profitYear ?? 0,
      beginningDate: rehireForfeituresQueryParams?.beginningDate ?? "",
      endingDate: rehireForfeituresQueryParams?.endingDate ?? "",
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };

    await triggerSearch(request, false);
  }, [
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch,
    rehireForfeituresQueryParams?.profitYear,
    rehireForfeituresQueryParams?.beginningDate,
    rehireForfeituresQueryParams?.endingDate
  ]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  // Create a flattened view of the data
  const flattenedData = useMemo(() => {
    if (!rehireForfeitures?.response?.results) return [];

    const flattenedRows = [];

    for (const row of rehireForfeitures.response.results) {
      // Add main row
      flattenedRows.push({
        ...row,
        isParent: true,
        rowType: 'parent'
      });

      // Add detail rows if they exist
      if (row.details && row.details.length > 0) {
        for (const detail of row.details) {
          flattenedRows.push({
            badgeNumber: row.badgeNumber,
            fullName: row.fullName,
            ...detail,
            isParent: false,
            rowType: 'child',
            parentId: row.badgeNumber
          });
        }
      }
    }

    return flattenedRows;
  }, [rehireForfeitures]);

  // Combine main and detail columns
  const columnDefs = useMemo(() => {
    const mainColumns = GetMilitaryAndRehireForfeituresColumns();
    const detailColumns = GetDetailColumns();

    // Add a rowType column for styling/display logic if needed
    const typeColumn = {
      headerName: "Type",
      field: "rowType",
      hide: true
    };

    return [typeColumn, ...mainColumns, ...detailColumns];
  }, []);

  return (
    <div>
      <Typography
        variant="h2"
        sx={{ color: "#0258A5" }}>
        {`Rehire Forfeitures (QPREV-PROF) (${rehireForfeitures?.response.total || 0} ${rehireForfeitures?.response.total === 1 ? 'Record' : 'Records'})`}
      </Typography>

      {rehireForfeitures?.response && (
        <>
          <DSMGrid
            preferenceKey={"REHIRE_FORFEITURES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: flattenedData,
              columnDefs: columnDefs
            }}
          />

          {rehireForfeitures.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber + 1}
              setPageNumber={(value: number) => {
                setPageNumber(value - 1);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                setPageSize(value);
                setPageNumber(0);
                setInitialSearchLoaded(true);
              }}
              recordCount={rehireForfeitures.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default RehireForfeituresGrid;