import { useMemo } from "react";
import { useGetFakeTimeStatusQuery } from "reduxstore/api/ItOperationsApi";

/**
 * Hook that returns the current date, respecting fake time when active.
 * Falls back to real system time when fake time is not active.
 *
 * @returns Date object representing current time (fake or real)
 *
 * @example
 * ```tsx
 * const currentDate = useFakeTimeAwareDate();
 * const formattedDate = format(currentDate, 'yyyy-MM-dd');
 * ```
 */
export const useFakeTimeAwareDate = (): Date => {
  const { data: fakeTimeStatus } = useGetFakeTimeStatusQuery(undefined, {
    // Poll every 60 seconds to stay in sync with FakeTimeBanner
    pollingInterval: 60000,
    // Skip if no token (handled by API internally)
    skip: false
  });

  return useMemo(() => {
    if (fakeTimeStatus?.isActive && fakeTimeStatus?.currentFakeDateTime) {
      return new Date(fakeTimeStatus.currentFakeDateTime);
    }
    return new Date();
  }, [fakeTimeStatus]);
};

/**
 * Hook that returns the current year, respecting fake time when active.
 * Falls back to real system year when fake time is not active.
 *
 * @returns Current year as a number
 *
 * @example
 * ```tsx
 * const currentYear = useFakeTimeAwareYear();
 * // Use for initializing profit year fields
 * const [profitYear, setProfitYear] = useState(currentYear);
 * ```
 */
export const useFakeTimeAwareYear = (): number => {
  const currentDate = useFakeTimeAwareDate();
  return currentDate.getFullYear();
};

/**
 * Hook that returns fake time status directly.
 * Useful when you need access to the full fake time status object.
 *
 * @returns Fake time status object or undefined
 *
 * @example
 * ```tsx
 * const fakeTimeStatus = useFakeTimeStatus();
 * if (fakeTimeStatus?.isActive) {
 *   console.log('Fake time is active:', fakeTimeStatus.currentFakeDateTime);
 * }
 * ```
 */
export const useFakeTimeStatus = () => {
  const { data: fakeTimeStatus } = useGetFakeTimeStatusQuery(undefined, {
    pollingInterval: 60000,
    skip: false
  });

  return fakeTimeStatus;
};
