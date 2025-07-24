import { useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetVestingAmountByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { ISortParams } from "smart-ui-library";
import DSMGrid from "components/DSMGrid/DSMGrid";
import ReportSummary from "../../../components/ReportSummary";
import { VestedAmountsByAge } from "../../../reduxstore/types";
import { GetVestedAmountsByAgeColumns } from "./VestedAmountsByAgeGridColumns";

interface VestedAmountsByAgeGridProps {
  gridTitle: string;
  countColName: string;
  amountColName: string;
  totalCount?: keyof VestedAmountsByAge | undefined;
}

const VestedAmountsByAgeGrid: React.FC<VestedAmountsByAgeGridProps> = ({
  gridTitle,
  amountColName,
  countColName,
  totalCount
}) => {
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);
  const [_triggerSearch, { isFetching }] = useLazyGetVestingAmountByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefs = useMemo(
    () => GetVestedAmountsByAgeColumns(countColName, amountColName),
    [countColName, amountColName]
  );

  return (
    <>
      {vestedAmountsByAge?.response && (
        <>
          <ReportSummary report={vestedAmountsByAge} />
          <DSMGrid
            preferenceKey={gridTitle}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: vestedAmountsByAge?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
    </>
  );
};

export default VestedAmountsByAgeGrid;
