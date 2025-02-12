import { Typography, Button } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import { useNavigate } from "react-router";
import { useMemo } from "react";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";

const sampleData = [
    {
        badgeNumber: 47425,
        employeeName: "BACHELDER, JAKE R",
        ssn: "***-**-7425",
        forfeitures: 5000.00,
        contForfeitPoints: 565,
        earningsPoints: 317,
        benNumber: "12345"
    },
    {
        badgeNumber: 82424,
        employeeName: "BATISTA, STEVEN",
        ssn: "***-**-2424",
        forfeitures: 2500.00,
        contForfeitPoints: 23,
        earningsPoints: 23,
        benNumber: "12346"
    }
];

const totalsRow = {
    forfeitures: '0.00',
    contForfeitPoints: 0,
    earningsPoints: 0
};

const ForfeitGrid = () => {
    const navigate = useNavigate();

    const viewBadge = (params: any) => {
        if (params.value) {
            return (
                <Button
                    variant="text"
                    onClick={() => navigate(`/master-inquiry/${params.value}`)}
                >
                    {params.value}
                </Button>
            );
        }
        return null;
    };

    const columnDefs = useMemo(() => GetProfitShareForfeitColumns(viewBadge), []);

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`PROFIT SHARE FORFEIT (PAY443) (${sampleData.length || 0})`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"PROFIT_SHARE_FORFEIT"}
                isLoading={false}
                handleSortChanged={(params) => { }}
                providedOptions={{
                    rowData: sampleData,
                    pinnedBottomRowData: [totalsRow],
                    columnDefs: columnDefs
                }}
            />
        </>
    );
};

export default ForfeitGrid;