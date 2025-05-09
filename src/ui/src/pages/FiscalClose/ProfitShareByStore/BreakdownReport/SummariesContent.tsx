
import { Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMGrid } from "smart-ui-library";
import { useEffect } from "react";
import { useLazyGetBreakdownGrandTotalsQuery } from "reduxstore/api/YearsEndApi";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useSelector } from "react-redux";
import { RootState } from "../../../../reduxstore/store";

const SummariesContent: React.FC = () => {
  const { breakdownGrandTotals } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  // Use the API hook to fetch data
  const [getBreakdownGrandTotals] = useLazyGetBreakdownGrandTotalsQuery();

  useEffect(() => {
    if (hasToken) {
      // Fetch grand totals when profit year changes
      getBreakdownGrandTotals({
        profitYear: profitYear
      });
    }
  }, [profitYear, getBreakdownGrandTotals, hasToken]);

  // Transform API data to match the grid format
  const transformApiDataToGridFormat = () => {
    if (!breakdownGrandTotals || !breakdownGrandTotals.Rows) {
      return {
        rowData: [
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
        ],
        grandTotal: {
          category: "Grand Total",
          ste1: "XX",
          "700": "XX",
          "701": "XX",
          "800": "XX",
          "801": "XX",
          "802": "XX",
          "900": "XX",
          total: "XX"
        }
      };
    }

    // Find rows by category
    const fullyVestedRow = breakdownGrandTotals.Rows.find(row => row.Category === "100% Vested");
    const partiallyVestedRow = breakdownGrandTotals.Rows.find(row => row.Category === "Partially Vested");
    const notVestedRow = breakdownGrandTotals.Rows.find(row => row.Category === "Not Vested");
    const grandTotalRow = breakdownGrandTotals.Rows.find(row => row.Category === "Grand Total");

    const rowData = [
      {
        category: "100% Vested",
        ste1: fullyVestedRow ? fullyVestedRow.StoreOther.toString() : "XX",
        "700": fullyVestedRow ? fullyVestedRow.Store700.toString() : "XX",
        "701": fullyVestedRow ? fullyVestedRow.Store701.toString() : "XX",
        "800": fullyVestedRow ? fullyVestedRow.Store800.toString() : "XX",
        "801": fullyVestedRow ? fullyVestedRow.Store801.toString() : "XX",
        "802": fullyVestedRow ? fullyVestedRow.Store802.toString() : "XX",
        "900": fullyVestedRow ? fullyVestedRow.Store900.toString() : "XX",
        total: fullyVestedRow ? fullyVestedRow.RowTotal.toString() : "XX"
      },
      {
        category: "Partially Vested",
        ste1: partiallyVestedRow ? partiallyVestedRow.StoreOther.toString() : "XX",
        "700": partiallyVestedRow ? partiallyVestedRow.Store700.toString() : "XX",
        "701": partiallyVestedRow ? partiallyVestedRow.Store701.toString() : "XX",
        "800": partiallyVestedRow ? partiallyVestedRow.Store800.toString() : "XX",
        "801": partiallyVestedRow ? partiallyVestedRow.Store801.toString() : "XX",
        "802": partiallyVestedRow ? partiallyVestedRow.Store802.toString() : "XX",
        "900": partiallyVestedRow ? partiallyVestedRow.Store900.toString() : "XX",
        total: partiallyVestedRow ? partiallyVestedRow.RowTotal.toString() : "XX"
      },
      {
        category: "Not Vested",
        ste1: notVestedRow ? notVestedRow.StoreOther.toString() : "XX",
        "700": notVestedRow ? notVestedRow.Store700.toString() : "XX",
        "701": notVestedRow ? notVestedRow.Store701.toString() : "XX",
        "800": notVestedRow ? notVestedRow.Store800.toString() : "XX",
        "801": notVestedRow ? notVestedRow.Store801.toString() : "XX",
        "802": notVestedRow ? notVestedRow.Store802.toString() : "XX",
        "900": notVestedRow ? notVestedRow.Store900.toString() : "XX",
        total: notVestedRow ? notVestedRow.RowTotal.toString() : "XX"
      }
    ];

    const grandTotal = {
      category: "Grand Total",
      ste1: grandTotalRow ? grandTotalRow.StoreOther.toString() : "XX",
      "700": grandTotalRow ? grandTotalRow.Store700.toString() : "XX",
      "701": grandTotalRow ? grandTotalRow.Store701.toString() : "XX",
      "800": grandTotalRow ? grandTotalRow.Store800.toString() : "XX",
      "801": grandTotalRow ? grandTotalRow.Store801.toString() : "XX",
      "802": grandTotalRow ? grandTotalRow.Store802.toString() : "XX",
      "900": grandTotalRow ? grandTotalRow.Store900.toString() : "XX",
      total: grandTotalRow ? grandTotalRow.RowTotal.toString() : "XX"
    };

    return { rowData, grandTotal };
  };

  const { rowData, grandTotal } = transformApiDataToGridFormat();

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
          preferenceKey={`BREAKDOWN_REPORT_ALL_EMPLOYEES_SUMMARY_STORE`}
          isLoading={false}
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: rowData,
            columnDefs: allEmployeesColumnDefs,
            domLayout: "autoHeight",
            pinnedTopRowData: [grandTotal],
            ...gridOptions
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default SummariesContent;