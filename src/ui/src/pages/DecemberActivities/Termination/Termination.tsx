import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { Page, DSMAccordion } from "smart-ui-library";

import { Divider, CircularProgress } from "@mui/material";
import Grid2 from "@mui/material/Grid2";

import { CAPTIONS } from "../../../constants";
import TerminationGrid from "./TerminationGrid";
import TerminationSearchFilter from "./TerminationSearchFilter";
import useFiscalCalendarYear from "hooks/useFiscalCalendarYear";
import { StartAndEndDateRequest } from "../../../reduxstore/types";

const Termination = () => {
  const fiscalData = useFiscalCalendarYear();
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<StartAndEndDateRequest | null>(null);

  const handleSearch = (params: StartAndEndDateRequest) => {
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

  const isCalendarDataLoaded = !!fiscalData?.fiscalBeginDate && !!fiscalData?.fiscalEndDate;

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
         {!isCalendarDataLoaded ? (
          <Grid2 width={"100%"} container justifyContent="center" padding={4}>
            <CircularProgress />
          </Grid2>
        ) : (
          <>
        <Grid2 width="100%">
           <DSMAccordion title="Filter">
          <TerminationSearchFilter
            setInitialSearchLoaded={setInitialSearchLoaded}
            fiscalData={fiscalData}
            onSearch={handleSearch}
          />
          </DSMAccordion>
        </Grid2>
        <Grid2 width="100%">
          <TerminationGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            searchParams={searchParams}
          />
        </Grid2>
                  </>
        )}
      </Grid2>
    </Page>
  );
};

export default Termination;
