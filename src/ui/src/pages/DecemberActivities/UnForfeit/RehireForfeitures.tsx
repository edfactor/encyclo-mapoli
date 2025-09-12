import { CircularProgress, Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import RehireForfeituresGrid from "./RehireForfeituresGrid";
import RehireForfeituresSearchFilter from "./RehireForfeituresSearchFilter";
// removed Redux selector dependency for search state

const RehireForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [resetPageFlag, setResetPageFlag] = useState(false);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [shouldBlock, setShouldBlock] = useState(false);
  const [previousStatus, setPreviousStatus] = useState<string | null>(null);
  const [shouldArchive, setShouldArchive] = useState(false);
  const [fetchAccountingRange, { data: fiscalCalendarYear, isLoading: isRangeLoading }] =
    useLazyGetAccountingRangeToCurrent(6);

  // removed: search gating on status change

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode
        onStatusChange={(newStatus: string, statusName?: string) => {
          // Check if this is a change TO a "Complete"-like status (e.g., Complete/Completed), case-insensitive
          const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
          const isChangingToComplete = isCompleteLike && previousStatus !== newStatus;

          // Always trigger archive when changing to Complete (no search prerequisite)
          if (isChangingToComplete) {
            setShouldArchive(true);
          }

          // Update the previous status to track further changes
          setPreviousStatus(newStatus);
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

  // Clear archive flag when the grid confirms handling it
  const handleArchiveHandled = () => setShouldArchive(false);

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
      handleSearch();
    }
  }, [shouldArchive]);

  return (
    <Page
      label={`${CAPTIONS.REHIRE_FORFEITURES}`}
      actionNode={renderActionNode()}>
      <div>
        <ApiMessageAlert commonKey="UnforfeitSave" />
      </div>
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
                  setHasUnsavedChanges={setHasUnsavedChanges}
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
                onArchiveHandled={handleArchiveHandled}
                setHasUnsavedChanges={setHasUnsavedChanges}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Page>
  );
};

export default RehireForfeitures;
