import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { Page, DSMAccordion } from "smart-ui-library";

import { Divider, CircularProgress } from "@mui/material";
import { Grid } from "@mui/material";

import { CAPTIONS } from "../../../constants";
import TerminationGrid from "./TerminationGrid";
import TerminationSearchFilter from "./TerminationSearchFilter";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import { StartAndEndDateRequest } from "../../../reduxstore/types";

export interface TerminationSearchRequest extends StartAndEndDateRequest {
  forfeitureStatus: string;
}

const Termination = () => {
  const [fetchAccountingRange, { data: fiscalData, isLoading: isRangeLoading }] = useLazyGetAccountingRangeToCurrent(6);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<TerminationSearchRequest | null>(null);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [resetPageFlag, setResetPageFlag] = useState(false);
  const [shouldBlock, setShouldBlock] = useState(false);

  const handleSearch = (params: TerminationSearchRequest) => {
    setSearchParams(params);
    setInitialSearchLoaded(true);
    setResetPageFlag((prev) => !prev);
  };

  const handleUnsavedChanges = (hasChanges: boolean) => {
    setHasUnsavedChanges(hasChanges);
    setShouldBlock(hasChanges);
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  // Set initialSearchLoaded to true when component mounts
  useEffect(() => {
    setInitialSearchLoaded(true);
  }, []);

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (shouldBlock) {
        e.preventDefault();
        e.returnValue = "Please save your changes.";
        return "Please save your changes.";
      }
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    return () => window.removeEventListener("beforeunload", handleBeforeUnload);
  }, [shouldBlock]);

  useEffect(() => {
    if (!shouldBlock) return;

    const handlePopState = (event: PopStateEvent) => {
      if (shouldBlock) {
        const userConfirmed = window.confirm("Please save your changes. Do you want to leave without saving?");
        if (!userConfirmed) {
          window.history.pushState(null, "", window.location.href);
          event.preventDefault();
        }
      }
    };

    window.history.pushState(null, "", window.location.href);

    window.addEventListener("popstate", handlePopState);

    return () => {
      window.removeEventListener("popstate", handlePopState);
    };
  }, [shouldBlock]);

  useEffect(() => {
    if (!shouldBlock) return;

    const handleClick = (event: Event) => {
      const target = event.target as HTMLElement;

      const link = target.closest('a, [role="button"], button');

      if (link && shouldBlock) {
        const href = link.getAttribute("href");

        if (href && href !== window.location.pathname && href !== "#") {
          const userConfirmed = window.confirm("Please save your changes. Do you want to leave without saving?");
          if (!userConfirmed) {
            event.preventDefault();
            event.stopPropagation();
            return false;
          }
        }
      }
    };

    document.addEventListener("click", handleClick, true);

    return () => {
      document.removeEventListener("click", handleClick, true);
    };
  }, [shouldBlock]);

  const isCalendarDataLoaded = !!fiscalData?.fiscalBeginDate && !!fiscalData?.fiscalEndDate;

  return (
    <Page
      label={CAPTIONS.TERMINATIONS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        {!isCalendarDataLoaded ? (
          <Grid
            width={"100%"}
            container
            justifyContent="center"
            padding={4}>
            <CircularProgress />
          </Grid>
        ) : (
          <>
            <Grid width="100%">
              <DSMAccordion title="Filter">
                <TerminationSearchFilter
                  setInitialSearchLoaded={setInitialSearchLoaded}
                  fiscalData={fiscalData}
                  onSearch={handleSearch}
                  hasUnsavedChanges={hasUnsavedChanges}
                />
              </DSMAccordion>
            </Grid>
            <Grid width="100%">
              <TerminationGrid
                setInitialSearchLoaded={setInitialSearchLoaded}
                initialSearchLoaded={initialSearchLoaded}
                searchParams={searchParams}
                resetPageFlag={resetPageFlag}
                onUnsavedChanges={handleUnsavedChanges}
                hasUnsavedChanges={hasUnsavedChanges}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Page>
  );
};

export default Termination;
