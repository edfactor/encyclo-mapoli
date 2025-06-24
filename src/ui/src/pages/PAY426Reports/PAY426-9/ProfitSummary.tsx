import { Divider, Grid2, Typography } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { DSMGrid, Page } from "smart-ui-library";
import { useLazyGetYearEndProfitSharingSummaryReportQuery } from "reduxstore/api/YearsEndApi";
import { GetProfitSummaryGridColumns } from "./ProfitSummaryGridColumns";
import { YearEndProfitSharingReportSummaryLineItem, FilterParams } from "reduxstore/types";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import presets from "../PAY426N/presets";

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

interface ProfitSummaryProps {
  onPresetParamsChange?: (params: FilterParams | null) => void;
}

const ProfitSummary: React.FC<ProfitSummaryProps> = ({ onPresetParamsChange }) => {
  const [trigger, { data, isFetching }] = useLazyGetYearEndProfitSharingSummaryReportQuery();
  const [selectedLineItem, setSelectedLineItem] = useState<string | null>(null);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();

  const getPresetForLineItem = (lineItemPrefix: string): FilterParams | null => {
    const presetMap: { [key: string]: string } = {
      "1": "PAY426-1",
      "2": "PAY426-2",
      "3": "PAY426-3",
      "4": "PAY426-4",
      "5": "PAY426-5",
      "6": "PAY426-6",
      "7": "PAY426-7",
      "8": "PAY426-8",
      "9": "PAY426-3",
      "N": "PAY426-10"
    };
    
    const presetId = presetMap[lineItemPrefix];
    return presets.find(preset => preset.id === presetId)?.params || null;
  };

  const handleRowClick = (event: { data: YearEndProfitSharingReportSummaryLineItem }) => {
    const rowData = event.data;
    const clickedLineItem = rowData.lineItemPrefix;
    
    setSelectedLineItem(prevSelected => {
      const newSelected = prevSelected === clickedLineItem ? null : clickedLineItem;
      if (newSelected) {
        const params = getPresetForLineItem(newSelected);
        onPresetParamsChange?.(params);
      } else {
        onPresetParamsChange?.(null);
      }
      return newSelected;
    });
  };

  useEffect(() => {
    if (hasToken) {
      trigger({
        useFrozenData: true,
        profitYear: profitYear
      });
    }
  }, [trigger, profitYear, hasToken]);

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode />
    );
  };
  
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
    if (!data?.lineItems) return [];

    return data.lineItems.filter(item => item.subgroup.toUpperCase() === "TERMINATED");
  }, [data]);

  const getActiveAndInactiveTotals = useMemo(() => {
    if (!activeAndInactiveRowData) return [];

    return [{
      lineItemTitle: "TOTAL",
      numberOfMembers: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
      totalWages: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.totalWages, 0),
      totalBalance: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.totalBalance, 0)
    }];
  }, [activeAndInactiveRowData]);

  const getTerminatedTotals = useMemo(() => {
    if (!terminatedRowData) return [];

    return [{
      lineItemTitle: "TOTAL",
      numberOfMembers: terminatedRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
      totalWages: terminatedRowData.reduce((acc, curr) => acc + curr.totalWages, 0),
      totalBalance: terminatedRowData.reduce((acc, curr) => acc + curr.totalBalance, 0)
    }];
  }, [terminatedRowData]);

  const shouldShowDetailGrid = selectedLineItem && getPresetForLineItem(selectedLineItem);

  return (
    <>
      <Page label="Profit Summary" actionNode={renderActionNode()}>
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>

          <Grid2 width={"100%"}>
            <Typography variant="h6" sx={{ mb: 2, px: 3 }}>
              Active and Inactive
            </Typography>
            <DSMGrid
              preferenceKey={"ACTIVE_INACTIVE_SUMMARY"}
              isLoading={isFetching}
              handleSortChanged={() => { }}
              providedOptions={{
                rowData: activeAndInactiveRowData,
                pinnedTopRowData: getActiveAndInactiveTotals,
                columnDefs: columnDefs,
                onRowClicked: handleRowClick
              }}
            />
          </Grid2>

          <Grid2 width={"100%"}>
            <Typography variant="h6" sx={{ mb: 2, px: 3 }}>
              Terminated
            </Typography>
            <DSMGrid
              preferenceKey={"TERMINATED_SUMMARY"}
              isLoading={isFetching}
              handleSortChanged={() => { }}
              providedOptions={{
                rowData: terminatedRowData,
                pinnedTopRowData: getTerminatedTotals,
                columnDefs: columnDefs,
                onRowClicked: handleRowClick
              }}
            />
          </Grid2>

        </Grid2>
      </Page>
    </>
  );
};

export default ProfitSummary;