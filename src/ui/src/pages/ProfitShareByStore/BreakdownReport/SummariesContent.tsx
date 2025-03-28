import { Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMGrid } from "smart-ui-library";

interface SummariesContentProps {
  store: string;
}

const allEmployeesSampleData = [
  {
    category: "100% Vested",
    ste1: "XX",
    "700": "XX",
    "701": "XX",
    "800": "XX",
    "801": "XX",
    "802": "XX",
    "900": "XX",
    total: "XX"
  },
  {
    category: "Partially Vested",
    ste1: "XX",
    "700": "XX",
    "701": "XX",
    "800": "XX",
    "801": "XX",
    "802": "XX",
    "900": "XX",
    total: "XX"
  },
  {
    category: "Not Vested",
    ste1: "XX",
    "700": "XX",
    "701": "XX",
    "800": "XX",
    "801": "XX",
    "802": "XX",
    "900": "XX",
    total: "XX"
  }
];

const allEmployeesGrandTotal = {
  category: "Grand Total",
  ste1: "XX",
  "700": "XX",
  "701": "XX",
  "800": "XX",
  "801": "XX",
  "802": "XX",
  "900": "XX",
  total: "XX"
};

const under21EmployeesSampleData = [
  {
    category: "100% Vested",
    ste1: "XX",
    "700": "XX",
    "701": "XX",
    "800": "XX",
    "801": "XX",
    "802": "XX",
    "900": "XX",
    total: "XX"
  },
  {
    category: "Partially Vested",
    ste1: "XX",
    "700": "XX",
    "701": "XX",
    "800": "XX",
    "801": "XX",
    "802": "XX",
    "900": "XX",
    total: "XX"
  },
  {
    category: "Not Vested",
    ste1: "XX",
    "700": "XX",
    "701": "XX",
    "800": "XX",
    "801": "XX",
    "802": "XX",
    "900": "XX",
    total: "XX"
  }
];

const under21EmployeesGrandTotal = {
  category: "Grand Total",
  ste1: "XX",
  "700": "XX",
  "701": "XX",
  "800": "XX",
  "801": "XX",
  "802": "XX",
  "900": "XX",
  total: "XX"
};

const SummariesContent: React.FC<SummariesContentProps> = ({ store }) => {
  const allEmployeesColumnDefs = [
    {
      headerName: "Category",
      field: "category",
      width: 150
    },
    {
      headerName: "STE 1-140",
      field: "ste1",
      width: 100
    },
    {
      headerName: "700",
      field: "700",
      width: 70
    },
    {
      headerName: "701",
      field: "701",
      width: 70
    },
    {
      headerName: "800",
      field: "800",
      width: 70
    },
    {
      headerName: "801",
      field: "801",
      width: 70
    },
    {
      headerName: "802",
      field: "802",
      width: 70
    },
    {
      headerName: "900",
      field: "900",
      width: 70
    },
    {
      headerName: "Total",
      field: "total",
      width: 70
    }
  ];

  const under21EmployeesColumnDefs = [
    {
      headerName: "Category",
      field: "category",
      width: 150
    },
    {
      headerName: "STE 1-140",
      field: "ste1",
      width: 100
    },
    {
      headerName: "700",
      field: "700",
      width: 70
    },
    {
      headerName: "701",
      field: "701",
      width: 70
    },
    {
      headerName: "800",
      field: "800",
      width: 70
    },
    {
      headerName: "801",
      field: "801",
      width: 70
    },
    {
      headerName: "802",
      field: "802",
      width: 70
    },
    {
      headerName: "900",
      field: "900",
      width: 70
    },
    {
      headerName: "Total",
      field: "total",
      width: 70
    }
  ];

  const gridOptions = {
    getRowStyle: (params: any) => {
      if (params.node.rowPinned) {
        return { background: "#f0f0f0", fontWeight: "bold" };
      }
      return undefined;
    }
  };

  return (
    <Grid2
      container
      direction="column"
      width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {`All Employees (By report section)`}
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey={`BREAKDOWN_REPORT_ALL_EMPLOYEES_SUMMARY_STORE_${store}`}
          isLoading={false}
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: allEmployeesSampleData,
            columnDefs: allEmployeesColumnDefs,
            domLayout: "autoHeight",
            pinnedBottomRowData: [allEmployeesGrandTotal],
            ...gridOptions
          }}
        />
      </Grid2>

      <Grid2
        style={{ marginTop: "32px" }}
        paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {`Under 21 Employees (included in above totals)`}
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey={`BREAKDOWN_REPORT_UNDER_21_EMPLOYEES_SUMMARY_STORE_${store}`}
          isLoading={false}
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: under21EmployeesSampleData,
            columnDefs: under21EmployeesColumnDefs,
            domLayout: "autoHeight",
            pinnedBottomRowData: [under21EmployeesGrandTotal],
            ...gridOptions
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default SummariesContent;
