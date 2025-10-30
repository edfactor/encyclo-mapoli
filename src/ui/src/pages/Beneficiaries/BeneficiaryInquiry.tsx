import { Divider, Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useLazyBeneficiarySearchFilterQuery, useLazyGetBeneficiaryDetailQuery } from "reduxstore/api/BeneficiariesApi";

import { DSMAccordion, Page, Paged } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../constants";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import IndividualBeneficiaryView from "./IndividualBeneficiaryView";
import MemberResultsGrid from "./MemberResultsGrid";
import { useBeneficiarySearch } from "./hooks/useBeneficiarySearch";

import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiarySearchAPIRequest } from "@/types";

const BeneficiaryInquiry = () => {
  const [triggerBeneficiaryDetail, { isSuccess }] = useLazyGetBeneficiaryDetailQuery();
  const [selectedMember, setSelectedMember] = useState<BeneficiaryDetail | null>();
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiaryDetail>>();
  const [memberType, setMemberType] = useState<number | undefined>(undefined);
  const [beneficiarySearchFilterRequest, setBeneficiarySearchFilterRequest] = useState<
    BeneficiarySearchAPIRequest | undefined
  >();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();

  // Use custom hook for pagination and sort state
  const search = useBeneficiarySearch({ defaultPageSize: 10, defaultSortBy: "name" });

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
          });
      }
    },
    [search.sortParams, search.pageSize, triggerBeneficiaryDetail]
  );

  const onSearch = useCallback(
    (res: Paged<BeneficiaryDetail> | undefined) => {
      setBeneficiarySearchFilterResponse(res);
      if (res?.total == 1) {
        // Only 1 record - auto-select
        onBadgeClick(res.results[0]);
      }
    },
    [onBadgeClick]
  );

  useEffect(() => {
    if (beneficiarySearchFilterRequest) {
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
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search.pageNumber, search.pageSize, search.sortParams, beneficiarySearchFilterRequest, triggerSearch, memberType]);

  const handleReset = useCallback(() => {
    setBeneficiarySearchFilterResponse(undefined);
    setSelectedMember(null);
    setMemberType(undefined);
    search.reset();
  }, [search]);

  return (
    <MissiveAlertProvider>
      <Page label={CAPTIONS.BENEFICIARY_INQUIRY}>
        <Grid
          container
          rowSpacing="24px">
          <Grid
            size={{ xs: 12 }}
            width={"100%"}>
            <Divider />
          </Grid>
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

            {isSuccess && selectedMember && (
              <IndividualBeneficiaryView
                selectedMember={selectedMember}
                memberType={memberType}
              />
            )}
          </Grid>
        </Grid>
      </Page>
    </MissiveAlertProvider>
  );
};

export default BeneficiaryInquiry;
