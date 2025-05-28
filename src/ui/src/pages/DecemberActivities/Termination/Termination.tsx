import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { Page } from "smart-ui-library";

import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useSelector } from "react-redux";

import { CAPTIONS } from "../../../constants";
import TerminationGrid from "./TerminationGrid";
import TerminationSearchFilter from "./TerminationSearchFilter";
import { CalendarResponseDto, TerminationRequest } from "reduxstore/types";
import { RootState } from "reduxstore/store";
import useFiscalCalendarYear from "hooks/useFiscalCalendarYear";

const Termination = () => {
  const navigate = useNavigate();
  const fiscalData = useFiscalCalendarYear();
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<TerminationRequest | null>(null);

  const handleSearch = (params: TerminationRequest) => {
    setSearchParams(params);
    setInitialSearchLoaded(true);
  };

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
          <TerminationSearchFilter
            setInitialSearchLoaded={setInitialSearchLoaded}
            fiscalData={fiscalData}
            onSearch={handleSearch}
          />
        </Grid2>
        <Grid2 width="100%">
          <TerminationGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            searchParams={searchParams}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Termination;
