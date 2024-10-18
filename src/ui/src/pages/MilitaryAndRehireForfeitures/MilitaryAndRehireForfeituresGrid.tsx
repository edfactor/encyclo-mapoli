import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMilitaryAndRehireForfeituresColumns } from "./MilitaryAndRehireForfeituresGridColumns";

const MilitaryAndRehireForfeituresGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const { militaryAndRehireForfeitures } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNegativeEVTASSNQuery();

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
              {`Negative ETVA For SSNs On Payprofit (${militaryAndRehireForfeitures?.response.total || 0})`}
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
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={militaryAndRehireForfeitures.response.total}
        />
      )}
    </>
  );
};

export default MilitaryAndRehireForfeituresGrid;
