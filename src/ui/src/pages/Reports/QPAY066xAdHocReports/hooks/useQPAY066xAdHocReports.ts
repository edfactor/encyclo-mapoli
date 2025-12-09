import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useCallback, useMemo, useState } from "react";
import {
  useLazyGetBreakdownByStoreInactiveQuery,
  useLazyGetBreakdownByStoreInactiveWithVestedBalanceQuery,
  useLazyGetBreakdownByStoreQuery,
  useLazyGetBreakdownByStoreRetiredWithBalanceActivityQuery,
  useLazyGetBreakdownByStoreTerminatedBalanceNotVestedQuery,
  useLazyGetBreakdownByStoreTerminatedWithBenAllocationsQuery,
  useLazyGetBreakdownByStoreTotalsQuery
} from "reduxstore/api/YearsEndApi";
import { SortedPaginationRequestDto } from "../../../../types/common/api";
import { BreakdownByStoreEmployee } from "../../../../types/reports/breakdown";
import reports from "../availableQPAY066xReports";

export interface QPAY066xSearchParams {
  reportId: string;
  storeNumber?: number;
  badgeNumber?: number;
  employeeName?: string;
  storeManagement: boolean;
  startDate?: string;
  endDate?: string;
  pagination?: SortedPaginationRequestDto;
}

export interface QPAY066xResults {
  results: BreakdownByStoreEmployee[];
  total: number;
}

export interface QPAY066xTotals {
  totalBeginningBalances: number;
  totalVestedBalance: number;
  totalForfeitures: number;
}

export const useQPAY066xAdHocReports = () => {
  const profitYear = useDecemberFlowProfitYear();
  const [currentReportId, setCurrentReportId] = useState<string | null>(null);

  // Call all query hooks unconditionally (React rules requirement)
  const [fetchQPAY066TA, { isFetching: isFetchingTA }] = useLazyGetBreakdownByStoreQuery();
  const [fetchQPAY066Inactive, { isFetching: isFetchingInactive }] = useLazyGetBreakdownByStoreInactiveQuery();
  const [fetchQPAY066I, { isFetching: isFetchingI }] = useLazyGetBreakdownByStoreInactiveWithVestedBalanceQuery();
  const [fetchQPAY066C, { isFetching: isFetchingC }] = useLazyGetBreakdownByStoreTerminatedBalanceNotVestedQuery();
  const [fetchQPAY066B, { isFetching: isFetchingB }] = useLazyGetBreakdownByStoreTerminatedWithBenAllocationsQuery();
  const [fetchQPAY066W, { isFetching: isFetchingW }] = useLazyGetBreakdownByStoreRetiredWithBalanceActivityQuery();
  const [fetchTotals, { isFetching: isFetchingTotals }] = useLazyGetBreakdownByStoreTotalsQuery();

  // Aggregate loading state
  const isLoading = useMemo(
    () =>
      isFetchingTA ||
      isFetchingInactive ||
      isFetchingI ||
      isFetchingC ||
      isFetchingB ||
      isFetchingW ||
      isFetchingTotals,
    [isFetchingTA, isFetchingInactive, isFetchingI, isFetchingC, isFetchingB, isFetchingW, isFetchingTotals]
  );

  const executeSearch = useCallback(
    async (searchParams: QPAY066xSearchParams) => {
      if (!profitYear) {
        console.error("Profit year is not available");
        return;
      }

      const {
        reportId,
        storeNumber,
        badgeNumber,
        employeeName,
        storeManagement,
        startDate,
        endDate,
        pagination = {
          skip: 0,
          take: 255,
          sortBy: "badgeNumber",
          isSortDescending: false
        }
      } = searchParams;

      setCurrentReportId(reportId);

      // Build base params
      const baseParams = {
        profitYear,
        storeNumber,
        storeManagement,
        badgeNumber,
        employeeName,
        pagination
      };

      try {
        // Route to the correct API based on report ID
        switch (reportId) {
          case "QPAY066TA":
            await fetchQPAY066TA(baseParams);
            break;
          case "QPAY066-INACTIVE":
            await fetchQPAY066Inactive(baseParams);
            break;
          case "QPAY066-I":
            await fetchQPAY066I(baseParams);
            break;
          case "QPAY066C":
            // QPAY066C requires startDate and endDate
            if (startDate && endDate) {
              await fetchQPAY066C({ ...baseParams, startDate, endDate });
            }
            break;
          case "QPAY066B":
            // QPAY066B has optional date range
            await fetchQPAY066B({ ...baseParams, startDate, endDate });
            break;
          case "QPAY066W":
            // QPAY066W has optional date range
            await fetchQPAY066W({ ...baseParams, startDate, endDate });
            break;
          default:
            console.error(`Unknown report ID: ${reportId}`);
            return;
        }

        // Fetch totals only if storeNumber is provided
        if (storeNumber) {
          await fetchTotals(baseParams);
        }
      } catch (error) {
        console.error("Error executing search:", error);
      }
    },
    [
      profitYear,
      fetchQPAY066TA,
      fetchQPAY066Inactive,
      fetchQPAY066I,
      fetchQPAY066C,
      fetchQPAY066B,
      fetchQPAY066W,
      fetchTotals
    ]
  );

  const getReportTitle = useCallback((reportId: string | null): string => {
    if (!reportId) return "N/A";
    const matchingPreset = reports.find((preset) => preset.id === reportId);
    return matchingPreset ? matchingPreset.description.toUpperCase() : "N/A";
  }, []);

  return {
    executeSearch,
    isLoading,
    currentReportId,
    getReportTitle
  };
};
