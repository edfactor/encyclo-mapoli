import { Divider, CircularProgress } from "@mui/material";
import { Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import RehireForfeituresSearchFilter from "./RehireForfeituresSearchFilter";
import RehireForfeituresGrid from "./RehireForfeituresGrid";
import { useState, useEffect } from "react";
import { CAPTIONS } from "../../../constants";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";

const RehireForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [resetPageFlag, setResetPageFlag] = useState(false);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [shouldBlock, setShouldBlock] = useState(false);
  const [previousStatus, setPreviousStatus] = useState<string | null>(null);
  const [shouldArchive, setShouldArchive] = useState(false);
  const [fetchAccountingRange, { data: fiscalCalendarYear, isLoading: isRangeLoading }] =
    useLazyGetAccountingRangeToCurrent(6);
  
  // Get rehireForfeituresQueryParams from Redux to check if search has been performed
  const { rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const renderActionNode = () => {
    console.log('renderActionNode called, previousStatus:', previousStatus);
    return (
      <StatusDropdownActionNode
        onStatusChange={(newStatus: string, statusName?: string) => {
          console.log('*** onStatusChange callback triggered ***', { newStatus, statusName, previousStatus });
          
          // Check if this is a change TO "Complete" 
          // Trigger archive if: 1) status is "Complete" AND 2) it's different from previous status (or no previous status)
          const isChangingToComplete = statusName === "Complete" && previousStatus !== newStatus;
          const hasSearchBeenPerformed = !!rehireForfeituresQueryParams;
          console.log('Checking transition:', { statusName, isComplete: statusName === "Complete", differentFromPrevious: previousStatus !== newStatus, isChangingToComplete, hasSearchBeenPerformed, rehireForfeituresQueryParams });
          
          if (isChangingToComplete) {
            if (hasSearchBeenPerformed) {
              console.log('Setting shouldArchive to true');
              setShouldArchive(true);
            } else {
              console.log('Search required - showing alert');
              alert("Please perform a search first before changing the status to Complete.");
              return;
            }
          }
          
          // Update the previous status to track further changes
          setPreviousStatus(newStatus);
          console.log('Updated previous status to:', newStatus);
        }}
      />
    );
  };

  const handleUnsavedChanges = (hasChanges: boolean) => {
    setHasUnsavedChanges(hasChanges);
    setShouldBlock(hasChanges);
  };

  // Fetch the fiscal calendar year range on mount
  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  // Reset archive flag after it's been used (when data is loaded)
  useEffect(() => {
    if (shouldArchive && initialSearchLoaded) {
      setShouldArchive(false);
    }
  }, [shouldArchive, initialSearchLoaded]);

  useEffect(() => {
    if (!shouldBlock) return;

    const message = "Please save your changes. Do you want to leave without saving?";

    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
      return message;
    };

    const handlePopState = (e: PopStateEvent) => {
      const userConfirmed = window.confirm(message);
      if (!userConfirmed) {
        window.history.pushState(null, "", window.location.href);
        e.preventDefault?.();
      }
    };

    const handleClick = (e: MouseEvent) => {
      const target = e.target as HTMLElement;
      const link = target.closest('a, [role="button"], button') as HTMLElement | null;
      const href = link?.getAttribute("href");

      if (link && href && href !== window.location.pathname && href !== "#") {
        const userConfirmed = window.confirm(message);
        if (!userConfirmed) {
          e.preventDefault();
          e.stopPropagation();
        }
      }
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    window.addEventListener("popstate", handlePopState);
    document.addEventListener("click", handleClick, true);

    window.history.pushState(null, "", window.location.href);

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
      window.removeEventListener("popstate", handlePopState);
      document.removeEventListener("click", handleClick, true);
    };
  }, [shouldBlock]);

  const isCalendarDataLoaded = !!fiscalCalendarYear?.fiscalBeginDate && !!fiscalCalendarYear?.fiscalEndDate;

  const handleSearch = () => {
    setResetPageFlag((prev) => !prev);
  };

  // Auto-trigger search when archive mode is activated
  useEffect(() => {
    if (shouldArchive) {
      console.log('Auto-triggering search due to archive mode, resetPageFlag before:', resetPageFlag);
      handleSearch();
      console.log('handleSearch called, resetPageFlag after should change');
    }
  }, [shouldArchive]);

  return (
    <Page
      label={`${CAPTIONS.REHIRE_FORFEITURES}`}
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
            <Grid width={"100%"}>
              <DSMAccordion title="Filter">
                <RehireForfeituresSearchFilter
                  setInitialSearchLoaded={setInitialSearchLoaded}
                  fiscalData={fiscalCalendarYear}
                  onSearch={handleSearch}
                  hasUnsavedChanges={hasUnsavedChanges}
                />
              </DSMAccordion>
            </Grid>

            <Grid width="100%">
              <RehireForfeituresGrid
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
                resetPageFlag={resetPageFlag}
                onUnsavedChanges={handleUnsavedChanges}
                hasUnsavedChanges={hasUnsavedChanges}
                shouldArchive={shouldArchive}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Page>
  );
};

export default RehireForfeitures;
