import AddIcon from "@mui/icons-material/Add";
import DownloadIcon from "@mui/icons-material/Download";
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import { Button, Divider, Grid, Tooltip } from "@mui/material";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../constants";
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";
import { useLazySearchDistributionsQuery } from "../../reduxstore/api/DistributionApi";
import { DistributionSearchFormData } from "../../types";
import DistributionInquiryGrid from "./DistributionInquiryGrid";
import DistributionInquirySearchFilter from "./DistributionInquirySearchFilter";

const DistributionInquiryContent = () => {
  const [searchData, setSearchData] = useState<any>(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [triggerSearch, { data, isFetching }] = useLazySearchDistributionsQuery();
  const isReadOnly = useReadOnlyNavigation();

  const handleSearch = async (formData: DistributionSearchFormData) => {
    const request: any = {
      skip: 0,
      take: 25,
      sortBy: "badgeNumber",
      isSortDescending: false
    };

    // Map form data to API request
    if (formData.ssnOrMemberNumber) {
      const value = formData.ssnOrMemberNumber.trim();
      // Check if it's exactly 9 digits (SSN) or 5-11 digits (badge number)
      if (/^\d{9}$/.test(value)) {
        request.ssn = value;
      } else if (/^\d{5,11}$/.test(value)) {
        request.badgeNumber = parseInt(value, 10);
      }
      // Otherwise, set neither (invalid format)
    }

    if (formData.frequency) {
      request.distributionFrequencyId = formData.frequency;
    }

    // Support both single and multiple payment flags
    if (formData.paymentFlags && formData.paymentFlags.length > 0) {
      request.distributionStatusIds = formData.paymentFlags;
    } else if (formData.paymentFlag) {
      request.distributionStatusId = formData.paymentFlag;
    }

    if (formData.taxCode) {
      request.taxCodeId = formData.taxCode;
    }

    if (formData.minGrossAmount) {
      request.minGrossAmount = parseFloat(formData.minGrossAmount);
    }

    if (formData.maxGrossAmount) {
      request.maxGrossAmount = parseFloat(formData.maxGrossAmount);
    }

    if (formData.minCheckAmount) {
      request.minCheckAmount = parseFloat(formData.minCheckAmount);
    }

    if (formData.maxCheckAmount) {
      request.maxCheckAmount = parseFloat(formData.maxCheckAmount);
    }

    setSearchData(request);
    setHasSearched(true);
    await triggerSearch(request);
  };

  const handleReset = () => {
    setSearchData(null);
    setHasSearched(false);
  };

  const handlePaginationChange = async (pageNumber: number, pageSize: number, sortParams: any) => {
    if (searchData) {
      const request = {
        ...searchData,
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      };
      await triggerSearch(request);
    }
  };

  const handleNewEntry = () => {
    console.log("New Entry clicked");
  };

  const handleExport = () => {
    console.log("Export clicked");
  };

  const handleReport = () => {
    console.log("Report clicked");
  };

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width="100%">
        <Divider />
      </Grid>

      <Grid
        width="100%"
        sx={{ display: "flex", justifyContent: "flex-end", paddingX: "24px", gap: "12px" }}>
        <Tooltip title={isReadOnly ? "You are in read-only mode" : ""}>
          <span>
            <Button
              variant="outlined"
              onClick={handleNewEntry}
              disabled={isReadOnly}
              startIcon={<AddIcon />}>
              NEW ENTRY
            </Button>
          </span>
        </Tooltip>
        <Button
          variant="outlined"
          onClick={handleExport}
          startIcon={<DownloadIcon />}>
          EXPORT
        </Button>
        <Button
          variant="outlined"
          onClick={handleReport}
          startIcon={<PictureAsPdfIcon />}>
          REPORT
        </Button>
      </Grid>

      <Grid width="100%">
        <DSMAccordion title="Filter">
          <DistributionInquirySearchFilter
            onSearch={handleSearch}
            onReset={handleReset}
            isLoading={isFetching}
          />
        </DSMAccordion>
      </Grid>

      {hasSearched && (
        <Grid width="100%">
          <DistributionInquiryGrid
            postReturnData={data?.results ?? null}
            totalRecords={data?.total ?? 0}
            isLoading={isFetching}
            onPaginationChange={handlePaginationChange}
          />
        </Grid>
      )}
    </Grid>
  );
};

const DistributionInquiry = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.DISTRIBUTIONS_INQUIRY}
      actionNode={renderActionNode()}>
      <DistributionInquiryContent />
    </Page>
  );
};

export default DistributionInquiry;
