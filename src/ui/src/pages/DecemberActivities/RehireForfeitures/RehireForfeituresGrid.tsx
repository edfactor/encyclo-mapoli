import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { GetMilitaryAndRehireForfeituresColumns } from "./RehireForfeituresGridColumns";

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
    sortBy: "Badge",
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
  const columnDefs = useMemo(() => GetMilitaryAndRehireForfeituresColumns(), []);

  return (
    <>
      {rehireForfeitures?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${CAPTIONS.REHIRE_FORFEITURES} (${rehireForfeitures?.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: rehireForfeitures?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!rehireForfeitures && rehireForfeitures.response.results.length > 0 && (
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
          recordCount={rehireForfeitures.response.total}
        />
      )}
    </>
  );
};

export default RehireForfeituresGrid;
