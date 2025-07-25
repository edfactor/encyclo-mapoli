import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";

const YTDWages: React.FC = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const { employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);

  const componentRef = useRef<HTMLDivElement>(null);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={`YTD Wages Extract (PROF-DOLLAR-EXTRACT) (${employeeWagesForYear?.response && employeeWagesForYear.response.total} records)`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid
          width={"100%"}
          hidden={true}>
          <DSMAccordion title="Filter">
            <YTDWagesSearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              setPageReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <YTDWagesGrid
            innerRef={componentRef}
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default YTDWages;
