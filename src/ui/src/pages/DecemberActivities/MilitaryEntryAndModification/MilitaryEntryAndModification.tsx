import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MilitaryAndRehireEntryAndModificationEmployeeDetails from "./MilitaryEntryAndModificationEmployeeDetails";
import {
  useCreateMilitaryContributionMutation,
  useLazyGetMilitaryContributionsQuery
} from "reduxstore/api/MilitaryApi";
import MilitaryAndRehireEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";
import MilitaryContributionForm from "./MilitaryContributionForm";
import { MilitaryContribution } from "reduxstore/types";
import { CAPTIONS, MENU_LABELS } from "../../../constants";
import StatusDropdown from "components/StatusDropdown";
import { useNavigate } from "react-router";

const MilitaryEntryAndModification = () => {
  const [showContributions, setShowContributions] = useState(false);
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();
  const [trigger] = useCreateMilitaryContributionMutation();
  const navigate = useNavigate();

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <StatusDropdown onStatusChange={() => {}} />       
      </div>
    );
  };

  const handleFetchContributions = useCallback(() => {
    if (masterInquiryEmployeeDetails) {
      fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: 2024,
        pagination: { skip: 0, take: 25 }
      });
      setShowContributions(true);
    }
  }, [masterInquiryEmployeeDetails, fetchContributions]);

  useEffect(() => {
    if (masterInquiryEmployeeDetails) {
      handleFetchContributions();
    }
  }, [handleFetchContributions, masterInquiryEmployeeDetails]);

  const handleSubmitForRows = (rows: MilitaryContribution[]) => {
    if (!masterInquiryEmployeeDetails) return;
    rows.forEach((row) => {
      if (row.contributionAmount !== null) {
        trigger({
          badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
          profitYear: 2024,
          contributionAmount: row.contributionAmount
        });
      }
    });
  };

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryAndRehireEntryAndModificationSearchFilter />
          </DSMAccordion>
        </Grid2>

        {masterInquiryEmployeeDetails && (
          <>
            <Grid2 width="100%">
              <MilitaryAndRehireEntryAndModificationEmployeeDetails details={masterInquiryEmployeeDetails} />
            </Grid2>

            {showContributions && (
              <Grid2
                size={{ xs: 6 }}
                paddingX="24px">
                <MilitaryContributionForm
                  onSubmit={handleSubmitForRows}
                  onCancel={function (): void {
                    throw new Error("Function not implemented.");
                  }}
                />
              </Grid2>
            )}
          </>
        )}
      </Grid2>
    </Page>
  );
};

export default MilitaryEntryAndModification;
