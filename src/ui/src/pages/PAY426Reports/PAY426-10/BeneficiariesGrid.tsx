import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { DSMGrid } from "smart-ui-library";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { Path } from "react-router-dom";
import { GetBeneficiariesGridColumns } from "./BeneficiariesGridColumns";

const BeneficiariesGrid = () => {
    const navigate = useNavigate();
    const [trigger, { data, isLoading, error }] = useLazyGetYearEndProfitSharingReportQuery();

    useEffect(() => {
        trigger({
            isYearEnd: true,
            minimumAgeInclusive: 0,
            maximumAgeInclusive: 200,
            minimumHoursInclusive: 0,
            maximumHoursInclusive: 4000,
            includeActiveEmployees: false,
            includeInactiveEmployees: false,
            includeEmployeesTerminatedThisYear: false,
            includeTerminatedEmployees: false,
            includeBeneficiaries: true,
            includeEmployeesWithPriorProfitSharingAmounts: false,
            includeEmployeesWithNoPriorProfitSharingAmounts: false,
            profitYear: 2024,
            pagination: {
                skip: 0,
                take: 25
            }
        });
    }, [trigger]);

    const getPinnedBottomRowData = useMemo(() => {
        if (!data) return [];
        
        const beneficiaryCount = data.response?.results?.length || 0;
        const wagesTotal = data.wagesTotal || 0;
        
        return [
            {
                employeeName: `Total Non-EMPs Beneficiaries`,
                storeNumber: beneficiaryCount,
                wages: wagesTotal,
                balance: data.response?.results?.reduce((total, curr) => total + (curr.balance || 0), 0) || 0
            },
            {
                employeeName: "No Wages",
                storeNumber: 0,
                wages: 0,
                balance: 0
            }
        ];
    }, [data]);

    const handleNavigationForButton = useCallback(
        (destination: string | Partial<Path>) => {
            navigate(destination);
        },
        [navigate]
    );

    const columnDefs = useMemo(
        () => GetBeneficiariesGridColumns(handleNavigationForButton),
        [handleNavigationForButton]
    );

    return (
        <>
            <div style={{ padding: "0 24px 0 24px" }}>
                <Typography
                    variant="h2"
                    sx={{ color: "#0258A5" }}>
                    {`NON-EMPLOYEE BENEFICIARIES (${data?.response?.total || 0} records)`}
                </Typography>
            </div>
            <DSMGrid
                preferenceKey={"BENEFICIARIES"}
                isLoading={isLoading}
                handleSortChanged={(_params) => {}}
                providedOptions={{
                    rowData: data?.response?.results || [],
                    pinnedBottomRowData: getPinnedBottomRowData,
                    columnDefs: columnDefs
                }}
            />
        </>
    );
};

export default BeneficiariesGrid;