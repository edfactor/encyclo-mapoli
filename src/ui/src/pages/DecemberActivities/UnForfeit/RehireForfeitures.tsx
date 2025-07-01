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
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [shouldBlock, setShouldBlock] = useState(false);
  const [fetchAccountingRange, { data: fiscalCalendarYear, isLoading: isRangeLoading }] = useLazyGetAccountingRangeToCurrent(6);
  
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const handleUnsavedChanges = (hasChanges: boolean) => {
    setHasUnsavedChanges(hasChanges);
    setShouldBlock(hasChanges);
  };

  // Fetch the fiscal calendar year range on mount
  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

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
      const href = link?.getAttribute('href');
  
      if (link && href && href !== window.location.pathname && href !== '#') {
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
                onUnsavedChanges={handleUnsavedChanges}
                hasUnsavedChanges={hasUnsavedChanges}
              />
            </Grid2>
          </>
        )}
      </Grid2>
    </Page>
  );
};

export default RehireForfeitures;
