import { Divider, Grid, Typography } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingSummaryReportQuery } from "reduxstore/api/YearsEndApi";
import { FilterParams, YearEndProfitSharingReportSummaryLineItem } from "reduxstore/types";
import { DSMGrid, Page } from "smart-ui-library";
import { RootState } from "../../../reduxstore/store";
import presets from "../PAY426N/presets";
import { GetProfitSummaryGridColumns } from "./ProfitSummaryGridColumns";

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

interface ProfitSummaryProps {
  onPresetParamsChange?: (params: FilterParams | null) => void;
}

const ProfitSummary: React.FC<ProfitSummaryProps> = ({ onPresetParamsChange }) => {
  const [trigger, { data, isFetching }] = useLazyGetYearEndProfitSharingSummaryReportQuery();
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [selectedLineItem, setSelectedLineItem] = useState<string | null>(null);
  const [isStatusCompleted, setIsStatusCompleted] = useState(false);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");

  // Get the current navigation object to check its status
  const getCurrentNavigationObject = () => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const findNavigationById = (navigationArray: any[], id: number): any => {
      for (const item of navigationArray) {
        if (item.id === id) {
          return item;
        }
        if (item.items && item.items.length > 0) {
          const found = findNavigationById(item.items, id);
          if (found) {
            return found;
          }
        }
      }
      return null;
    };

    if (navigationList?.navigation && currentNavigationId) {
      return findNavigationById(navigationList.navigation, currentNavigationId);
    }
    return null;
  };

  // Check if current status is "Completed"
  useEffect(() => {
    const currentNav = getCurrentNavigationObject();
    if (currentNav) {
      setIsStatusCompleted(currentNav.statusName === "Complete");
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [navigationList, currentNavigationId]);

  const getPresetForLineItem = (lineItemPrefix: string): FilterParams | null => {
    const presetMap: { [key: string]: string } = {
      "1": "1",
      "2": "2",
      "3": "3",
      "4": "4",
      "5": "5",
      "6": "6",
      "7": "7",
      "8": "8",
      "9": "9",
      N: "10"
    };

    const presetId = presetMap[lineItemPrefix];
    return presets.find((preset) => preset.id === presetId)?.params || null;
  };

  const handleRowClick = (event: { data: YearEndProfitSharingReportSummaryLineItem }) => {
    const rowData = event.data;
    const clickedLineItem = rowData.lineItemPrefix;

    setSelectedLineItem((prevSelected) => {
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
        profitYear: profitYear,
        badgeNumber: null
      });
    }
  }, [trigger, profitYear, hasToken, isStatusCompleted]);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const columnDefs = useMemo(() => GetProfitSummaryGridColumns(), []);

  // Here we combine the API data with placeholders for "Active and Inactive" and "terminated" section
  // If data exists for a row, it replaces the placeholder; otherwise the placeholder is used - ensuring all rows are displayed regardless of whether data exists for them
  const activeAndInactiveRowData = useMemo(() => {
    if (!data?.lineItems) return activeInactivePlaceholders;

    const dataMap = new Map(
      data.lineItems
        .filter((item) => item.subgroup.toUpperCase() === "ACTIVE AND INACTIVE")
        .map((item) => [item.lineItemPrefix, item])
    );

    return activeInactivePlaceholders.map((placeholder) => dataMap.get(placeholder.lineItemPrefix) || placeholder);
  }, [data]);

  const terminatedRowData = useMemo(() => {
    if (!data?.lineItems) return [];

    return data.lineItems.filter((item) => item.subgroup.toUpperCase() === "TERMINATED");
  }, [data]);

  const getActiveAndInactiveTotals = useMemo(() => {
    if (!activeAndInactiveRowData) return [];

    return [
      {
        lineItemTitle: "TOTAL",
        numberOfMembers: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0)
      }
    ];
  }, [activeAndInactiveRowData]);

  const getTerminatedTotals = useMemo(() => {
    if (!terminatedRowData) return [];

    return [
      {
        lineItemTitle: "TOTAL",
        numberOfMembers: terminatedRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0)
      }
    ];
  }, [terminatedRowData]);

  //const shouldShowDetailGrid = selectedLineItem && getPresetForLineItem(selectedLineItem);

  return (
    <>
      <Page
        label="Profit Summary"
        actionNode={renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          <Grid width={"100%"}>
            <Divider />
          </Grid>

          <Grid width={"100%"}>
            <Typography
              variant="h6"
              sx={{ mb: 2, px: 3 }}>
              Active and Inactive
            </Typography>
            <DSMGrid
              preferenceKey={"ACTIVE_INACTIVE_SUMMARY"}
              isLoading={isFetching}
              handleSortChanged={() => {}}
              providedOptions={{
                rowData: activeAndInactiveRowData,
                pinnedTopRowData: getActiveAndInactiveTotals,
                columnDefs: columnDefs,
                onRowClicked: handleRowClick
              }}
            />
          </Grid>

          <Grid width={"100%"}>
            <Typography
              variant="h6"
              sx={{ mb: 2, px: 3 }}>
              Terminated
            </Typography>
            <DSMGrid
              preferenceKey={"TERMINATED_SUMMARY"}
              isLoading={isFetching}
              handleSortChanged={() => {}}
              providedOptions={{
                rowData: terminatedRowData,
                pinnedTopRowData: getTerminatedTotals,
                columnDefs: columnDefs,
                onRowClicked: handleRowClick
              }}
            />
          </Grid>
        </Grid>
      </Page>
    </>
  );
};

export default ProfitSummary;
