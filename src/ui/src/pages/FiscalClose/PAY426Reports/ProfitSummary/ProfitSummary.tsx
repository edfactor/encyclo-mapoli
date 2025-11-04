import { Button, Divider, Grid, Typography } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  useFinalizeReportMutation,
  useLazyGetYearEndProfitSharingSummaryReportQuery
} from "reduxstore/api/YearsEndApi";
import { YearEndProfitSharingReportSummaryLineItem } from "reduxstore/types";
import { DSMGrid, Page } from "smart-ui-library";
import { ROUTES } from "../../../../constants";
import { RootState } from "../../../../reduxstore/store";
import CommitModal from "../../../DecemberActivities/ProfitShareReport/CommitModal.tsx";
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
    totalBalance: 0,
    totalHours: 0,
    totalPoints: 0,
    totalPriorBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "2",
    lineItemTitle: ">= AGE 21 WITH >= 1000 PS HOURS",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0,
    totalHours: 0,
    totalPoints: 0,
    totalPriorBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "3",
    lineItemTitle: "<  AGE 18",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0,
    totalHours: 0,
    totalPoints: 0,
    totalPriorBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "4",
    lineItemTitle: ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0,
    totalHours: 0,
    totalPoints: 0,
    totalPriorBalance: 0
  },
  {
    subgroup: "ACTIVE AND INACTIVE",
    lineItemPrefix: "5",
    lineItemTitle: ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
    numberOfMembers: 0,
    totalWages: 0,
    totalBalance: 0,
    totalHours: 0,
    totalPoints: 0,
    totalPriorBalance: 0
  }
];

interface ProfitSummaryProps {
  frozenData: boolean;
}

const ProfitSummary: React.FC<ProfitSummaryProps> = ({ frozenData }) => {
  const [trigger, { data, isFetching }] = useLazyGetYearEndProfitSharingSummaryReportQuery();
  const [shouldArchive, setShouldArchive] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const navigate = useNavigate();
  const [finalizeReport, { isLoading: isFinalizing }] = useFinalizeReportMutation();

  const handleCommit = async () => {
    if (profitYear) {
      try {
        await finalizeReport({ profitYear });
        setIsModalOpen(false);
      } catch (error) {
        console.error("Failed to finalize report:", error);
      }
    }
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const handleStatusChange = (_newStatus: string, statusName?: string) => {
    // Only set shouldArchive to true when transitioning TO "Complete" status
    if (statusName === "Complete") {
      setShouldArchive(true);
    }
  };

  const getPresetNumberForLineItem = (lineItemPrefix: string): string | null => {
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

    return presetMap[lineItemPrefix] || null;
  };

  const handleRowClick = (event: { data: YearEndProfitSharingReportSummaryLineItem }) => {
    const rowData = event.data;
    const clickedLineItem = rowData.lineItemPrefix;
    const presetNumber = getPresetNumberForLineItem(clickedLineItem);

    if (presetNumber) {
      // Navigate to PAY426N with the preset number
      navigate(`/${ROUTES.PAY426N_LIVE}/${presetNumber}`);
    }
  };

  useEffect(() => {
    if (hasToken) {
      trigger({
        useFrozenData: frozenData,
        profitYear: profitYear,
        badgeNumber: null,
        archive: false
      });
    }
  }, [trigger, profitYear, hasToken, frozenData]);

  // Reload with archive=true when status changes to Complete
  useEffect(() => {
    if (shouldArchive && hasToken) {
      trigger({
        useFrozenData: frozenData,
        profitYear: profitYear,
        badgeNumber: null,
        archive: true
      });
      setShouldArchive(false);
    }
  }, [shouldArchive, hasToken, frozenData, profitYear, trigger]);

  const renderActionNode = () => {
    if (!frozenData) return null;

    return (
      <div className="flex h-10 items-center gap-2">
        <Button
          onClick={() => setIsModalOpen(true)}
          variant="outlined"
          className="h-10 min-w-fit whitespace-nowrap">
          Commit
        </Button>

        <StatusDropdownActionNode onStatusChange={handleStatusChange} />
      </div>
    );
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
        numberOfMembers: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
        totalWages: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.totalWages, 0),
        totalBalance: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.totalBalance, 0),
        totalHours: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.totalHours, 0),
        totalPoints: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.totalPoints, 0)
      }
    ];
  }, [activeAndInactiveRowData]);

  const getTerminatedTotals = useMemo(() => {
    if (!terminatedRowData) return [];

    return [
      {
        lineItemTitle: "TOTAL",
        numberOfMembers: terminatedRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
        totalWages: terminatedRowData.reduce((acc, curr) => acc + curr.totalWages, 0),
        totalBalance: terminatedRowData.reduce((acc, curr) => acc + curr.totalBalance, 0),
        totalHours: terminatedRowData.reduce((acc, curr) => acc + curr.totalHours, 0),
        totalPoints: terminatedRowData.reduce((acc, curr) => acc + curr.totalPoints, 0)
      }
    ];
  }, [terminatedRowData]);

  return (
    <Page
      label={`SUMMARY`}
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

      <CommitModal
        open={isModalOpen}
        onClose={handleCancel}
        onCommit={handleCommit}
        isFinalizing={isFinalizing}
      />
    </Page>
  );
};

export default ProfitSummary;
