import { GetMilitaryAndRehireForfeituresColumns } from "pages/MilitaryAndRehireForfeitures/MilitaryAndRehireForfeituresGridColumns";
import { useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";

const MilitaryContributionsGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { militaryContributionsData } = useSelector(
    (state: RootState) => state.military
  );

  const results = militaryContributionsData?.response?.results ?? [];
  const total = militaryContributionsData?.response?.total ?? 0;

  return (
    <DSMGrid
      providedOptions={{
        rowData: results,
        columnDefs: GetMilitaryAndRehireForfeituresColumns()
      }}
      isLoading={false}
      preferenceKey={""}
    />
  );
};

export default MilitaryContributionsGrid;
