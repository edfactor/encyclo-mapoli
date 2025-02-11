import { Button, Typography } from "@mui/material";
import { useMemo } from "react";
import { useDispatch } from "react-redux";
import { DSMGrid } from "smart-ui-library";
import { GetEighteenToTwentyGridColumns } from "./EighteenToTwentyGridColumns";
import { useNavigate } from "react-router";
import { ICellRendererParams } from "ag-grid-community";

interface EmployeeData {
    badge: number;
    employeeName: string;
    store: number;
    type: string;
    dateOfBirth: string;
    age: number;
    ssn: string;
    wages: number;
    hours: number;
    points: number;
    new: string;
    termDate: string | null;
    currentBalance: number;
    svc: number;
}

const sampleEmployeeData: EmployeeData[] = [
    // HARDCODED DUMMY DATA
    {
        badge: 47425,
        employeeName: "POTTER, HARRY",
        store: 3,
        type: "P", 
        dateOfBirth: "07/31/2004",
        age: 19,
        ssn: "***-**-7425",
        wages: 24850.75,
        hours: 1311,
        points: 0,
        new: "(<21)",
        termDate: null,
        currentBalance: 0.00,
        svc: 1
    },
    {
        badge: 82424,
        employeeName: "WEASLEY, RONALD",
        store: 3,
        type: "W",
        dateOfBirth: "03/01/2004",
        age: 20,
        ssn: "***-**-2424",
        wages: 22475.50,
        hours: 1054,
        points: 0,
        new: "(<21)",
        termDate: null,
        currentBalance: 0.00,
        svc: 1
    },
    {
        badge: 85744,
        employeeName: "GRANGER, HERMIONE",
        store: 3,
        type: "H",
        dateOfBirth: "09/19/2004",
        age: 20,
        ssn: "***-**-5744",
        wages: 27650.25,
        hours: 1548,
        points: 0,
        new: "(<21)",
        termDate: null,
        currentBalance: 0.00,
        svc: 1
    },
    {
        badge: 91532,
        employeeName: "LONGBOTTOM, NEVILLE",
        store: 3,
        type: "L",
        dateOfBirth: "07/30/2004",
        age: 19,
        ssn: "***-**-1532",
        wages: 23575.80,
        hours: 1275,
        points: 0,
        new: "(<21)",
        termDate: null,
        currentBalance: 0.00,
        svc: 1
    },
    {
        badge: 77889,
        employeeName: "LOVEGOOD, LUNA",
        store: 3,
        type: "L",
        dateOfBirth: "02/13/2004",
        age: 20,
        ssn: "***-**-7889",
        wages: 21980.60,
        hours: 1102,
        points: 0,
        new: "(<21)",
        termDate: null,
        currentBalance: 0.00,
        svc: 1
    }
];

const EighteenToTwentyGrid = () => {

    const navigate = useNavigate();

    const getPinnedBottomRowData = (data: any[]) => {

        return [
            {
                employeeName: "Total EMPS",
                store: 1,
                wages: 100.0,
                currentBalance: 0
            },
            {
                employeeName: "No Wages",
                store: 0,
                wages: 0,
                currentBalance: 0
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
    const columnDefs = useMemo(() => GetEighteenToTwentyGridColumns(viewBadge), []);

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`PROFIT-ELIGIBLE REPORT (${sampleEmployeeData.length || 0})`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"ELIGIBLE_EMPLOYEES"}
                isLoading={false}
                handleSortChanged={(params) => { }}
                providedOptions={{
                    rowData: sampleEmployeeData,
                    pinnedBottomRowData: getPinnedBottomRowData(sampleEmployeeData),
                    columnDefs: columnDefs
                }}
            />
        </>

    );
};

export default EighteenToTwentyGrid;