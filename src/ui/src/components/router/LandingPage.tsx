import Button from "@mui/material/Button";
import { useState } from "react";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, SectionTitle, SmartModal } from "smart-ui-library";

const BuggyComponent = () => {
  const [count, setCount] = useState(0);
  if (count === 5) {
    throw new Error("Simulated error: Counter reached 5!");
  }
  return (
    <div className="rounded border p-4">
      <p className="mb-4">Count: {count}</p>
      <button
        onClick={() => setCount(count + 1)}
        className="rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600">
        Increment to crash
      </button>
    </div>
  );
};

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
      <Button
        variant="contained"
        onClick={() => {
          throw new Error("Good morning to those watching the demo!");
        }}>
        Test Request
      </Button>
      <BuggyComponent />
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
