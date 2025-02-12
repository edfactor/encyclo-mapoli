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
      reportName: data?.reportName || "Summary",
      totalFullTime100PercentAmount: data?.totalFullTime100PercentAmount || 0,
      totalFullTimePartialAmount: data?.totalFullTimePartialAmount || 0,
      totalFullTimeNotVestedAmount: data?.totalFullTimeNotVestedAmount || 0,
      totalPartTime100PercentAmount: data?.totalPartTime100PercentAmount || 0,
      totalPartTimePartialAmount: data?.totalPartTimePartialAmount || 0,
      totalPartTimeNotVestedAmount: data?.totalPartTimeNotVestedAmount || 0,
      totalBeneficiaryCount: data?.totalBeneficiaryCount || 0,
      totalBeneficiaryAmount: data?.totalBeneficiaryAmount || 0,
      totalFullTimeCount: data?.totalFullTimeCount || 0,
      totalNotVestedCount: data?.totalNotVestedCount || 0,
      totalPartialVestedCount: data?.totalPartialVestedCount || 0,
      reportDate: data?.reportDate || "N/A",
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
            ...columns,
            {
              headerName: "Summary",
              children: [
                { headerName: "Report Name", field: "reportName" },
                { headerName: "Report Date", field: "reportDate" },
                { headerName: "FT 100% Vested Amount", field: "totalFullTime100PercentAmount" },
                { headerName: "FT Partial Vested Amount", field: "totalFullTimePartialAmount" },
                { headerName: "FT Not Vested Amount", field: "totalFullTimeNotVestedAmount" },
                { headerName: "PT 100% Vested Amount", field: "totalPartTime100PercentAmount" },
                { headerName: "PT Partial Vested Amount", field: "totalPartTimePartialAmount" },
                { headerName: "PT Not Vested Amount", field: "totalPartTimeNotVestedAmount" },
                { headerName: "Beneficiary Count", field: "totalBeneficiaryCount" },
                { headerName: "Beneficiary Amount", field: "totalBeneficiaryAmount" },
                { headerName: "FT Total Count", field: "totalFullTimeCount" },
                { headerName: "Not Vested Count", field: "totalNotVestedCount" },
                { headerName: "Partial Vested Count", field: "totalPartialVestedCount" },
              ],
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
