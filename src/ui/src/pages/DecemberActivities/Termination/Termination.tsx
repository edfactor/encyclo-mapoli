import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { Page } from "smart-ui-library";

import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";

import { CAPTIONS } from "../../../constants";
import TerminationGrid from "./TerminationGrid";

const Termination = () => {
  const navigate = useNavigate();

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  // Set initialSearchLoaded to true when component mounts
  useEffect(() => {
    setInitialSearchLoaded(true);
  }, []);


  return (
    <Page
      label={CAPTIONS.TERMINATIONS}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width="100%">
          <TerminationGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Termination;
