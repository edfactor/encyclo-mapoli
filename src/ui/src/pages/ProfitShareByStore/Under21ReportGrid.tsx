import { Button, Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import { useNavigate } from "react-router";
import { useMemo } from "react";
import { GetUnder21ReportColumns } from "./Under21ReportColumns";

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
    },
];

const Under21ReportGrid = () => {
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

    const columnDefs = useMemo(() => GetUnder21ReportColumns(viewBadge), []);

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`UNDER 21 AGE REPORT (${sampleData.length || 0})`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"UNDER_21_REPORT"}
                isLoading={false}
                handleSortChanged={(params) => { }}
                providedOptions={{
                    rowData: sampleData,
                    columnDefs: columnDefs
                }}
            />
        </>
    );
};

export default Under21ReportGrid;