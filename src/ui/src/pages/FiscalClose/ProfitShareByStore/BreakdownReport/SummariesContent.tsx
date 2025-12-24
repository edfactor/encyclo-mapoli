import { CircularProgress, Grid, Typography } from "@mui/material";
import { ColDef, RowClassParams } from "ag-grid-community";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBreakdownGrandTotalsQuery } from "reduxstore/api/AdhocApi";
import { DSMGrid, numberToCurrency } from "smart-ui-library";
import { GRID_KEYS } from "../../../../constants";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { RootState } from "../../../../reduxstore/store";
import { GrandTotalsByStoreResponseDto, GrandTotalsByStoreRowDto } from "../../../../reduxstore/types";

const SummariesContent: React.FC = () => {
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  // Component-level loading state
  const [isLoading, setIsLoading] = useState(true);

  // Use API hook
  const [getBreakdownGrandTotals, { data: breakdownGrandTotals }] = useLazyGetBreakdownGrandTotalsQuery();

  // Row data state
  // Define the type for row data
  interface RowData {
    category: string;
    ste1: number;
    "700": number;
    "701": number;
    "800": number;
    "801": number;
    "802": number;
    "900": number;
    total: number;
  }

  const [rowData, setRowData] = useState<RowData[]>([]);
  const [grandTotal, setGrandTotal] = useState<RowData>({
    category: "",
    ste1: 0,
    "700": 0,
    "701": 0,
    "800": 0,
    "801": 0,
    "802": 0,
    "900": 0,
    total: 0
  });

  // Load data when component mounts
  useEffect(() => {
    if (hasToken) {
      setIsLoading(true);
      getBreakdownGrandTotals({
        profitYear: profitYear
      })
        .unwrap()
        .then((data) => {
          if (data && data.rows) {
            updateGridFromApiData(data);
          }
          setIsLoading(false);
        })
        .catch((error) => {
          console.error("Error fetching breakdown grand totals:", error);
          setIsLoading(false);
        });
    }
  }, [profitYear, getBreakdownGrandTotals, hasToken]);

  // Separate effect to handle updates to breakdownGrandTotals
  useEffect(() => {
    if (breakdownGrandTotals && breakdownGrandTotals.rows) {
      updateGridFromApiData(breakdownGrandTotals);
      setIsLoading(false);
    }
  }, [breakdownGrandTotals]);

  const updateGridFromApiData = (data: GrandTotalsByStoreResponseDto) => {
    // Find rows by category
    const fullyVestedRow = data.rows.find((row: GrandTotalsByStoreRowDto) => row.category === "100% Vested");
    const partiallyVestedRow = data.rows.find((row: GrandTotalsByStoreRowDto) => row.category === "Partially Vested");
    const notVestedRow = data.rows.find((row: GrandTotalsByStoreRowDto) => row.category === "Not Vested");
    const grandTotalRow = data.rows.find((row: GrandTotalsByStoreRowDto) => row.category === "Grand Total");

    const rows = [
      {
        category: "100% Vested",
        ste1: fullyVestedRow ? fullyVestedRow.storeOther : 0,
        "700": fullyVestedRow ? fullyVestedRow.store700 : 0,
        "701": fullyVestedRow ? fullyVestedRow.store701 : 0,
        "800": fullyVestedRow ? fullyVestedRow.store800 : 0,
        "801": fullyVestedRow ? fullyVestedRow.store801 : 0,
        "802": fullyVestedRow ? fullyVestedRow.store802 : 0,
        "900": fullyVestedRow ? fullyVestedRow.store900 : 0,
        total: fullyVestedRow ? fullyVestedRow.rowTotal : 0
      },
      {
        category: "Partially Vested",
        ste1: partiallyVestedRow ? partiallyVestedRow.storeOther : 0,
        "700": partiallyVestedRow ? partiallyVestedRow.store700 : 0,
        "701": partiallyVestedRow ? partiallyVestedRow.store701 : 0,
        "800": partiallyVestedRow ? partiallyVestedRow.store800 : 0,
        "801": partiallyVestedRow ? partiallyVestedRow.store801 : 0,
        "802": partiallyVestedRow ? partiallyVestedRow.store802 : 0,
        "900": partiallyVestedRow ? partiallyVestedRow.store900 : 0,
        total: partiallyVestedRow ? partiallyVestedRow.rowTotal : 0
      },
      {
        category: "Not Vested",
        ste1: notVestedRow ? notVestedRow.storeOther : 0,
        "700": notVestedRow ? notVestedRow.store700 : 0,
        "701": notVestedRow ? notVestedRow.store701 : 0,
        "800": notVestedRow ? notVestedRow.store800 : 0,
        "801": notVestedRow ? notVestedRow.store801 : 0,
        "802": notVestedRow ? notVestedRow.store802 : 0,
        "900": notVestedRow ? notVestedRow.store900 : 0,
        total: notVestedRow ? notVestedRow.rowTotal : 0
      }
    ];

    const total = {
      category: "Grand Total",
      ste1: grandTotalRow ? grandTotalRow.storeOther : 0,
      "700": grandTotalRow ? grandTotalRow.store700 : 0,
      "701": grandTotalRow ? grandTotalRow.store701 : 0,
      "800": grandTotalRow ? grandTotalRow.store800 : 0,
      "801": grandTotalRow ? grandTotalRow.store801 : 0,
      "802": grandTotalRow ? grandTotalRow.store802 : 0,
      "900": grandTotalRow ? grandTotalRow.store900 : 0,
      total: grandTotalRow ? grandTotalRow.rowTotal : 0
    };

    setRowData(rows);
    setGrandTotal(total);
  };

  const allEmployeesColumnDefs: ColDef[] = [
    {
      headerName: "Category",
      field: "category",
      flex: 1,
      minWidth: 150
    },
    {
      headerName: "STE 1-140",
      field: "ste1",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "700",
      field: "700",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "701",
      field: "701",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "800",
      field: "800",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "801",
      field: "801",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "802",
      field: "802",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "900",
      field: "900",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    },
    {
      headerName: "Total",
      field: "total",
      flex: 1,
      valueFormatter: (params: { value: string | number }) => numberToCurrency(params.value)
    }
  ];

  const gridOptions = {
    getRowStyle: (params: RowClassParams) => {
      if (params.node.rowPinned) {
        return { background: "#f0f0f0", fontWeight: "bold" };
      }
      return undefined;
    }
  };

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {`All Employees (By report section)`}
        </Typography>
      </Grid>
      <Grid width="100%">
        {isLoading ? (
          <Grid sx={{ display: "flex", justifyContent: "center", padding: 4 }}>
            <CircularProgress />
          </Grid>
        ) : (
          <DSMGrid
            preferenceKey={GRID_KEYS.BREAKDOWN_REPORT_SUMMARY}
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
        )}
      </Grid>
    </Grid>
  );
};

export default SummariesContent;
