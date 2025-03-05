import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetMilitaryAndRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMilitaryAndRehireForfeituresColumns } from "./MilitaryAndRehireForfeituresGridColumns";

interface MilitaryAndRehireForfeituresGridSearchProps {
  profitYearCurrent: number | null;
  reportingYearCurrent: string | null;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const MilitaryAndRehireForfeituresGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
  profitYearCurrent,
  reportingYearCurrent,
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { militaryAndRehireForfeitures } = useSelector((state: RootState) => state.yearsEnd);

  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireForfeituresQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYearCurrent ?? 0,
      reportingYear: reportingYearCurrent ?? "",
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [profitYearCurrent, reportingYearCurrent, pageNumber, pageSize, triggerSearch]);

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
              {`Military and Rehire Forfeitures (${militaryAndRehireForfeitures?.response.total || 0})`}
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

export default MilitaryAndRehireForfeituresGrid;
