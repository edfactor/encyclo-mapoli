import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import BeneficiariesGrid from "./BeneficiariesGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";

const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

const Beneficiaries = () => {
  return (
    <Page
      label={CAPTIONS.PAY426_NON_EMPLOYEE}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <BeneficiariesGrid />
        </Grid>
      </Grid>
    </Page>
  );
};

export default Beneficiaries;
