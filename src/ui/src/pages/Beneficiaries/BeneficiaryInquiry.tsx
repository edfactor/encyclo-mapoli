import { Divider, Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useLazyBeneficiarySearchFilterQuery, useLazyGetBeneficiaryDetailQuery } from "reduxstore/api/BeneficiariesApi";

import { DSMAccordion, Page, Paged } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { BENEFICIARY_INQUIRY_MESSAGES } from "../../components/MissiveAlerts/MissiveMessages";
import { CAPTIONS, ROUTES } from "../../constants";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import { setDistributionHome } from "../../reduxstore/slices/distributionSlice";
import { ServiceErrorResponse } from "../../types/errors/errors";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import IndividualBeneficiaryView from "./IndividualBeneficiaryView";
import MemberResultsGrid from "./MemberResultsGrid";
import { useBeneficiarySearch } from "./hooks/useBeneficiarySearch";

import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiarySearchAPIRequest } from "@/types";

const BeneficiaryInquiryContent = () => {
  const dispatch = useDispatch();
  const [triggerBeneficiaryDetail] = useLazyGetBeneficiaryDetailQuery();
  const [selectedMember, setSelectedMember] = useState<BeneficiaryDetail | null>();
  const [hasSelectedMember, setHasSelectedMember] = useState(false);
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiaryDetail>>();
  const [memberType, setMemberType] = useState<number | undefined>(undefined);
  const [beneficiarySearchFilterRequest, setBeneficiarySearchFilterRequest] = useState<
    BeneficiarySearchAPIRequest | undefined
  >();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();
  const { addAlert, clearAlerts, missiveAlerts } = useMissiveAlerts();

  // Use custom hook for pagination and sort state
  const search = useBeneficiarySearch({ defaultPageSize: 10, defaultSortBy: "FullName" });

  // Set distribution home when component mounts
  useEffect(() => {
    dispatch(setDistributionHome(ROUTES.BENEFICIARY_INQUIRY));
  }, [dispatch]);

  const onBadgeClick = useCallback(
    (data: BeneficiaryDetail) => {
      if (data) {
        const request: BeneficiaryDetailAPIRequest = {
          badgeNumber: data.badgeNumber,
          psnSuffix: data.psnSuffix,
          isSortDescending: search.sortParams.isSortDescending,
          skip: 0,
          sortBy: search.sortParams.sortBy,
          take: search.pageSize
        };
        triggerBeneficiaryDetail(request)
          .unwrap()
          .then((res) => {
            setSelectedMember(res);
            setHasSelectedMember(true);
          });
      }
    },
    [search.sortParams, search.pageSize, triggerBeneficiaryDetail]
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
            onMemberTypeChange={(type) => {
              setMemberType(type);
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
          />
        )}

        {hasSelectedMember && selectedMember && (
          <IndividualBeneficiaryView
            selectedMember={selectedMember}
            memberType={memberType}
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
    <Page label={CAPTIONS.BENEFICIARY_INQUIRY}>
      <MissiveAlertProvider>
        <BeneficiaryInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default BeneficiaryInquiry;
