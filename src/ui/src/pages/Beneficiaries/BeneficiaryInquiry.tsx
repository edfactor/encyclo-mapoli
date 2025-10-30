import { Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { useLazyBeneficiarySearchFilterQuery, useLazyGetBeneficiaryDetailQuery } from "reduxstore/api/BeneficiariesApi";

import { DSMAccordion, Page, Paged } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../constants";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import IndividualBeneficiaryView from "./IndividualBeneficiaryView";
import MemberResultsGrid from "./MemberResultsGrid";

import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiarySearchAPIRequest } from "@/types";

const BeneficiaryInquiry = () => {
  const [triggerBeneficiaryDetail, { isSuccess }] = useLazyGetBeneficiaryDetailQuery();
  const [selectedMember, setSelectedMember] = useState<BeneficiaryDetail | null>();

  const [sortParams, _setSortParams] = useState<{ sortBy: string; isSortDescending: boolean }>({
    sortBy: "name",
    isSortDescending: false
  });
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiaryDetail>>();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [memberType, setMemberType] = useState<number | undefined>(undefined);

  const [initialSearch, setInitateSearch] = useState<number>(0);
  const [beneficiarySearchFilterRequest, setBeneficiarySearchFilterRequest] = useState<
    BeneficiarySearchAPIRequest | undefined
  >();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();

  const onBadgeClick = (data: BeneficiaryDetail) => {
    if (data) {
      const request: BeneficiaryDetailAPIRequest = {
        badgeNumber: data.badgeNumber,
        psnSuffix: data.psnSuffix,
        isSortDescending: sortParams.isSortDescending,
        skip: 0,
        sortBy: sortParams.sortBy,
        take: pageSize
      };
      triggerBeneficiaryDetail(request)
        .unwrap()
        .then((res) => {
          setSelectedMember(res);
        });
    }
  };

  useEffect(() => {
    if (beneficiarySearchFilterRequest) {
      const updatedRequest = {
        ...beneficiarySearchFilterRequest,
        isSortDescending: sortParams.isSortDescending,
        skip: pageNumber * pageSize,
        sortBy: sortParams.sortBy,
        take: pageSize,
        memberType: memberType ?? beneficiarySearchFilterRequest.memberType
      };
      triggerSearch(updatedRequest)
        .unwrap()
        .then((res) => {
          onSearch(res);
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialSearch, pageSize, pageNumber, sortParams, beneficiarySearchFilterRequest, triggerSearch]);

  const onSearch = (res: Paged<BeneficiaryDetail> | undefined) => {
    setBeneficiarySearchFilterResponse(res);
    if (res?.total == 1) {
      //only 1 record
      onBadgeClick(res.results[0]);
    }
  };

  const handleReset = () => {
    setBeneficiarySearchFilterResponse(undefined);
    setSelectedMember(null);
    setMemberType(undefined);
  };

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
                  setInitateSearch((param) => param + 1);
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
                pageNumber={pageNumber}
                pageSize={pageSize}
                onRowClick={onBadgeClick}
                onPageNumberChange={setPageNumber}
                onPageSizeChange={setPageSize}
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
