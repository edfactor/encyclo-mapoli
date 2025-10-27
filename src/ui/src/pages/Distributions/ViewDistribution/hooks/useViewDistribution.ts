import { useCallback, useState } from "react";
import { useDispatch } from "react-redux";
import { DISTRIBUTION_INQUIRY_MESSAGES } from "../../../../components/MissiveAlerts/MissiveMessages";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";
import { useLazyGetProfitMasterInquiryMemberQuery } from "../../../../reduxstore/api/InquiryApi";
import { clearCurrentMember, setCurrentMember } from "../../../../reduxstore/slices/distributionSlice";
import type { EmployeeDetails } from "../../../../types/employee/employee";
import type { ServiceErrorResponse } from "../../../../types/errors/errors";

interface UseViewDistributionReturn {
  isLoading: boolean;
  error: string | null;
  fetchMember: (memberId: string, memberType: string, profitYear: number) => Promise<void>;
  clearMemberData: () => void;
}

const useViewDistribution = (): UseViewDistributionReturn => {
  const dispatch = useDispatch();
  const { addAlert } = useMissiveAlerts();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [triggerMemberQuery] = useLazyGetProfitMasterInquiryMemberQuery();

  // Fetch member data using memberId and memberType from URL parameters
  const fetchMember = useCallback(
    async (memberId: string, memberType: string, profitYear: number) => {
      try {
        setIsLoading(true);
        setError(null);

        const request = {
          id: Number(memberId),
          memberType: Number(memberType),
          profitYear: profitYear
        };

        const response = await triggerMemberQuery(request).unwrap();

        // Check if we got a response
        if (response) {
          dispatch(setCurrentMember(response as EmployeeDetails));
        } else {
          // No member found
          addAlert(DISTRIBUTION_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
          setError("Member not found");
        }
      } catch (err) {
        console.error("Error fetching member:", err);
        const serviceError = err as ServiceErrorResponse;

        // Check for specific error messages
        if (
          serviceError?.data?.status === 500 &&
          (serviceError?.data?.title === "Badge number not found." || serviceError?.data?.title === "SSN not found.")
        ) {
          addAlert(DISTRIBUTION_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
          setError("Member not found");
        } else {
          const errorMessage = serviceError?.data?.detail || "Failed to load member data";
          setError(errorMessage);
        }
      } finally {
        setIsLoading(false);
      }
    },
    [dispatch, triggerMemberQuery, addAlert]
  );

  // Clear member data (but NOT currentDistribution - it's set by DistributionActions)
  const clearMemberData = useCallback(() => {
    dispatch(clearCurrentMember());
    // DO NOT clear currentDistribution here - it's managed by the parent navigation
    setError(null);
  }, [dispatch]);

  return {
    isLoading,
    error,
    fetchMember,
    clearMemberData
  };
};

export default useViewDistribution;
