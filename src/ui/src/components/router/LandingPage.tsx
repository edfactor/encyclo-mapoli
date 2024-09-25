import Button from "@mui/material/Button";
import { useState } from "react";
import { DSMAccordion, SectionTitle, SmartModal } from "smart-ui-library";

const LandingPage = () => {
  const [openModal, setOpenModal] = useState<boolean>(false);
  return (
    <div style={{ width: "100%", height: "100%" }}>
      <SectionTitle title="Landing Page"></SectionTitle>
      <Button
        variant="contained"
        onClick={() => setOpenModal(true)}>
        Open Modal
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
