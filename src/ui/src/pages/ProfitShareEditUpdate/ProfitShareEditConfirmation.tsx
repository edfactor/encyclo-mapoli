import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import { Button, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { ProfitMasterStatus, ProfitShareEditUpdateQueryParams } from "reduxstore/types";

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

    <Grid2
      container
      rowSpacing="18px">
      <Grid2 width={"100%"}>
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
      </Grid2>
      <Grid2 width={"100%"}>
        <Typography
          variant="body2"
          sx={{}}>
          {messageHeadline}
        </Typography>
      </Grid2>
      <Grid2 width={"100%"}>
        <Typography
          component={"span"}
          variant="body2"
          sx={{}}>
          <ul style={{ listStyleType: "none", padding: 0 }}>
            {params && params.contributionPercent != null && params.contributionPercent != 0 && (
              <li
                style={{
                  marginBottom: "6px"
                }}>
                {`Contribution Percent: `}
                <strong>{params.contributionPercent}</strong>
              </li>
            )}
            {params && params.earningsPercent != null && params.earningsPercent != 0 && (
              <li style={{ marginBottom: "6px" }}>
                {`Earnings Percent: `}
                <strong>{params.earningsPercent}</strong>
              </li>
            )}
            {params && params.incomingForfeitPercent != null && params.incomingForfeitPercent != 0 && (
              <li style={{ marginBottom: "6px" }}>
                {`Incoming Forfeit Percent: `}
                <strong>{params.incomingForfeitPercent}</strong>
              </li>
            )}
            {params && params?.secondaryEarningsPercent != null && params?.secondaryEarningsPercent != 0 && (
              <li style={{ marginBottom: "6px" }}>
                {`Secondary Earnings Percent: `}
                <strong>{params?.secondaryEarningsPercent}</strong>
              </li>
            )}
            {params && params?.maxAllowedContributions != null && params?.maxAllowedContributions != 0 && (
              <li style={{ marginBottom: "6px" }}>
                {`Max Allowed Contributions: `}
                <strong>{params?.maxAllowedContributions}</strong>
              </li>
            )}
            {params &&
              ((params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust != null &&
              (params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust != 0 ? (
                <li style={{ marginBottom: "6px" }}>
                  {`Badge To Adjust: `}
                  <strong>{(params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust}</strong>
                </li>
              ) : (params as ProfitMasterStatus)?.badgeAdjusted != null &&
                (params as ProfitMasterStatus)?.badgeAdjusted != 0 ? (
                <li style={{ marginBottom: "6px" }}>
                  {`Badge Adjusted: `}
                  <strong>{(params as ProfitMasterStatus)?.badgeAdjusted}</strong>
                </li>
              ) : null)}
            {params && params?.adjustContributionAmount != null && params?.adjustContributionAmount != 0 && (
              <li style={{ marginBottom: "6px" }}>
                {`Adjust Contribution Amount: `}
                <strong>{params?.adjustContributionAmount}</strong>
              </li>
            )}
            {params && params?.adjustEarningsAmount != null && params?.adjustEarningsAmount != 0 && (
              <li style={{ marginBottom: "6px" }}>
                {`Adjust Earnings Amount: `}
                <strong>{params?.adjustEarningsAmount}</strong>
              </li>
            )}
            {params && params?.adjustIncomingForfeitAmount != null && params?.adjustIncomingForfeitAmount != 0 && (
              <li
                style={{
                  marginBottom: "6px"
                }}>
                {`Adjust Incoming Forfeit Amount: `}
                <strong>{params?.adjustIncomingForfeitAmount}</strong>
              </li>
            )}
            {params &&
              ((params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust2 != null &&
              (params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust2 != 0 ? (
                <li style={{ marginBottom: "6px" }}>
                  {`Second Badge to Adjust: `}
                  <strong>{(params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust2}</strong>
                </li>
              ) : (params as ProfitMasterStatus)?.badgeAdjusted2 != null &&
                (params as ProfitMasterStatus)?.badgeAdjusted2 != 0 ? (
                <li style={{ marginBottom: "6px" }}>
                  {`Second Badge Adjusted: `}
                  <strong>{(params as ProfitMasterStatus)?.badgeAdjusted2}</strong>
                </li>
              ) : null)}
            {params && params?.adjustEarningsSecondaryAmount != null && params?.adjustEarningsSecondaryAmount != 0 && (
              <li
                style={{
                  marginBottom: "6px"
                }}>
                {`Adjust Secondary Earnings Amount: `}
                <strong>{params?.adjustEarningsSecondaryAmount}</strong>
              </li>
            )}
          </ul>
        </Typography>
      </Grid2>
      <Grid2 width={"100%"}>
        <Typography
          variant="body2"
          sx={{}}>
          {lastWarning}
        </Typography>
      </Grid2>
      <Grid2 width={"100%"}>
        {RenderYesButton(performLabel, actionFunction, setOpenModal)}
        {RenderNoButton(closeLabel, setOpenModal)}
      </Grid2>
    </Grid2>
    //</Page>
  );
};

export default ProfitShareEditConfirmation;
