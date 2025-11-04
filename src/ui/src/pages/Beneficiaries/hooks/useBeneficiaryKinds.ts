import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBeneficiaryKindQuery } from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { BeneficiaryKindDto } from "reduxstore/types";

/**
 * Return type for useBeneficiaryKinds hook
 */
export interface UseBeneficiaryKindsReturn {
  beneficiaryKinds: BeneficiaryKindDto[];
  isLoading: boolean;
  error: string | null;
}

/**
 * useBeneficiaryKinds - Fetches and caches beneficiary kind lookup data
 *
 * This hook manages fetching the list of beneficiary kinds (e.g., Spouse, Child, Parent).
 * It uses a ref-based flag to ensure the data is only fetched once, preventing
 * unnecessary API calls when the modal is opened/closed multiple times.
 *
 * The data is cached in local component state. For app-wide caching,
 * consider moving this to Redux store as a follow-up optimization.
 *
 * @returns Object with beneficiary kinds array, loading state, and error
 *
 * @example
 * const { beneficiaryKinds, isLoading, error } = useBeneficiaryKinds();
 *
 * if (isLoading) return <Spinner />;
 *
 * return (
 *   <Select>
 *     {beneficiaryKinds.map(kind => (
 *       <MenuItem key={kind.id} value={kind.id}>{kind.name}</MenuItem>
 *     ))}
 *   </Select>
 * );
 */
export const useBeneficiaryKinds = (): UseBeneficiaryKindsReturn => {
  const token = useSelector((state: RootState) => state.security.token);
  const [triggerGetBeneficiaryKind, { isLoading }] = useLazyGetBeneficiaryKindQuery();
  const [beneficiaryKinds, setBeneficiaryKinds] = useState<BeneficiaryKindDto[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Ref to track whether we've already attempted to fetch
  // Prevents multiple fetch attempts if hook re-mounts
  const hasAttempted = useRef(false);

  useEffect(() => {
    // Only fetch if:
    // 1. We have a token (user is authenticated)
    // 2. We haven't already attempted a fetch
    // 3. We don't already have data
    // Note: beneficiaryKinds.length should not be in deps - we check it once at initialization
    if (token && !hasAttempted.current && beneficiaryKinds.length === 0) {
      hasAttempted.current = true;

      triggerGetBeneficiaryKind({})
        .unwrap()
        .then((data) => {
          setBeneficiaryKinds(data.beneficiaryKindList ?? []);
          setError(null);
        })
        .catch((reason) => {
          console.error("Failed to fetch beneficiary kinds:", reason);
          setError("Failed to load beneficiary types");
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, triggerGetBeneficiaryKind]);

  return {
    beneficiaryKinds,
    isLoading,
    error
  };
};
