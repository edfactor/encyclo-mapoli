import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import { Button, Typography } from "@mui/material";
import { Grid } from "@mui/material";
import { ProfitMasterStatus, ProfitShareEditUpdateQueryParams } from "reduxstore/types";
import ChangesList from "./ChangesList";

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
  params?: ProfitShareEditUpdateQueryParams | ProfitMasterStatus | null;
  lastWarning?: string;
}

const ProfitShareEditConfirmation: React.FC<ProfitShareEditConfirmationProps> = ({
  performLabel,
  closeLabel,
  setOpenModal,
  actionFunction,
  messageType,
  messageHeadline,
  params,
  lastWarning
}) => {
  return (
    // Create a jsx ul element where the list items are the params, unless the value of that param is null or zero

    <Grid
      container
      rowSpacing="18px">
      <Grid width={"100%"}>
        <Typography
          variant="body2"
          sx={{ display: "flex", alignItems: "center" }}>
          {(() => {
            if (messageType === "warning") {
              return (
                <>
                  <span style={{ display: "flex", alignItems: "center", marginRight: "8px", color: "red" }}>
                    <WarningAmberIcon />
                  </span>
                  <strong style={{ color: "red" }}>Warning</strong>
                </>
              );
            } else if (messageType === "confirmation") {
              return <strong style={{ color: "green" }}>Are you sure you want to save?</strong>;
            } else {
              return null;
            }
          })()}
        </Typography>
      </Grid>
      <Grid width={"100%"}>
        <Typography
          variant="body2"
          sx={{}}>
          {messageHeadline}
        </Typography>
      </Grid>
      <Grid width={"100%"}>
        <ChangesList params={params} />
      </Grid>
      <Grid width={"100%"}>
        <Typography
          variant="body2"
          sx={{}}>
          {lastWarning}
        </Typography>
      </Grid>
      <Grid width={"100%"}>
        {RenderYesButton(performLabel, actionFunction, setOpenModal)}
        {RenderNoButton(closeLabel, setOpenModal)}
      </Grid>
    </Grid>
    //</Page>
  );
};

export default ProfitShareEditConfirmation;
