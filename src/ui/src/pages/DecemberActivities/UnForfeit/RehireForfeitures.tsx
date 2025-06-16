import { Divider, CircularProgress } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import RehireForfeituresSearchFilter from "./RehireForfeituresSearchFilter";
import RehireForfeituresGrid from "./RehireForfeituresGrid";
import { useState, useEffect } from "react";
import { CAPTIONS } from "../../../constants";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const RehireForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [resetPageFlag, setResetPageFlag] = useState(false);
  const [fetchAccountingRange, { data: fiscalCalendarYear, isLoading: isRangeLoading }] = useLazyGetAccountingRangeToCurrent(6);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  // Fetch the fiscal calendar year range on mount
  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  const isCalendarDataLoaded = !!fiscalCalendarYear?.fiscalBeginDate && !!fiscalCalendarYear?.fiscalEndDate;

  const handleSearch = () => {
    setResetPageFlag(prev => !prev);
  };

  return (
    <Page
      label={`${CAPTIONS.REHIRE_FORFEITURES}`}
      actionNode={renderActionNode()}
    >
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
            <Grid2 width={"100%"}>
              <DSMAccordion title="Filter">
                <RehireForfeituresSearchFilter
                  setInitialSearchLoaded={setInitialSearchLoaded}
                  fiscalData={fiscalCalendarYear}
                  onSearch={handleSearch}
                />
              </DSMAccordion>
            </Grid2>

            <Grid2 width="100%">
              <RehireForfeituresGrid
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
                resetPageFlag={resetPageFlag}
              />
            </Grid2>
          </>
        )}
      </Grid2>
    </Page>
  );
};

export default RehireForfeitures;
