import { Typography } from "@mui/material";
import { useEffect, useMemo } from "react";
import { DSMGrid } from "smart-ui-library";
import { useLazyGetYearEndProfitSharingSummaryReportQuery } from "reduxstore/api/YearsEndApi";
import { GetProfitSummaryGridColumns } from "./ProfitSummaryGridColumns";
import { YearEndProfitSharingReportSummaryLineItem } from "reduxstore/types";

/**
 * Default rows for "Active and Inactive" section - these will display with zero values
 * if no data is returned from the API for any particular row
 */
const activeInactivePlaceholders: YearEndProfitSharingReportSummaryLineItem[] = [
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "1",
    lineItemTitle: "AGE 18-20 WITH >= 1000 PS HOURS",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "2",
    lineItemTitle: ">= AGE 21 WITH >= 1000 PS HOURS",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "3",
    lineItemTitle: "<  AGE 18",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "4",
    lineItemTitle: ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "5",
    lineItemTitle: ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  }
];

/**
 * Default rows for "Terminated" section - these will display with zero values
 * if no data is returned from the API for any particular row
 */
const terminatedPlaceholders: YearEndProfitSharingReportSummaryLineItem[] = [
  {
    subgroup: "TERMINATED",
    lineItemPrefix: "6",
    lineItemTitle: ">= AGE 18 WITH >= 1000 PS HOURS",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "TERMINATED",
    lineItemPrefix: "7",
    lineItemTitle: ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "TERMINATED",
    lineItemPrefix: "8",
    lineItemTitle: ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "TERMINATED",
    lineItemPrefix: "9",
    lineItemTitle: "< AGE 18 NO WAGES",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  },
  {
    subgroup: "TERMINATED",
    lineItemPrefix: "N",
    lineItemTitle: "NON-EMPLOYEE BENEFICIARIES",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0
  }
];

const ProfitSummary = () => {
  const [trigger, { data, isFetching }] = useLazyGetYearEndProfitSharingSummaryReportQuery();

  useEffect(() => {
    trigger({
      useFrozenData: true,
      profitYear: 2024
    });
  }, [trigger]);

  const columnDefs = useMemo(() => GetProfitSummaryGridColumns(), []);

  // Here we combine the API data with placeholders for "Active and Inactive" and "terminated" section
  // If data exists for a row, it replaces the placeholder; otherwise the placeholder is used - ensuring all rows are displayed regardless of whether data exists for them
  const activeAndInactiveRowData = useMemo(() => {
    if (!data?.lineItems) return activeInactivePlaceholders;
    
    const dataMap = new Map(
      data.lineItems
        .filter(item => item.subgroup.toUpperCase() === "ACTIVE AND INACTIVE")
        .map(item => [item.lineItemPrefix, item])
    );
    
    return activeInactivePlaceholders.map(placeholder => 
      dataMap.get(placeholder.lineItemPrefix) || placeholder
    );
  }, [data]);

  const terminatedRowData = useMemo(() => {
    if (!data?.lineItems) return terminatedPlaceholders;
    
    const dataMap = new Map(
      data.lineItems
        .filter(item => item.subgroup.toUpperCase() === "TERMINATED")
        .map(item => [item.lineItemPrefix, item])
    );
    
    return terminatedPlaceholders.map(placeholder => 
      dataMap.get(placeholder.lineItemPrefix) || placeholder
    );
  }, [data]);

  const getActiveAndInactiveTotals = useMemo(() => {
    const totals = activeAndInactiveRowData.reduce(
      (acc, curr) => ({
        numberOfMembers: acc.numberOfMembers + curr.numberOfMembers,
        totalWages: acc.totalWages + curr.totalWages,
        totalBalance: acc.totalBalance + curr.totalBalance
      }),
      { numberOfMembers: 0, totalWages: 0, totalBalance: 0 }
    );

    return [
      {
        lineItemTitle: "Total all reports",
        numberOfMembers: totals.numberOfMembers,
        totalWages: totals.totalWages,
        totalBalance: totals.totalBalance
      }
    ];
  }, [activeAndInactiveRowData]);

  const getTerminatedTotals = useMemo(() => {
    const totals = terminatedRowData.reduce(
      (acc, curr) => ({
        numberOfMembers: acc.numberOfMembers + curr.numberOfMembers,
        totalWages: acc.totalWages + curr.totalWages,
        totalBalance: acc.totalBalance + curr.totalBalance
      }),
      { numberOfMembers: 0, totalWages: 0, totalBalance: 0 }
    );

    return [
      {
        lineItemTitle: "Total all reports",
        numberOfMembers: totals.numberOfMembers,
        totalWages: totals.totalWages,
        totalBalance: totals.totalBalance
      }
    ];
  }, [terminatedRowData]);

  return (
    <>
      <div>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", padding: "0 24px" }}>
          Profit Sharing Summary Total Page
        </Typography>
      </div>
      <div>
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", padding: "12px 24px", fontWeight: "bold" }}>
          Active and Inactive
        </Typography>
        <DSMGrid
          preferenceKey={"ACTIVE_INACTIVE_SUMMARY"}
          isLoading={isFetching}
          handleSortChanged={() => {}}
          providedOptions={{
            rowData: activeAndInactiveRowData,
            pinnedTopRowData: getActiveAndInactiveTotals,
            columnDefs: columnDefs
          }}
        />
      </div>

      <div>
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", padding: "12px 24px", fontWeight: "bold" }}>
          Terminated
        </Typography>
        <DSMGrid
          preferenceKey={"TERMINATED_SUMMARY"}
          isLoading={isFetching}
          handleSortChanged={() => {}}
          providedOptions={{
            rowData: terminatedRowData,
            pinnedTopRowData: getTerminatedTotals,
            columnDefs: columnDefs
          }}
        />
      </div>
    </>
  );
};

export default ProfitSummary;