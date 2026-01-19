import { Typography } from "@mui/material";
import { useMemo } from "react";
import { useLazyGetUnder21BreakdownByStoreQuery, useLazyGetUnder21TotalsQuery } from "reduxstore/api/YearsEndApi";
import { DSMGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { GetUnder21ReportColumns } from "./Under21ReportGridColumns";

const sampleData = [
  {
    badge: 47425,
    fullName: "BACHELDER, JAKE R",
    ssn: "***-**-7425",
    psYears: 1,
    ne: 0,
    thisYearPSHours: 3,
    lastYearPSHours: 3,
    type: "H",
    hireDate: "XX/XX/XXXX",
    fullDate: "XX/XX/XXXX",
    termDate: "XX/XX/XXXX",
    birthDate: "XX/XX/XXXX",
    age: "X"
  }
];

const Under21ReportGrid = () => {
  const columnDefs = useMemo(() => GetUnder21ReportColumns(), []);

  const [__, { isFetching: isTotalsFetching }] = useLazyGetUnder21TotalsQuery();
  const [___, { isFetching: isBreakdownFetching }] = useLazyGetUnder21BreakdownByStoreQuery();

  const isFetching = isTotalsFetching || isBreakdownFetching;

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`UNDER 21 AGE REPORT (${sampleData.length || 0} ${sampleData.length === 1 ? "Record" : "Records"})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={GRID_KEYS.UNDER_21_REPORT}
        isLoading={isFetching}
        handleSortChanged={(_params) => {}}
        providedOptions={{
          rowData: sampleData,
          columnDefs: columnDefs
        }}
      />
    </>
  );
};

export default Under21ReportGrid;
