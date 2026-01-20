import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBreakdownGrandTotalsQuery } from "reduxstore/api/AdhocApi";
import { RootState } from "../reduxstore/store";
import { GrandTotalsByStoreResponseDto, GrandTotalsByStoreRowDto } from "../reduxstore/types";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";

/** Row data interface for breakdown grand totals grids */
export interface BreakdownRowData {
  category: string;
  ste1: number;
  "700": number;
  "701": number;
  "800": number;
  "801": number;
  "802": number;
  "900": number;
  total: number;
}

/** Empty row data for initial state */
const EMPTY_ROW_DATA: BreakdownRowData = {
  category: "",
  ste1: 0,
  "700": 0,
  "701": 0,
  "800": 0,
  "801": 0,
  "802": 0,
  "900": 0,
  total: 0
};

interface UseBreakdownGrandTotalsOptions {
  /** Whether to filter for under 21 participants only */
  under21Participants?: boolean;
  /** Error message to display on failure */
  errorMessage?: string;
}

interface UseBreakdownGrandTotalsResult {
  /** Row data for the grid (excluding grand total) */
  rowData: BreakdownRowData[];
  /** Grand total row for pinned bottom row */
  grandTotal: BreakdownRowData;
  /** Whether data is currently loading */
  isLoading: boolean;
  /** Error message if fetch failed */
  error: string | null;
}

/**
 * Custom hook for fetching and transforming breakdown grand totals data.
 * Handles API calls, data transformation, loading states, and error handling.
 * 
 * @param options Configuration options for the hook
 * @returns Transformed row data, grand total, loading state, and error
 * 
 * @example
 * ```tsx
 * // For all employees
 * const { rowData, grandTotal, isLoading, error } = useBreakdownGrandTotals();
 * 
 * // For under 21 employees only
 * const { rowData, grandTotal, isLoading, error } = useBreakdownGrandTotals({
 *   under21Participants: true,
 *   errorMessage: "Failed to load Under 21 data"
 * });
 * ```
 */
export const useBreakdownGrandTotals = (
  options: UseBreakdownGrandTotalsOptions = {}
): UseBreakdownGrandTotalsResult => {
  const {
    under21Participants = false,
    errorMessage = "Failed to load employee data. Please try again."
  } = options;

  const profitYear = useDecemberFlowProfitYear();
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [rowData, setRowData] = useState<BreakdownRowData[]>([]);
  const [grandTotal, setGrandTotal] = useState<BreakdownRowData>(EMPTY_ROW_DATA);

  const [fetchGrandTotals, { data: apiData }] = useLazyGetBreakdownGrandTotalsQuery();

  /** Transform API response to grid row data */
  const transformApiData = useCallback((data: GrandTotalsByStoreResponseDto) => {
    const findRow = (category: string): GrandTotalsByStoreRowDto | undefined =>
      data.rows.find((row) => row.category === category);

    const mapRowToData = (row: GrandTotalsByStoreRowDto | undefined, category: string): BreakdownRowData => ({
      category,
      ste1: row?.storeOther ?? 0,
      "700": row?.store700 ?? 0,
      "701": row?.store701 ?? 0,
      "800": row?.store800 ?? 0,
      "801": row?.store801 ?? 0,
      "802": row?.store802 ?? 0,
      "900": row?.store900 ?? 0,
      total: row?.rowTotal ?? 0
    });

    const rows: BreakdownRowData[] = [
      mapRowToData(findRow("100% Vested"), "100% Vested"),
      mapRowToData(findRow("Partially Vested"), "Partially Vested"),
      mapRowToData(findRow("Not Vested"), "Not Vested")
    ];

    const total = mapRowToData(findRow("Grand Total"), "Grand Total");

    setRowData(rows);
    setGrandTotal(total);
  }, []);

  // Fetch data when component mounts or dependencies change
  useEffect(() => {
    if (hasToken) {
      setIsLoading(true);
      setError(null);

      fetchGrandTotals({
        profitYear,
        ...(under21Participants && { under21Participants: true })
      })
        .unwrap()
        .then((data) => {
          if (data?.rows) {
            transformApiData(data);
          }
          setIsLoading(false);
        })
        .catch((err) => {
          console.error("Error fetching breakdown grand totals:", err);
          setError(errorMessage);
          setIsLoading(false);
        });
    }
  }, [profitYear, fetchGrandTotals, hasToken, under21Participants, errorMessage, transformApiData]);

  // Handle updates from cached API data
  useEffect(() => {
    if (apiData?.rows) {
      transformApiData(apiData);
      setIsLoading(false);
    }
  }, [apiData, transformApiData]);

  return { rowData, grandTotal, isLoading, error };
};

export default useBreakdownGrandTotals;
