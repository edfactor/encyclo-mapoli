import { Button, Typography } from "@mui/material";
import { useMemo, useEffect } from "react";
import { DSMGrid } from "smart-ui-library";
import { useNavigate } from "react-router";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";

const PriorHoursGrid = () => {
    const navigate = useNavigate();
    const [trigger, { data, isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

    useEffect(() => {
        trigger({
            isYearEnd: true,
            minimumAgeInclusive: 18,
            maximumAgeInclusive: 200,
            minimumHoursInclusive: 0,
            maximumHoursInclusive: 999.99,
            includeActiveEmployees: true,
            includeInactiveEmployees: true,
            includeEmployeesTerminatedThisYear: false,
            includeTerminatedEmployees: false,
            includeBeneficiaries: false,
            includeEmployeesWithPriorProfitSharingAmounts: true,
            includeEmployeesWithNoPriorProfitSharingAmounts: false,
            profitYear: 2024,
            pagination: {
                skip: 0,
                take: 25
            }
        });
    }, [trigger]);

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

    const columnDefs = useMemo(() => GetProfitSharingReportGridColumns(viewBadge), []);

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`ACTIVE/INACTIVE WITH PRIOR PS REPORT (${data?.response?.results?.length || 0})`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"PRIOR_HOURS_EMPLOYEES"}
                isLoading={isLoading}
                handleSortChanged={(params) => { }}
                providedOptions={{
                    rowData: data?.response?.results || [],
                    columnDefs: columnDefs
                }}
            />
        </>
    );
};

export default PriorHoursGrid;