import Button from "@mui/material/Button";
import BuggyComponent from "components/BuggyComponent";
import { useState } from "react";
import { useLazyGetDemographicBadgesNotInPayprofitQuery, useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, SectionTitle, SmartModal } from "smart-ui-library";

const LandingPage = () => {
  const [openModal, setOpenModal] = useState<boolean>(false);

  const [trigger] = useLazyGetDemographicBadgesNotInPayprofitQuery();
  return (
    <div style={{ width: "100%", height: "100%" }}>
      <SectionTitle title="Landing Page"></SectionTitle>
      <Button
        variant="contained"
        onClick={() => setOpenModal(true)}>
        Open Modal
      </Button>
      <BuggyComponent />
      <Button
      variant="contained"
      onClick={() => {
        trigger({}, false);
      }}>
        Test Request
      </Button>
      <SmartModal
        open={openModal}
        onClose={() => setOpenModal(false)}>
        <DSMAccordion title="Accordion Component">
          <big>Sample Content</big>
        </DSMAccordion>
      </SmartModal>
    </div>
  );
};

export default LandingPage;
