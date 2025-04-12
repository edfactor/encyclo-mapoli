import { Button, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Page } from "smart-ui-library";

const RenderYesButton = (buttonLabel: string, actionFunction: () => void, setOpenModal: (isOpen: boolean) => void) => {
  const doButton = (
    <Button
      variant="contained"
      color="primary"
      size="medium"
      onClick={async () => {
        actionFunction();
        setOpenModal(false);
      }}>
      {buttonLabel}
    </Button>
  );

  return doButton;
};

const RenderNoButton = (buttonLabel: string, setOpenModal: (isOpen: boolean) => void) => {
  const cancelButton = (
    <Button
      variant="text"
      color="primary"
      size="medium"
      onClick={async () => {
        setOpenModal(false);
      }}>
      {buttonLabel}
    </Button>
  );

  return cancelButton;
};

interface ProfitShareEditConfirmationProps {
  performLabel: string;
  closeLabel: string;
  setOpenModal: (isOpen: boolean) => void;
  actionFunction: () => void;
  messageType: string;
  messageHeadline?: string;
  messageBody?: string;
  lastWarning?: string;
}

const ProfitShareEditConfirmation: React.FC<ProfitShareEditConfirmationProps> = ({
  performLabel,
  closeLabel,
  setOpenModal,
  actionFunction,
  messageType
}) => {
  return (
    <Page>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Typography
            variant="body2"
            sx={{}}>
            Do you still wish to
          </Typography>
        </Grid2>
        <Grid2 width={"100%"}>
          {RenderYesButton(performLabel, actionFunction, setOpenModal)}
          {RenderNoButton(closeLabel, setOpenModal)}
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareEditConfirmation;
