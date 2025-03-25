import { Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitShareGrossReportColumns } from "./ProfitShareGrossReportColumns";
import { useCallback, useMemo } from "react";

const sampleData = [
  {
    badge: 47425,
    employeeName: "BACHELDER, JAKE R",
    ssn: "***-**-7425",
    dateOfBirth: "XX/XX/XXXX",
    psWages: 15750.25,
    psAmount: 12600.2,
    loans: 5000.0,
    forfeitures: 0.0,
    ec: 2500.75
  },
  {
    badge: 82424,
    employeeName: "BATISTA, STEVEN",
    ssn: "***-**-2424",
    dateOfBirth: "XX/XX/XXXX",
    psWages: 18200.5,
    psAmount: 14560.4,
    loans: 7500.0,
    forfeitures: 0.0,
    ec: 3100.25
  }
];

const totalsRow = {
  psWages: 0,
  psAmount: 0,
  loans: 0,
  forfeitures: 0
};

const ProfitShareGrossReportGrid = () => {
  const navigate = useNavigate();

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitShareGrossReportColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`PROFIT SHARE GROSS REPORT (QPAY501) (${sampleData.length || 0} ${sampleData.length === 1 ? 'Record' : 'Records'})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"PROFIT_SHARE_GROSS_REPORT"}
        isLoading={false}
        handleSortChanged={(_params) => {}}
        providedOptions={{
          rowData: sampleData,
          pinnedBottomRowData: [totalsRow],
          columnDefs: columnDefs
        }}
      />
    </>
  );
};

export default ProfitShareGrossReportGrid;
