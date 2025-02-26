import { Button, Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import { useNavigate } from "react-router";
import { GetProfitShareGrossReportColumns } from "./ProfitShareGrossReportColumns";
import { useMemo } from "react";

const sampleData = [
    {
        badge: 47425,
        employeeName: "BACHELDER, JAKE R",
        ssn: "***-**-7425",
        dateOfBirth: "XX/XX/XXXX",
        psWages: 15750.25,
        psAmount: 12600.20,
        loans: 5000.00,
        forfeitures: 0.00,
        ec: 2500.75
    },
    {
        badge: 82424,
        employeeName: "BATISTA, STEVEN",
        ssn: "***-**-2424",
        dateOfBirth: "XX/XX/XXXX",
        psWages: 18200.50,
        psAmount: 14560.40,
        loans: 7500.00,
        forfeitures: 0.00,
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

    const columnDefs = useMemo(() => GetProfitShareGrossReportColumns(viewBadge), []);

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`PROFIT SHARE GROSS REPORT (QPAY501) (${sampleData.length || 0})`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"PROFIT_SHARE_GROSS_REPORT"}
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

export default ProfitShareGrossReportGrid;