import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetVestingAmountByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetVestedAmountsByAgeColumns } from "./VestedAmountsByAgeGridColumns";
import { VestedAmountsByAge } from "../../../reduxstore/types";

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
    sortBy: "Badge",
    isSortDescending: false
  });

  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);
  const [_trigger, { isLoading }] = useLazyGetVestingAmountByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefs = GetVestedAmountsByAgeColumns(countColName, amountColName);

  return (
    <>
      {vestedAmountsByAge?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${gridTitle} ${totalCount != undefined ? `(${vestedAmountsByAge[totalCount]})` : ""}`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={gridTitle}
            isLoading={isLoading}
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
