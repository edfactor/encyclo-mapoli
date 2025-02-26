import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { Search } from "@mui/icons-material";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MilitaryAndRehireEntryAndModificationEmployeeDetails from "./MilitaryAndRehireEntryAndModificationEmployeeDetails";
import { useCreateMilitaryContributionMutation, useLazyGetMilitaryContributionsQuery } from "reduxstore/api/MilitaryApi";
import MilitaryAndRehireEntryAndModificationSearchFilter from "./MilitaryAndRehireEntryAndModificationSearchFilter";
import MilitaryContributionForm from "./MilitaryContributionForm";
import { MilitaryContribution } from "reduxstore/types";

const MilitaryAndRehireEntryAndModification = () => {
  const [showContributions, setShowContributions] = useState(false);
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.yearsEnd);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();
  const [trigger] = useCreateMilitaryContributionMutation();

  const handleFetchContributions = () => {
    if (masterInquiryEmployeeDetails) {
      fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: 2024,
        pagination: { skip: 0, take: 25 }
      });
      setShowContributions(true);
    }
  };

  useEffect(() => {
    if (masterInquiryEmployeeDetails) {
      handleFetchContributions();
    }
  }, [masterInquiryEmployeeDetails]);

  const handleSubmitForRows = (rows: MilitaryContribution[]) => {
    if (!masterInquiryEmployeeDetails) return;
    rows.forEach((row) => {
      if (row.contributionAmount !== null) {
        trigger({
          badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
          profitYear: 2024,
          contributionAmount: row.contributionAmount
        })
      }
    })
  };

  return (
    <Page label="Military Contributions">
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
              <MilitaryAndRehireEntryAndModificationEmployeeDetails
                details={masterInquiryEmployeeDetails}
              />
            </Grid2>

            {showContributions && (
              <Grid2 xs={6} paddingX="24px">
                <MilitaryContributionForm
                  onSubmit={handleSubmitForRows} onCancel={function (): void {
                    throw new Error("Function not implemented.");
                  }} />
              </Grid2>
            )}
          </>
        )}
      </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireEntryAndModification;