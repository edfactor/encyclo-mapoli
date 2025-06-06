import { useEffect, useMemo } from "react";
import { useLazyGetProfitMasterInquiryGroupingQuery } from "reduxstore/api/InquiryApi";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { Typography, Box, CircularProgress } from "@mui/material";
import { RootState } from "reduxstore/store";
import { useSelector } from "react-redux";
import { MasterInquiryRequest, GroupedProfitSummaryDto } from "reduxstore/types";
import { NestedGrid } from "components/DSMNestedGrid/NestedGrid";
import { INestedGridColumn, INestedGridRowData } from "components/DSMNestedGrid/NestedGridRow";
import { numberToCurrency, DSMGrid } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

const MasterInquiryGroupingGrid = ({ searchParams }: { searchParams: MasterInquiryRequest }) => {
    const [getProfitMasterInquiryGrouping, { isLoading: isGroupingLoading }] = useLazyGetProfitMasterInquiryGroupingQuery();

    const searchParamsForQuery = {
        profitYear: searchParams.profitYear,
        endProfitYear: searchParams.endProfitYear,
        sortBy: null,
        isSortDescending: null,
        pagination: {
            skip: 0,
            take: 255,
            sortBy: "profitYear",
            isSortDescending: true
        }
    };

    useEffect(() => {
        getProfitMasterInquiryGrouping({
            ...searchParamsForQuery,
        });
    }, [getProfitMasterInquiryGrouping, searchParams]);


    const { masterInquiryGroupingData } = useSelector((state: RootState) => state.inquiry);

    const groupingColumns: INestedGridColumn[] = useMemo(() => [
        {
            key: 'profitYear',
            label: 'Profit Year',
            width: 120,
            render: (value: number) => (
                <Typography sx={{ fontWeight: 500, color: '#231F20' }}>
                    {value}
                </Typography>
            )
        },
        {
            key: 'monthToDate',
            label: 'Month',
            width: 100,
            align: 'center',
            render: (value: number) => (
                <Typography sx={{ color: '#231F20' }}>
                    {String(value).padStart(2, '0')}
                </Typography>
            )
        },
        {
            key: 'totalContribution',
            label: 'Total Contributions',
            width: 150,
            align: 'right',
            render: (value: number) => (
                <Typography sx={{ color: '#231F20' }}>
                    {numberToCurrency(value)}
                </Typography>
            )
        },
        {
            key: 'totalEarnings',
            label: 'Total Earnings',
            width: 150,
            align: 'right',
            render: (value: number) => (
                <Typography sx={{ color: '#231F20' }}>
                    {numberToCurrency(value)}
                </Typography>
            )
        },
        {
            key: 'totalForfeiture',
            label: 'Total Forfeitures',
            width: 150,
            align: 'right',
            render: (value: number) => (
                <Typography sx={{ color: '#231F20' }}>
                    {numberToCurrency(value)}
                </Typography>
            )
        },
        {
            key: 'totalPayment',
            label: 'Total Payments',
            width: 150,
            align: 'right',
            render: (value: number) => (
                <Typography sx={{ color: '#231F20' }}>
                    {numberToCurrency(value)}
                </Typography>
            )
        },
        {
            key: 'transactionCount',
            label: 'Transaction Count',
            width: 150,
            align: 'right',
            render: (value: number) => (
                <Typography sx={{ color: '#231F20' }}>
                    {value.toLocaleString()}
                </Typography>
            )
        }
    ], []);

    const nestedGridData: INestedGridRowData[] = useMemo(() => {
        if (!masterInquiryGroupingData) return [];
        
        return masterInquiryGroupingData.map((item: GroupedProfitSummaryDto) => ({
            id: `${item.profitYear}-${item.monthToDate}`,
            ...item
        }));
    }, [masterInquiryGroupingData]);

    const renderNestedContent = (row: INestedGridRowData, isExpanded: boolean) => {
        if (!isExpanded) return null;

        const detailColumns: ColDef[] = [
            { field: "badge", headerName: "Badge", width: 80, cellStyle: { textAlign: 'center' } },
            { field: "name", headerName: "Name", width: 180, flex: 1, minWidth: 150 },
            { field: "ssn", headerName: "SSN", width: 120, cellStyle: { textAlign: 'center' } },
            { field: "date", headerName: "Date", width: 100 },
            { field: "distributionAmount", headerName: "Distribution Amount", width: 150, valueFormatter: (params) => numberToCurrency(params.value), cellStyle: { textAlign: 'right' } },
            { field: "stateTax", headerName: "State Tax", width: 110, valueFormatter: (params) => numberToCurrency(params.value), cellStyle: { textAlign: 'right' } },
            { field: "federalTax", headerName: "Federal Tax", width: 110, valueFormatter: (params) => numberToCurrency(params.value), cellStyle: { textAlign: 'right' } },
            { field: "forfeitAmount", headerName: "Forfeit Amount", width: 130, valueFormatter: (params) => numberToCurrency(params.value), cellStyle: { textAlign: 'right' } },
            { field: "tc", headerName: "T C", width: 60, cellStyle: { textAlign: 'center' } },
            { field: "age", headerName: "Age", width: 70, cellStyle: { textAlign: 'center' } },
            { field: "otherName", headerName: "Other Name", width: 120 },
            { field: "distribution", headerName: "Distribution", width: 120 }
        ];

        const dummyDetailData = [
            { badge: "47479", name: "BACHELIER, BRAD R", ssn: "***-**-7423", date: "MM/DD/YYYY", distributionAmount: 15000, stateTax: 750, federalTax: 3000, forfeitAmount: 0, tc: "X", age: 45, otherName: "", distribution: "XX" },
            { badge: "28294", name: "BRADL, STEVEN", ssn: "***-**-2824", date: "MM/DD/YYYY", distributionAmount: 22000, stateTax: 1100, federalTax: 4400, forfeitAmount: 0, tc: "A", age: 52, otherName: "", distribution: "XX" },
            { badge: "38744", name: "BRADLEY, ZACHARY W", ssn: "***-**-5744", date: "MM/DD/YYYY", distributionAmount: 8500, stateTax: 425, federalTax: 1700, forfeitAmount: 0, tc: "B", age: 38, otherName: "", distribution: "XX" },
            { badge: "54863", name: "COCHRAN, BRENDAN E", ssn: "***-**-4861", date: "MM/DD/YYYY", distributionAmount: 12750, stateTax: 637, federalTax: 2550, forfeitAmount: 0, tc: "C", age: 41, otherName: "", distribution: "XX" }
        ];

        return (
            <Box sx={{ mx: 0, px: 0, py: 0 }}>
                <DSMGrid
                    preferenceKey={`master-inquiry-detail-${row.id}`}
                    isLoading={false}
                    showColumnControl={false}
                    maxHeight={250}
                    providedOptions={{
                        rowData: dummyDetailData,
                        columnDefs: detailColumns,
                        defaultColDef: {
                            resizable: true,
                            sortable: true,
                            floatingFilter: false,
                            cellStyle: { paddingLeft: '8px', paddingRight: '8px', fontSize: '13px' }
                        },
                        rowHeight: 40,
                        headerHeight: 36,
                        suppressRowClickSelection: true,
                        suppressCellFocus: true
                    }}
                />
            </Box>
        );
    };

    return (
        <div style={{ width: '100%' }}>
            {isGroupingLoading && 
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                    <CircularProgress />
                </Box>
            }

            {masterInquiryGroupingData && masterInquiryGroupingData.length > 0 ? (
                <NestedGrid 
                    title={`Master Inquiry (${masterInquiryGroupingData.length} ${masterInquiryGroupingData.length === 1 ? "Record" : "Records"})`}
                    data={nestedGridData}
                    columns={groupingColumns}
                    renderNestedContent={renderNestedContent}
                    className="w-full"
                />
            ) : !isGroupingLoading && masterInquiryGroupingData && masterInquiryGroupingData.length === 0 ? (
                <Typography variant="body1" sx={{ padding: 2 }}>
                    No profit grouping data found for the selected criteria.
                </Typography>
            ) : null}
        </div>
    )
}

export default MasterInquiryGroupingGrid;