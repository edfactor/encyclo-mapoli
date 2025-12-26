import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { Button, CircularProgress, Grid, IconButton, Typography } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useDynamicGridHeight } from "hooks/useDynamicGridHeight";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useMemo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { closeDrawer, openDrawer, setFullscreen } from "../../../../reduxstore/slices/generalSlice";
import { useLazyGetFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import {
  useFinalizeReportMutation,
  useLazyGetYearEndProfitSharingReportTotalsQuery,
  useLazyGetYearEndProfitSharingSummaryReportQuery
} from "reduxstore/api/YearsEndApi";
import { YearEndProfitSharingReportSummaryLineItem } from "reduxstore/types";
import { DSMGrid, numberToCurrency, Page } from "smart-ui-library";
import { GRID_KEYS } from "../../../../constants";
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
    lineItemTitle: "AGE 18-20 WITH >= 1000 PS HOUR",
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

/**
 * Helper to sum values that may be masked strings - returns masked string if any value is masked
 */
const sumOrMasked = (
  rowData: YearEndProfitSharingReportSummaryLineItem[],
  field: "totalWages" | "totalBalance" | "totalHours" | "totalPoints"
): number | string | null | undefined => {
  const values = rowData.map((row) => row[field]);
  const hasMaskedValue = values.some((v) => typeof v === "string" && String(v).includes("X"));
  if (hasMaskedValue) {
    // Return the first masked value as the total (since we can't sum masked data)
    return values.find((v) => typeof v === "string" && String(v).includes("X"));
  }
  // For nullable fields (hours/points), check if all values are null
  const hasAnyValue = values.some((v) => v !== null && v !== undefined);
  if (!hasAnyValue) return null;
  return values.reduce((acc, curr) => (acc as number) + (Number(curr) || 0), 0);
};

interface ProfitSummaryProps {
  frozenData: boolean;
  externalIsGridExpanded?: boolean;
  externalOnToggleExpand?: () => void;
  /** When true, triggers an archive request. Parent should set this when status changes to Complete. */
  triggerArchive?: boolean;
  /** Callback when archive request completes. Parent should reset triggerArchive to false. */
  onArchiveComplete?: () => void;
  /** Page title. Defaults to "PROFIT SUMMARY". Pass empty string to hide when embedded in parent with its own title. */
  pageTitle?: string;
}

const ProfitSummary: React.FC<ProfitSummaryProps> = ({
  frozenData,
  externalIsGridExpanded,
  externalOnToggleExpand,
  triggerArchive,
  onArchiveComplete,
  pageTitle = "PROFIT SUMMARY"
}) => {
  const dispatch = useDispatch();
  const [trigger, { isFetching }] = useLazyGetYearEndProfitSharingSummaryReportQuery();
  const [shouldArchive, setShouldArchive] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [internalIsGridExpanded, setInternalIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  // Use external expand state if provided, otherwise use internal
  const isGridExpanded = externalIsGridExpanded ?? internalIsGridExpanded;

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  // Use the appropriate Redux slice based on frozenData - live and frozen are cached separately
  const summaryData = useSelector((state: RootState) =>
    frozenData ? state.yearsEnd.profitShareSummaryReportFrozen : state.yearsEnd.profitShareSummaryReportLive
  );
  const totalsData = useSelector((state: RootState) =>
    frozenData
      ? state.yearsEnd.yearEndProfitSharingReportTotalsFrozen
      : state.yearsEnd.yearEndProfitSharingReportTotalsLive
  );
  const profitYear = useFiscalCloseProfitYear();

  const [triggerTotals] = useLazyGetYearEndProfitSharingReportTotalsQuery();
  const [finalizeReport, { isLoading: isFinalizing }] = useFinalizeReportMutation();

  // Use dynamic grid height utility hook
  useDynamicGridHeight({
    heightPercentage: isGridExpanded ? 0.85 : 0.4
  });

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

  const handleToggleGridExpand = () => {
    // Use external handler if provided
    if (externalOnToggleExpand) {
      externalOnToggleExpand();
      return;
    }

    // Otherwise use internal logic
    if (!internalIsGridExpanded) {
      // Expanding: remember current drawer state and close it
      setWasDrawerOpenBeforeExpand(isDrawerOpen || false);
      dispatch(closeDrawer());
      dispatch(setFullscreen(true));
      setInternalIsGridExpanded(true);
    } else {
      // Collapsing: restore previous state
      dispatch(setFullscreen(false));
      setInternalIsGridExpanded(false);
      if (wasDrawerOpenBeforeExpand) {
        dispatch(openDrawer());
      }
    }
  };

  // Fetch summary report if not already in Redux (live and frozen are cached separately)
  useEffect(() => {
    if (hasToken && !summaryData) {
      trigger({
        useFrozenData: frozenData,
        profitYear: profitYear,
        badgeNumber: null,
        archive: false
      });
    }
  }, [trigger, profitYear, hasToken, frozenData, summaryData]);

  // Fetch totals if not already in Redux (live and frozen are cached separately)
  useEffect(() => {
    if (hasToken && !totalsData) {
      triggerTotals({
        useFrozenData: frozenData,
        profitYear: profitYear,
        badgeNumber: null,
        archive: false
      });
    }
  }, [triggerTotals, profitYear, hasToken, frozenData, totalsData]);

  // Reload with archive=true when status changes to Complete (internal trigger)
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

  // Handle archive trigger from parent component (e.g., ProfitShareReport)
  useEffect(() => {
    if (triggerArchive && hasToken) {
      trigger({
        useFrozenData: frozenData,
        profitYear: profitYear,
        badgeNumber: null,
        archive: true
      });
      onArchiveComplete?.();
    }
  }, [triggerArchive, hasToken, frozenData, profitYear, trigger, onArchiveComplete]);

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
    if (!summaryData?.lineItems) return activeInactivePlaceholders;

    const dataMap = new Map(
      summaryData.lineItems
        .filter((item) => item.subgroup.toUpperCase() === "ACTIVE AND INACTIVE")
        .map((item) => [item.lineItemPrefix, item])
    );

    return activeInactivePlaceholders.map((placeholder) => dataMap.get(placeholder.lineItemPrefix) || placeholder);
  }, [summaryData]);

  const terminatedRowData = useMemo(() => {
    if (!summaryData?.lineItems) return [];

    return summaryData.lineItems.filter((item) => item.subgroup.toUpperCase() === "TERMINATED");
  }, [summaryData]);

  const employeesRowData = useMemo(() => {
    if (!summaryData?.lineItems) return [];

    return summaryData.lineItems.filter((item) => {
      const subgroupUpper = item.subgroup.toUpperCase();
      return (
        subgroupUpper.includes("EMPLOYEE") &&
        !subgroupUpper.includes("NON-EMPLOYEE") &&
        !subgroupUpper.includes("NON EMPLOYEE") &&
        subgroupUpper !== "ACTIVE AND INACTIVE" &&
        subgroupUpper !== "TERMINATED"
      );
    });
  }, [summaryData]);

  const getActiveAndInactiveTotals = useMemo(() => {
    if (!activeAndInactiveRowData) return [];

    return [
      {
        lineItemTitle: "TOTAL",
        numberOfMembers: activeAndInactiveRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
        totalWages: sumOrMasked(activeAndInactiveRowData, "totalWages"),
        totalBalance: sumOrMasked(activeAndInactiveRowData, "totalBalance"),
        totalHours: sumOrMasked(activeAndInactiveRowData, "totalHours"),
        totalPoints: sumOrMasked(activeAndInactiveRowData, "totalPoints")
      }
    ];
  }, [activeAndInactiveRowData]);

  const getTerminatedTotals = useMemo(() => {
    if (!terminatedRowData) return [];

    return [
      {
        lineItemTitle: "TOTAL",
        numberOfMembers: terminatedRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
        totalWages: sumOrMasked(terminatedRowData, "totalWages"),
        totalBalance: sumOrMasked(terminatedRowData, "totalBalance"),
        totalHours: sumOrMasked(terminatedRowData, "totalHours"),
        totalPoints: sumOrMasked(terminatedRowData, "totalPoints")
      }
    ];
  }, [terminatedRowData]);

  const getEmployeesTotals = useMemo(() => {
    if (!employeesRowData || employeesRowData.length === 0) return [];

    return [
      {
        lineItemTitle: "TOTAL",
        numberOfMembers: employeesRowData.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
        totalWages: sumOrMasked(employeesRowData, "totalWages"),
        totalBalance: sumOrMasked(employeesRowData, "totalBalance"),
        totalHours: sumOrMasked(employeesRowData, "totalHours"),
        totalPoints: sumOrMasked(employeesRowData, "totalPoints")
      }
    ];
  }, [employeesRowData]);

  const getGrandTotals = useMemo(() => {
    const allSections = [...activeAndInactiveRowData, ...terminatedRowData, ...(employeesRowData || [])];

    if (allSections.length === 0) return [];

    return [
      {
        lineItemTitle: "GRAND TOTAL",
        numberOfMembers: allSections.reduce((acc, curr) => acc + curr.numberOfMembers, 0),
        totalWages: sumOrMasked(allSections, "totalWages"),
        totalBalance: sumOrMasked(allSections, "totalBalance"),
        totalHours: sumOrMasked(allSections, "totalHours"),
        totalPoints: sumOrMasked(allSections, "totalPoints")
      }
    ];
  }, [activeAndInactiveRowData, terminatedRowData, employeesRowData]);

  return (
    <Page
      label={pageTitle}
      actionNode={isGridExpanded ? undefined : renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Grid
            container
            justifyContent="space-between"
            alignItems="center"
            marginBottom={2}>
            <Grid />
            <Grid>
              <IconButton
                onClick={handleToggleGridExpand}
                sx={{ zIndex: 1 }}
                aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}>
                {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
              </IconButton>
            </Grid>
          </Grid>
          <div className="mb-4 flex items-center gap-6 px-3">
            <div className="flex items-center gap-2">
              <span className="font-semibold">Total Employees:</span>
              <span>{getGrandTotals[0]?.numberOfMembers?.toLocaleString() ?? 0}</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="font-semibold">Total Wages:</span>
              <span>
                {typeof getGrandTotals[0]?.totalWages === "string"
                  ? getGrandTotals[0]?.totalWages
                  : numberToCurrency(getGrandTotals[0]?.totalWages ?? 0)}
              </span>
            </div>
            <div className="flex items-center gap-2">
              <span className="font-semibold">Total Balance:</span>
              <span>
                {typeof getGrandTotals[0]?.totalBalance === "string"
                  ? getGrandTotals[0]?.totalBalance
                  : numberToCurrency(getGrandTotals[0]?.totalBalance ?? 0)}
              </span>
            </div>
          </div>
        </Grid>

        <Grid width={"100%"}>
          <Typography
            variant="h6"
            sx={{ mb: 2, px: 3 }}>
            Plan Eligibility (18+, 1000+ Hours)
          </Typography>
          <div className="px-3">
            {/* One-off inline styles for this simple table. If we reuse this pattern, move to a shared CSS module. */}
            <style>{`
              .eligibility-table {
                width: 100%;
                border-collapse: collapse;
              }
              .eligibility-table thead tr {
                background-color: #E8E8E8;
              }
              .eligibility-table th,
              .eligibility-table td {
                padding: 0.5rem 1rem;
                text-align: right;
                font-size: 0.875rem;
              }
              .eligibility-table th {
                font-weight: 500;
              }
            `}</style>
            <div className="rounded border border-gray-300">
              <table className="eligibility-table">
                <thead>
                  <tr>
                    <th>Total Eligible</th>
                    <th>New to Plan</th>
                    <th>Under 21</th>
                    <th>Already in Plan</th>
                    <th>Wages</th>
                    <th>Hours</th>
                    <th>Points</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>{totalsData?.numberOfEmployees?.toLocaleString() ?? "0"}</td>
                    <td>{totalsData?.numberOfNewEmployees?.toLocaleString() ?? "0"}</td>
                    <td>{totalsData?.numberOfEmployeesUnder21?.toLocaleString() ?? "0"}</td>
                    <td>{totalsData?.numberOfEmployeesInPlan?.toLocaleString() ?? "0"}</td>
                    <td>{numberToCurrency(totalsData?.wagesTotal ?? 0)}</td>
                    <td>{totalsData?.hoursTotal?.toLocaleString() ?? "0"}</td>
                    <td>{totalsData?.pointsTotal?.toLocaleString() ?? "0"}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </Grid>

        <Grid width={"100%"}>
          <Typography
            variant="h6"
            sx={{ mb: 2, px: 3 }}>
            Active and Inactive
          </Typography>
          <div className="px-3">
            <DSMGrid
              preferenceKey={GRID_KEYS.ACTIVE_INACTIVE_SUMMARY}
              isLoading={isFetching}
              handleSortChanged={() => {}}
              providedOptions={{
                rowData: activeAndInactiveRowData,
                pinnedTopRowData: getActiveAndInactiveTotals,
                columnDefs: columnDefs
              }}
            />
          </div>
        </Grid>

        <Grid width={"100%"}>
          <Typography
            variant="h6"
            sx={{ mb: 2, px: 3 }}>
            Terminated
          </Typography>
          <div className="px-3">
            <DSMGrid
              preferenceKey={GRID_KEYS.TERMINATED_SUMMARY}
              isLoading={isFetching}
              handleSortChanged={() => {}}
              providedOptions={{
                rowData: terminatedRowData,
                pinnedTopRowData: getTerminatedTotals,
                columnDefs: columnDefs
              }}
            />
          </div>
        </Grid>

        {employeesRowData && employeesRowData.length > 0 && (
          <Grid width={"100%"}>
            <Typography
              variant="h6"
              sx={{ mb: 2, px: 3 }}>
              Employees
            </Typography>
            <div className="px-3">
              <DSMGrid
                preferenceKey={GRID_KEYS.EMPLOYEES_SUMMARY}
                isLoading={isFetching}
                handleSortChanged={() => {}}
                providedOptions={{
                  rowData: employeesRowData,
                  pinnedTopRowData: getEmployeesTotals,
                  columnDefs: columnDefs
                }}
              />
            </div>
          </Grid>
        )}
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

/**
 * Wrapper component that fetches the frozen state profitYear and renders ProfitSummary.
 * Used by the router when navigating directly to the PAY426 Summary page under Fiscal Close.
 */
interface FrozenProfitSummaryWrapperProps {
  frozenData: boolean;
}

export const FrozenProfitSummaryWrapper: React.FC<FrozenProfitSummaryWrapperProps> = ({ frozenData }) => {
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  // Use Redux-cached frozen state for persistence across navigation
  const cachedFrozenState = useSelector((state: RootState) => state.frozen.frozenStateResponseData);
  const [fetchFrozenState, { isLoading }] = useLazyGetFrozenStateResponseQuery();

  // Only fetch if not already in Redux cache
  useEffect(() => {
    if (hasToken && !cachedFrozenState) {
      fetchFrozenState(undefined, false);
    }
  }, [fetchFrozenState, hasToken, cachedFrozenState]);

  // Use Redux-cached data (persists across navigation)
  if (isLoading || !cachedFrozenState?.profitYear) {
    return (
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        sx={{ minHeight: 200 }}>
        <CircularProgress />
      </Grid>
    );
  }

  return <ProfitSummary frozenData={frozenData} />;
};

export default ProfitSummary;
