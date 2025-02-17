import { Button, Typography } from "@mui/material";
import { useMemo } from "react";
import { useDispatch } from "react-redux";
import { DSMGrid } from "smart-ui-library";
import { useNavigate } from "react-router";
import { ICellRendererParams } from "ag-grid-community";
import { GetPay450GridColumns } from "./Pay450GridColumns";

const sampleProfitShareData = [
    {
        badge: 47425,
        employeeName: "BAGGINS, FRODO",
        store: 3,
        psAmountOriginal: 15750.25,
        psVestedOriginal: 12600.20,
        yearsOriginal: 14,
        enrollOriginal: 1,
        psAmountUpdated: 16250.75,
        psVestedUpdated: 13000.60,
        yearsUpdated: 14,
        enrollUpdated: 1
    },
    {
        badge: 82424,
        employeeName: "GAMGEE, SAMWISE",
        store: 3,
        psAmountOriginal: 18200.50,
        psVestedOriginal: 14560.40,
        yearsOriginal: 17,
        enrollOriginal: 2,
        psAmountUpdated: 19100.25,
        psVestedUpdated: 15280.20,
        yearsUpdated: 17,
        enrollUpdated: 2
    },
    {
        badge: 85744,
        employeeName: "BRANDYBUCK, MERIADOC",
        store: 3,
        psAmountOriginal: 22450.75,
        psVestedOriginal: 17960.60,
        yearsOriginal: 27,
        enrollOriginal: 2,
        psAmountUpdated: 23575.50,
        psVestedUpdated: 18860.40,
        yearsUpdated: 27,
        enrollUpdated: 2
    },
    {
        badge: 94861,
        employeeName: "TOOK, PEREGRIN",
        store: 4,
        psAmountOriginal: 25800.25,
        psVestedOriginal: 20640.20,
        yearsOriginal: 38,
        enrollOriginal: 1,
        psAmountUpdated: 27090.75,
        psVestedUpdated: 21672.60,
        yearsUpdated: 38,
        enrollUpdated: 1
    }
];

const Pay450Grid = () => {

    const navigate = useNavigate();

    const getSummaryRow = (data: any[]) => {
        const totals = data.reduce((acc, curr) => ({
          psAmountOriginal: acc.psAmountOriginal + curr.psAmountOriginal,
          psVestedOriginal: acc.psVestedOriginal + curr.psVestedOriginal,
          psAmountUpdated: acc.psAmountUpdated + curr.psAmountUpdated,
          psVestedUpdated: acc.psVestedUpdated + curr.psVestedUpdated
        }), {
          psAmountOriginal: 0,
          psVestedOriginal: 0,
          psAmountUpdated: 0,
          psVestedUpdated: 0
        });
      
        return [
          {
            psAmountOriginal: totals.psAmountOriginal,
            psVestedOriginal: totals.psVestedOriginal,
            psAmountUpdated: totals.psAmountUpdated,
            psVestedUpdated: totals.psVestedUpdated
          }
        ];
      };

    const viewBadge = (params: ICellRendererParams) => {
        return (
            params.value && (
                <Button
                    variant="text"
                    onClick={() => navigate(`/master-inquiry/${params.value}`)}
                >
                    {params.value}
                </Button>
            )
        );
    };

    const dispatch = useDispatch();
    const columnDefs = useMemo(() => GetPay450GridColumns(viewBadge), []);

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`PROFIT-ELIGIBLE REPORT (${sampleProfitShareData.length || 0})`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"ELIGIBLE_EMPLOYEES"}
                isLoading={false}
                handleSortChanged={(params) => { }}
                providedOptions={{
                    rowData: sampleProfitShareData,
                    pinnedBottomRowData: getSummaryRow(sampleProfitShareData),
                    columnDefs: columnDefs
                }}
            />
        </>

    );
};

export default Pay450Grid;