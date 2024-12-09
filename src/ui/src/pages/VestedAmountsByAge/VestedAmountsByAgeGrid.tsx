import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetVestingAmountByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetVestedAmountsByAgeColumns } from "./VestedAmountsByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { VestedAmountsByAge } from "../../reduxstore/types";

const VestedAmountsByAgeGrid = () => {
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false,
  });

  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);
  const [_trigger, { isLoading }] = useLazyGetVestingAmountByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetVestedAmountsByAgeColumns();

  const createSummaryRows = (data: VestedAmountsByAge) => [
    {
      age: "TOTAL",
      employeeCount: data?.totalFullTimeCount || 0,
      currentBalance: data?.totalFullTime100PercentAmount || 0,
      vestedBalance: data?.totalFullTimePartialAmount || 0,
    },
  ];

  const renderDSMGrid = (data: VestedAmountsByAge, columns: ReturnType<typeof GetVestedAmountsByAgeColumns>, key: string) => {
    const summaryRows = createSummaryRows(data);

    return (
      <DSMGrid
        preferenceKey={key}
        isLoading={isLoading}
        handleSortChanged={sortEventHandler}
        providedOptions={{
          rowData: [...summaryRows, ...(data.response?.results || [])],
          pinnedTopRowData: summaryRows,
          columnDefs: [
            {
              headerName: "columns.headerName",
              children: columns,
            },
          ],
        }}
      />
    );
  };

  return (
    <>
      {vestedAmountsByAge?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography variant="h2" sx={{ color: "#0258A5" }}>
              {vestedAmountsByAge.reportName}
            </Typography>
          </div>
          <Grid2 container spacing={2}>
            <Grid2 xs={12}>
              {renderDSMGrid(vestedAmountsByAge, columnDefsTotal, "Vesting Amounts by Age")}
            </Grid2>
          </Grid2>
        </>
      )}
    </>
  );
};

export default VestedAmountsByAgeGrid;
