import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";

import { CircularProgress, Divider, Grid } from "@mui/material";

import { CAPTIONS } from "../../../constants";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import TerminationGrid from "../../../pages/DecemberActivities/Termination/TerminationGrid";
import TerminationSearchFilter from "../../../pages/DecemberActivities/Termination/TerminationSearchFilter";
import { StartAndEndDateRequest } from "../../../reduxstore/types";

export interface TerminationSearchRequest extends StartAndEndDateRequest {
  forfeitureStatus: string;
  archive?: boolean;
}

const RecentlyTerminated = () => {
  const [fetchAccountingRange, { data: fiscalData, isLoading: isRangeLoading }] = useLazyGetAccountingRangeToCurrent(6);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<TerminationSearchRequest | null>(null);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [resetPageFlag, setResetPageFlag] = useState(false);
  const [shouldBlock, setShouldBlock] = useState(false);
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);
  const [archiveMode, setArchiveMode] = useState(false);
  const [shouldArchive, setShouldArchive] = useState(false);

  const handleSearch = (params: TerminationSearchRequest) => {
    // Add archive parameter if we're in archive mode
    const searchParamsWithArchive = {
      ...params,
      ...(archiveMode && { archive: true })
    };
    setSearchParams(searchParamsWithArchive);
    setInitialSearchLoaded(true);
    setResetPageFlag((prev) => !prev);
  };

  const handleUnsavedChanges = (hasChanges: boolean) => {
    setHasUnsavedChanges(hasChanges);
    setShouldBlock(hasChanges);
  };

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
    const isChangingToComplete = isCompleteLike && currentStatus !== statusName;

    if (isChangingToComplete) {
      setArchiveMode(true);
      setCurrentStatus(statusName || null);
      setShouldArchive(true);

      // If we have existing search params, update them to include archive and trigger
      if (searchParams) {
        const archivedParams = { ...searchParams, archive: true };
        setSearchParams(archivedParams);
        setResetPageFlag((prev) => !prev);
      }
    } else {
      setCurrentStatus(statusName || null);
      // Reset archive mode if status changes away from "Complete"
      if (!isCompleteLike) {
        setArchiveMode(false);
        // If we have existing search params, trigger a new search without archive
        if (searchParams) {
          const { archive, ...paramsWithoutArchive } = searchParams as any;
          setSearchParams(paramsWithoutArchive as any);
          setResetPageFlag((prev) => !prev);
        }
      }
    }
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  // Initialize current status from StatusDropdownActionNode
  useEffect(() => {
    // This effect will be triggered when the StatusDropdownActionNode's status is initially loaded
    // The handleStatusChange will be called with the initial status
  }, []);

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

  // Clear archive flag when the grid confirms handling it
  const handleArchiveHandled = () => setShouldArchive(false);

  return (
    <Page
      label={CAPTIONS.RECENTLY_TERMINATED}
      actionNode={renderActionNode()}>
      <div>
        <ApiMessageAlert commonKey="TerminationSave" />
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
                  fiscalData={fiscalData}
                  shouldArchive={shouldArchive}
                  onArchiveHandled={handleArchiveHandled}
                />
              </Grid>
            </>
          )}
        </Grid>
      </div>
    </Page>
  );
};

export default RecentlyTerminated;
