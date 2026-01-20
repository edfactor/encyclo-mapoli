import { Divider, Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useLazyBeneficiarySearchFilterQuery } from "reduxstore/api/BeneficiariesApi";
import { useFakeTimeAwareYear } from "../../hooks/useFakeTimeAwareDate";
import { InquiryApi } from "../../reduxstore/api/InquiryApi";

import { DSMAccordion, Page, Paged } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { BENEFICIARY_INQUIRY_MESSAGES } from "../../components/MissiveAlerts/MissiveMessages";
import { PageErrorBoundary } from "../../components/PageErrorBoundary";
import { CAPTIONS, ROUTES } from "../../constants";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import { setDistributionHome } from "../../reduxstore/slices/distributionSlice";
import { ServiceErrorResponse } from "../../types/errors/errors";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import IndividualBeneficiaryView from "./IndividualBeneficiaryView";
import MemberResultsGrid from "./MemberResultsGrid";
import { useBeneficiarySearch } from "./hooks/useBeneficiarySearch";

import { BeneficiaryDetail, BeneficiarySearchAPIRequest } from "@/types";
import type { EmployeeDetails } from "../../types/employee/employee";

const BeneficiaryInquiryContent = () => {
  const dispatch = useDispatch();
  const [triggerMemberDetails] = InquiryApi.useLazyGetProfitMasterInquiryMemberQuery();
  const [selectedMember, setSelectedMember] = useState<BeneficiaryDetail | null>();
  const [memberDetails, setMemberDetails] = useState<EmployeeDetails | null>(null);
  const [isFetchingMemberDetails, setIsFetchingMemberDetails] = useState(false);
  const [hasSelectedMember, setHasSelectedMember] = useState(false);
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiaryDetail>>();
  const [memberType, setMemberType] = useState<number | undefined>(undefined);
  const [beneficiarySearchFilterRequest, setBeneficiarySearchFilterRequest] = useState<
    BeneficiarySearchAPIRequest | undefined
  >();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();
  const { addAlert, clearAlerts, missiveAlerts } = useMissiveAlerts();
  const profitYear = useFakeTimeAwareYear();

  // Use custom hook for pagination and sort state
  const search = useBeneficiarySearch({ defaultPageSize: 10, defaultSortBy: "fullName" });

  // Set distribution home when component mounts
  useEffect(() => {
    dispatch(setDistributionHome(ROUTES.BENEFICIARY_INQUIRY));
  }, [dispatch]);

  const onBadgeClick = useCallback(
    (data: BeneficiaryDetail) => {
      if (data) {
        // Calculate memberType: psnSuffix 0 = employee (type 1), else beneficiary (type 2)
        const calculatedMemberType = data.psnSuffix === 0 ? 1 : 2;

        // Fetch member details for MasterInquiryMemberDetails component
        setIsFetchingMemberDetails(true);
        triggerMemberDetails({ memberType: calculatedMemberType, id: data.id, profitYear })
          .unwrap()
          .then((memberDetailsRes) => {
            setSelectedMember(data);
            setMemberDetails(memberDetailsRes);
            setHasSelectedMember(true);
          })
          .catch((error) => {
            console.error("Failed to fetch member details:", error);
            setSelectedMember(data);
            setMemberDetails(null);
            setHasSelectedMember(true);
          })
          .finally(() => {
            setIsFetchingMemberDetails(false);
          });
      }
    },
    [triggerMemberDetails, profitYear]
  );

  const onSearch = useCallback(
    (res: Paged<BeneficiaryDetail> | undefined) => {
      setBeneficiarySearchFilterResponse(res);
      if (res?.total === 0) {
        addAlert(BENEFICIARY_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
      } else if (res?.total === 1) {
        // Only 1 record - auto-select
        onBadgeClick(res.results[0]);
      }
    },
    [onBadgeClick, addAlert]
  );

  useEffect(() => {
    if (beneficiarySearchFilterRequest) {
      clearAlerts();
      const updatedRequest = {
        ...beneficiarySearchFilterRequest,
        isSortDescending: search.sortParams.isSortDescending,
        skip: search.pageNumber * search.pageSize,
        sortBy: search.sortParams.sortBy,
        take: search.pageSize,
        memberType: memberType ?? beneficiarySearchFilterRequest.memberType
      };
      triggerSearch(updatedRequest)
        .unwrap()
        .then((res) => {
          onSearch(res);
        })
        .catch((error) => {
          const serviceError = error as ServiceErrorResponse;
          // Check if it's a 500 error with "Badge number not found" or "SSN not found" title
          if (
            serviceError?.data?.status === 500 &&
            (serviceError?.data?.title === "Badge number not found." || serviceError?.data?.title === "SSN not found.")
          ) {
            addAlert(BENEFICIARY_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
          }
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [
    search.pageNumber,
    search.pageSize,
    search.sortParams,
    beneficiarySearchFilterRequest,
    triggerSearch,
    memberType
  ]);

  const handleReset = useCallback(() => {
    setBeneficiarySearchFilterRequest(undefined);
    setBeneficiarySearchFilterResponse(undefined);
    setSelectedMember(null);
    setMemberDetails(null);
    setHasSelectedMember(false);
    setMemberType(undefined);
    clearAlerts();
    search.reset();
  }, [search, clearAlerts]);

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid
        size={{ xs: 12 }}
        width={"100%"}>
        <Divider />
      </Grid>

      {missiveAlerts.length > 0 && <MissiveAlerts />}

      <Grid
        size={{ xs: 12 }}
        width={"100%"}>
        <DSMAccordion title="Filter">
          <BeneficiaryInquirySearchFilter
            onSearch={(req) => {
              setBeneficiarySearchFilterRequest(req);
              setSelectedMember(null);
              search.reset();
            }}
            onReset={handleReset}
            isSearching={isFetching}
          />
        </DSMAccordion>
      </Grid>

      <Grid
        size={{ xs: 12 }}
        width="100%">
        {beneficiarySearchFilterResponse && beneficiarySearchFilterResponse?.total > 1 && (
          <MemberResultsGrid
            searchResults={beneficiarySearchFilterResponse}
            isLoading={isFetching}
            pageNumber={search.pageNumber}
            pageSize={search.pageSize}
            onRowClick={onBadgeClick}
            onPageNumberChange={(page) => search.handlePaginationChange(page, search.pageSize)}
            onPageSizeChange={(size) => search.handlePaginationChange(0, size)}
            handleSortChange={search.handleSortChange}
          />
        )}

        {hasSelectedMember && selectedMember && (
          <IndividualBeneficiaryView
            selectedMember={selectedMember}
            memberDetails={memberDetails}
            isFetchingMemberDetails={isFetchingMemberDetails}
            profitYear={profitYear}
            onBeneficiarySelect={(beneficiary) => {
              setSelectedMember(beneficiary);
              setHasSelectedMember(true);
            }}
          />
        )}
      </Grid>
    </Grid>
  );
};

const BeneficiaryInquiry = () => {
  return (
    <PageErrorBoundary pageName="Beneficiary Inquiry">
      <Page label={CAPTIONS.BENEFICIARY_INQUIRY}>
        <MissiveAlertProvider>
          <BeneficiaryInquiryContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default BeneficiaryInquiry;
