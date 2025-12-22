import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { Paged } from "smart-ui-library";
import { SortParams } from "../../../hooks/useGridPagination";
import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiaryDto } from "../../../types";

/**
 * Configuration for useBeneficiaryRelationshipData hook
 */
interface UseBeneficiaryRelationshipDataConfig {
  selectedMember: BeneficiaryDetail | null;
  pageNumber: number;
  pageSize: number;
  sortParams: SortParams;
  externalRefreshTrigger?: number;
}

/**
 * Return type for useBeneficiaryRelationshipData hook
 */
export interface UseBeneficiaryRelationshipDataReturn {
  // Data
  beneficiaryList: Paged<BeneficiaryDto> | undefined;
  beneficiaryOfList: Paged<BeneficiaryDto> | undefined;
  isLoading: boolean;

  // Actions
  refresh: () => void;
}

/**
 * useBeneficiaryRelationshipData - Manages fetching of beneficiary relationship data
 *
 * This hook encapsulates the data fetching logic for both the beneficiary list
 * and the "beneficiary of" list. It handles:
 * - Fetching data when member is selected
 * - Re-fetching when pagination/sort changes
 * - Re-fetching when external component triggers refresh (e.g., after save/delete)
 * - Manual refresh via refresh() method
 *
 * The component using this hook should manage validation, delete, and edit logic.
 *
 * @param config - Configuration including selected member, pagination, sort, and refresh triggers
 * @returns Object with beneficiary data, loading state, and refresh method
 *
 * @example
 * const relationships = useBeneficiaryRelationshipData({
 *   selectedMember,
 *   pageNumber,
 *   pageSize,
 *   sortParams,
 *   externalRefreshTrigger: changeCounter // Incremented after save/delete
 * });
 *
 * // In component:
 * const onSaveSuccess = () => {
 *   relationships.refresh(); // Manually refresh if needed
 * };
 */
export const useBeneficiaryRelationshipData = ({
  selectedMember,
  pageNumber,
  pageSize,
  sortParams,
  externalRefreshTrigger
}: UseBeneficiaryRelationshipDataConfig): UseBeneficiaryRelationshipDataReturn => {
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetBeneficiariesQuery();
  const [beneficiaryList, setBeneficiaryList] = useState<Paged<BeneficiaryDto>>();
  const [beneficiaryOfList, setBeneficiaryOfList] = useState<Paged<BeneficiaryDto>>();
  const [manualRefreshTrigger, setManualRefreshTrigger] = useState(0);

  /**
   * Creates a request object for the API call
   * Returns null if required member identifiers are missing
   */
  const createRequest = useCallback((): BeneficiaryDetailAPIRequest | null => {
    if (!selectedMember?.badgeNumber || selectedMember.badgeNumber === null) return null;
    if (!selectedMember?.psnSuffix && selectedMember.psnSuffix !== 0) return null;

    return {
      badgeNumber: selectedMember.badgeNumber,
      psnSuffix: selectedMember.psnSuffix,
      isSortDescending: sortParams.isSortDescending,
      skip: pageNumber * pageSize,
      sortBy: sortParams.sortBy,
      take: pageSize
    };
  }, [selectedMember?.badgeNumber, selectedMember?.psnSuffix, pageNumber, pageSize, sortParams]);

  /**
   * Fetches the relationship data from the API
   */
  const fetchData = useCallback(() => {
    const request = createRequest();
    if (!request || !hasToken) {
      return;
    }

    triggerSearch(request, false)
      .unwrap()
      .then((res) => {
        setBeneficiaryList(res.beneficiaries);
        setBeneficiaryOfList(res.beneficiaryOf);
      })
      .catch((err) => {
        console.error("Failed to fetch beneficiary relationships:", err);
      });
  }, [createRequest, hasToken, triggerSearch]);

  /**
   * Effect: Fetch data when member changes, pagination changes, or refresh is triggered
   */
  useEffect(() => {
    fetchData();
  }, [selectedMember, pageNumber, pageSize, sortParams, externalRefreshTrigger, manualRefreshTrigger, fetchData]);

  /**
   * Manually trigger a refresh of the relationship data
   * Used after operations like save or delete that modify the data
   */
  const refresh = useCallback(() => {
    setManualRefreshTrigger((prev) => prev + 1);
  }, []);

  return {
    beneficiaryList,
    beneficiaryOfList,
    isLoading: isFetching,
    refresh
  };
};
