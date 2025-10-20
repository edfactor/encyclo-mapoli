import { useCallback, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetProfitMasterInquiryMemberDetailsQuery,
  useLazySearchProfitMasterInquiryQuery
} from "../../../reduxstore/api/InquiryApi";
import { useMergeProfitsDetailMutation } from "../../../reduxstore/api/AdjustmentsApi";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { MasterInquiryRequest, MissiveResponse } from "../../../types";
import {
  clearMasterInquiryData,
  clearMasterInquiryDataSecondary,
  setMasterInquiryData,
  setMasterInquiryDataSecondary
} from "../../../reduxstore/slices/inquirySlice";
import { RootState } from "../../../reduxstore/store";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

interface MemberRecord {
  id: number;
  isEmployee: boolean;
  badgeNumber?: number;
}

interface ProfitDetailsResponse {
  results: Array<Record<string, unknown>>;
  total: number;
  totalRecords?: number;
}

export const useAdjustments = () => {
  const [triggerSearch] = useLazySearchProfitMasterInquiryQuery();
  const [triggerProfitDetails] = useLazyGetProfitMasterInquiryMemberDetailsQuery();
  const [mergeProfitsDetail, { isLoading: isMerging }] = useMergeProfitsDetailMutation();
  const reduxDispatch = useDispatch();
  const { masterInquiryMemberDetails, masterInquiryMemberDetailsSecondary } = useSelector(
    (state: RootState) => state.inquiry
  );
  const { clearAlerts, addAlert } = useMissiveAlerts();
  const profitDetailsResponseSource = useRef<ProfitDetailsResponse | null>(null);
  const profitDetailsResponseDestination = useRef<ProfitDetailsResponse | null>(null);

  const fetchProfitDetailsForMember = useCallback(
    async (member: MemberRecord, isSecondary: boolean = false) => {
      try {
        console.log(`Fetching profit details for ${isSecondary ? "secondary" : "primary"} member:`, member);

        const profitDetailsResponse = await triggerProfitDetails({
          id: member.id,
          memberType: member.isEmployee ? 1 : 2
        }).unwrap();

        console.log(
          `Profit details response for ${isSecondary ? "secondary" : "primary"} member:`,
          profitDetailsResponse
        );

        // Handle empty results properly
        const hasResults = profitDetailsResponse?.results && profitDetailsResponse.results.length > 0;

        if (!hasResults) {
          // Don't store anything if there are no results
          if (isSecondary) {
            profitDetailsResponseDestination.current = null;
          } else {
            profitDetailsResponseSource.current = null;
          }
          return null;
        }

        // Limit to 5 records for display (only when we have results)
        const limitedResponse = {
          ...profitDetailsResponse,
          results: profitDetailsResponse.results.slice(0, 5),
          total: Math.min(5, profitDetailsResponse.results.length),
          totalRecords: profitDetailsResponse.results.length
        };

        if (isSecondary) {
          profitDetailsResponseDestination.current = limitedResponse;
        } else {
          profitDetailsResponseSource.current = limitedResponse;
        }

        return limitedResponse;
      } catch (error) {
        console.error(`Error fetching profit details for ${isSecondary ? "secondary" : "primary"} member:`, error);
        addAlert({
          id: Date.now(),
          message: `Failed to fetch profit details`,
          description: `Could not retrieve profit details for ${isSecondary ? "destination" : "source"} employee.`,
          severity: "warning"
        } as MissiveResponse);
        return null;
      }
    },
    [triggerProfitDetails, addAlert]
  );

  const executeSearch = useCallback(
    async (params: number[], profitYear: number) => {
      try {
        clearAlerts();

        const searchParamsSource: MasterInquiryRequest = {
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
          ssn: params[0],
          profitYear: profitYear,
          memberType: 1 // Active employees only
        };

        const searchParamsDestination: MasterInquiryRequest = {
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
          ssn: params[1],
          profitYear: profitYear,
          memberType: 1 // Active employees only
        };

        // Search for source member
        const responseSource = await triggerSearch(searchParamsSource).unwrap();
        if (responseSource?.results && responseSource.results.length > 0) {
          const sourceMember = responseSource.results[0];
          reduxDispatch(setMasterInquiryData(sourceMember));

          // Fetch profit details for source member
          await fetchProfitDetailsForMember(sourceMember, false);
        } else {
          // Clear profit details if no source member found
          profitDetailsResponseSource.current = null;
        }

        if (responseSource.results.length === 0) {
          addSsnDoesNotExistAlert(params[0]);
        }

        // Search for destination member
        const responseDestination = await triggerSearch(searchParamsDestination).unwrap();
        if (responseDestination?.results && responseDestination.results.length > 0) {
          const destinationMember = responseDestination.results[0];
          reduxDispatch(setMasterInquiryDataSecondary(destinationMember));

          // Fetch profit details for destination member
          await fetchProfitDetailsForMember(destinationMember, true);
        } else {
          // Clear profit details if no destination member found
          profitDetailsResponseDestination.current = null;
        }

        if (responseDestination.results.length === 0) {
          addSsnDoesNotExistAlert(params[1]);
        }
      } catch (error) {
        addAlert({
          id: Date.now(),
          message:
            error instanceof Error ? error.message : `Generic retrieval failure ssns: ${params[0]} or ${params[1]}`,
          description:
            error instanceof Error ? error.message : `Generic retrieval failure ssns: ${params[0]} or ${params[1]}`,
          severity: "error"
        } as MissiveResponse);
        console.error("Error during search:", error);
      }
    },
    [triggerSearch, reduxDispatch, addAlert, clearAlerts, fetchProfitDetailsForMember]
  );

  const executeMerge = useCallback(
    async (sourceSSN: string, destinationSSN: string) => {
      try {
        // Validate that we have both source and destination members
        if (!masterInquiryMemberDetails) {
          addAlert({
            id: Date.now(),
            message: "Source member required",
            description: "Please search for a source member before attempting to merge.",
            severity: "warning"
          } as MissiveResponse);
          return false;
        }

        if (!masterInquiryMemberDetailsSecondary) {
          addAlert({
            id: Date.now(),
            message: "Destination member required",
            description: "Please search for a destination member before attempting to merge.",
            severity: "warning"
          } as MissiveResponse);
          return false;
        }

        // Safely convert SSNs to numbers
        const sourceSSNNumber = parseInt(sourceSSN.trim(), 10);
        const destinationSSNNumber = parseInt(destinationSSN.trim(), 10);

        // Validate the conversions
        if (isNaN(sourceSSNNumber) || sourceSSNNumber <= 0) {
          addAlert({
            id: Date.now(),
            message: "Invalid source SSN",
            description: "Source SSN must be a valid number.",
            severity: "error"
          } as MissiveResponse);
          return false;
        }

        if (isNaN(destinationSSNNumber) || destinationSSNNumber <= 0) {
          addAlert({
            id: Date.now(),
            message: "Invalid destination SSN",
            description: "Destination SSN must be a valid number.",
            severity: "error"
          } as MissiveResponse);
          return false;
        }

        // Additional validation: ensure SSNs are different
        if (sourceSSNNumber === destinationSSNNumber) {
          addAlert({
            id: Date.now(),
            message: "Invalid merge operation",
            description: "Source and destination SSNs cannot be the same.",
            severity: "error"
          } as MissiveResponse);
          return false;
        }

        clearAlerts();

        // Call the merge API with converted numbers
        const mergeRequest = {
          sourceSsn: sourceSSNNumber,
          destinationSsn: destinationSSNNumber
        };

        console.log("Executing merge with request:", mergeRequest);

        const response = await mergeProfitsDetail(mergeRequest).unwrap();

        // Show success message
        addAlert({
          id: Date.now(),
          message: "Merge completed successfully",
          description: `Successfully merged profit records from employee ${masterInquiryMemberDetails.badgeNumber} to employee ${masterInquiryMemberDetailsSecondary.badgeNumber}.`,
          severity: "success"
        } as MissiveResponse);

        console.log("Merge response:", response);
        return true;
      } catch (error) {
        console.error("Merge failed:", error);

        const errorMessage = error instanceof Error ? error.message : "Unknown error occurred during merge";

        addAlert({
          id: Date.now(),
          message: "Merge failed",
          description: `Failed to merge profit records: ${errorMessage}`,
          severity: "error"
        } as MissiveResponse);

        return false;
      }
    },
    [mergeProfitsDetail, masterInquiryMemberDetails, masterInquiryMemberDetailsSecondary, addAlert, clearAlerts]
  );

  const addSsnDoesNotExistAlert = useCallback(
    (ssn: number) => {
      addAlert({
        id: Date.now(),
        message: `No member found with SSN: ${ssn}`,
        description: `No member found with SSN: ${ssn}`,
        severity: "warning"
      } as MissiveResponse);
    },
    [addAlert]
  );

  const resetSearch = useCallback(() => {
    reduxDispatch(clearMasterInquiryData());
    reduxDispatch(clearMasterInquiryDataSecondary());
    clearAlerts();
  }, [reduxDispatch, clearAlerts]);

  const canMerge = masterInquiryMemberDetails && masterInquiryMemberDetailsSecondary;

  return {
    isSearching: false,
    isMerging,
    canMerge,
    executeSearch,
    executeMerge,
    resetSearch,
    fetchProfitDetailsForMember,
    profitDetailsResponseSource: profitDetailsResponseSource.current,
    profitDetailsResponseDestination: profitDetailsResponseDestination.current
  };
};

export default useAdjustments;
