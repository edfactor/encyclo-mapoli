import { Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import { useMemo } from "react";
import Grid2 from '@mui/material/Grid2';

interface AssociatesGridProps {
  store: string; // everything hardcoded for now, will eventually pull from store based on store number
}

const sampleData = [
  {
    badge: 47425,
    employeeName: "BACHELDER, JAKE R",
    beginningBalance: "$X,XXX.XX",
    earnings: "$X,XXX.XX",
    cont: "$X,XXX.XX",
    forf: "$X,XXX.XX",
    dist: "$X,XXX.XX",
    endingBalance: "$X,XXX.XX",
    endingBalance2: "$X,XXX.XX",
    vestedAmount: "$X,XXX.XX"
  },
  {
    badge: 82424,
    employeeName: "BATISTA, STEVEN",
    beginningBalance: "$X,XXX.XX",
    earnings: "$X,XXX.XX",
    cont: "$X,XXX.XX",
    forf: "$X,XXX.XX",
    dist: "$X,XXX.XX",
    endingBalance: "$X,XXX.XX",
    endingBalance2: "$X,XXX.XX",
    vestedAmount: "$X,XXX.XX"
  },
  {
    badge: 85744,
    employeeName: "BRADLEY, ZACHARY",
    beginningBalance: "$X,XXX.XX",
    earnings: "$X,XXX.XX",
    cont: "$X,XXX.XX",
    forf: "$X,XXX.XX",
    dist: "$X,XXX.XX",
    endingBalance: "$X,XXX.XX",
    endingBalance2: "$X,XXX.XX",
    vestedAmount: "$X,XXX.XX"
  },
  {
    badge: 94861,
    employeeName: "COCHRAN, KYLE E",
    beginningBalance: "$X,XXX.XX",
    earnings: "$X,XXX.XX",
    cont: "$X,XXX.XX",
    forf: "$X,XXX.XX",
    dist: "$X,XXX.XX",
    endingBalance: "$X,XXX.XX",
    endingBalance2: "$X,XXX.XX",
    vestedAmount: "$X,XXX.XX"
  }
];

const AssociatesGrid: React.FC<AssociatesGridProps> = ({ store }) => {
  const columnDefs = useMemo(() => [
    {
      headerName: "Badge",
      field: "badge",
      width: 100
    },
    {
      headerName: "Employee Name",
      field: "employeeName",
      width: 200
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      width: 150
    },
    {
      headerName: "Earnings",
      field: "earnings",
      width: 120
    },
    {
      headerName: "Cont",
      field: "cont",
      width: 120
    },
    {
      headerName: "Forf",
      field: "forf",
      width: 120
    },
    {
      headerName: "Dist",
      field: "dist",
      width: 120
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      width: 150
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance2",
      width: 150
    },
    {
      headerName: "Vested Amount",
      field: "vestedAmount",
      width: 150
    }
  ], []);

  return (
    <Grid2 container direction="column" width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Associates
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey={`BREAKDOWN_REPORT_ASSOCIATES_STORE_${store}`}
          isLoading={false}
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: sampleData,
            columnDefs: columnDefs
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default AssociatesGrid; 