import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetMilitaryAndRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
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

  const { militaryAndRehireForfeitures, militaryAndRehireForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireForfeituresQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: militaryAndRehireForfeituresQueryParams?.profitYear ?? 0,
      reportingYear: militaryAndRehireForfeituresQueryParams?.reportingYear ?? "",
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [
    pageNumber,
    pageSize,
    triggerSearch,
    militaryAndRehireForfeituresQueryParams?.profitYear,
    militaryAndRehireForfeituresQueryParams?.reportingYear
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
      {militaryAndRehireForfeitures?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${CAPTIONS.REHIRE_FORFEITURES} (${militaryAndRehireForfeitures?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: militaryAndRehireForfeitures?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!militaryAndRehireForfeitures && militaryAndRehireForfeitures.response.results.length > 0 && (
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
          recordCount={militaryAndRehireForfeitures.response.total}
        />
      )}
    </>
  );
};

export default RehireForfeituresGrid;
