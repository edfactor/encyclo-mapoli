import { useEffect, useMemo, useState } from "react";
import { useLazyGetProfitMasterInquiryGroupingQuery, useLazyGetProfitMasterInquiryFilteredDetailsQuery } from "reduxstore/api/InquiryApi";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { Typography, Box, CircularProgress } from "@mui/material";
import { RootState } from "reduxstore/store";
import { useSelector } from "react-redux";
import { MasterInquiryRequest, GroupedProfitSummaryDto, MasterInquiryResponseDto } from "reduxstore/types";
import { NestedGrid } from "components/DSMNestedGrid/NestedGrid";
import { INestedGridColumn, INestedGridRowData } from "components/DSMNestedGrid/NestedGridRow";
import { numberToCurrency, DSMGrid } from "smart-ui-library";

const MasterInquiryGroupingGrid = ({ searchParams }: { searchParams: MasterInquiryRequest }) => {
    const [getProfitMasterInquiryGrouping, { isLoading: isGroupingLoading }] = useLazyGetProfitMasterInquiryGroupingQuery();
    const [getFilteredDetails, { data: filteredDetailsData, isFetching: isFetchingFilteredDetails }] = useLazyGetProfitMasterInquiryFilteredDetailsQuery();
    const [expandedRowIds, setExpandedRowIds] = useState<Set<string>>(new Set());
    const [expandedRowDataMap, setExpandedRowDataMap] = useState<Record<string, MasterInquiryResponseDto[]>>({});
    
    const searchParamsForQuery = useMemo(() => ({
        ...searchParams,
        pagination: {
            skip: 0,
            take: 25,
            sortBy: "profitYear",
            isSortDescending: true
        },
        memberType: searchParams.memberType === 0 ? undefined : searchParams.memberType,
        paymentType: searchParams.paymentType === 0 ? undefined : searchParams.paymentType
    }), [searchParams]);

    useEffect(() => {
        getProfitMasterInquiryGrouping(searchParamsForQuery);
    }, [getProfitMasterInquiryGrouping, searchParamsForQuery]);

    useEffect(() => {
        if (filteredDetailsData?.results && expandedRowIds.size > 0) {
            const latestExpandedRowId = Array.from(expandedRowIds).pop();
            if (latestExpandedRowId) {
                setExpandedRowDataMap(prev => ({
                    ...prev,
                    [latestExpandedRowId]: filteredDetailsData.results || []
                }));
            }
        }
    }, [filteredDetailsData, expandedRowIds]);

    const { masterInquiryGroupingData } = useSelector((state: RootState) => state.inquiry);

    const groupingColumns = useMemo((): INestedGridColumn<GroupedProfitSummaryDto>[] => [
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

    const nestedGridData = useMemo((): INestedGridRowData<GroupedProfitSummaryDto>[] =>
        masterInquiryGroupingData?.map((item: GroupedProfitSummaryDto) => ({
            id: `${item.profitYear}-${item.monthToDate}`,
            ...item
        })) || []
        , [masterInquiryGroupingData]);

    const detailColumns = useMemo(() => GetMasterInquiryGridColumns(), []);

    const renderNestedContent = (row: INestedGridRowData<GroupedProfitSummaryDto>, isExpanded: boolean) => {
        const rowId = String(row.id);

        if (!isExpanded) {
            if (expandedRowIds.has(rowId)) {
                setExpandedRowIds(prev => {
                    const newSet = new Set(prev);
                    newSet.delete(rowId);
                    return newSet;
                });
                setExpandedRowDataMap(prev => {
                    const { [rowId]: _, ...rest } = prev;
                    return rest;
                });
            }
            return null;
        }

        if (!expandedRowIds.has(rowId)) {
            setExpandedRowIds(prev => new Set(prev).add(rowId));
            
            getFilteredDetails({
                memberType: searchParams.memberType || 0,
                profitYear: row.profitYear,
                monthToDate: row.monthToDate,
                badgeNumber: searchParams.badgeNumber,
                psnSuffix: searchParams.psnSuffix,
                ssn: searchParams.ssn?.toString(),
                startProfitMonth: searchParams.startProfitMonth,
                endProfitMonth: searchParams.endProfitMonth,
                profitCode: searchParams.profitCode,
                contributionAmount: searchParams.contributionAmount,
                earningsAmount: searchParams.earningsAmount,
                forfeitureAmount: searchParams.forfeitureAmount,
                paymentAmount: searchParams.paymentAmount,
                name: searchParams.name,
                paymentType: searchParams.paymentType,
                skip: 0,
                take: 25,
                sortBy: "profitYear",
                isSortDescending: true
            });
        }

        return (
            <Box sx={{ mx: 0, px: 0, py: 0 }}>
                <DSMGrid
                    preferenceKey={`master-inquiry-detail-${row.id}`}
                    isLoading={isFetchingFilteredDetails && !expandedRowDataMap[rowId]}
                    showColumnControl={false}
                    maxHeight={250}
                    providedOptions={{
                        rowData: expandedRowDataMap[rowId] || [],
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

    if (isGroupingLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (!masterInquiryGroupingData?.length) {
        return (
            <Typography variant="body1" sx={{ padding: 2 }}>
                No profit grouping data found for the selected criteria.
            </Typography>
        );
    }

    return (
        <NestedGrid
            title={`Master Inquiry (${masterInquiryGroupingData.length} ${masterInquiryGroupingData.length === 1 ? "Record" : "Records"})`}
            data={nestedGridData}
            columns={groupingColumns}
            renderNestedContent={renderNestedContent}
            className="w-full"
        />
    );
}

export default MasterInquiryGroupingGrid;