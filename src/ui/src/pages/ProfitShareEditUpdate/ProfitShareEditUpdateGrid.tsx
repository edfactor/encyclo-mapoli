import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { ProfitShareUpdateGridColumns } from "./ProfitShareEditUpdateGridColumns";
import { ProfitShareEditUpdateGridColumns } from "./ProfitShareEditGridColumns";

const ProfitShareEditUpdateGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Name",
    isSortDescending: false
  });
  const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
  const editColumnDefs = useMemo(() => ProfitShareEditUpdateGridColumns(), []);
  const { profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

  return (
    <>
      <h1>{profitSharingUpdate?.reportName}</h1>
      {!!profitSharingUpdate && profitSharingUpdate?.reportName == "Profit Sharing Update" && (
        <DSMGrid
          preferenceKey={"ProfitShareUpdateGrid"}
          isLoading={false}
          providedOptions={{
            rowData: "response" in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
            columnDefs: columnDefs
          }}
        />
      )}
      {!!profitSharingUpdate && profitSharingUpdate?.reportName == "Profit Sharing Edit" && (
        <DSMGrid
          preferenceKey={"ProfitShareEditGrid"}
          isLoading={false}
          providedOptions={{
            rowData: "response" in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
            columnDefs: editColumnDefs
          }}
        />
      )}
    </>
  );
};

export default ProfitShareEditUpdateGrid;
