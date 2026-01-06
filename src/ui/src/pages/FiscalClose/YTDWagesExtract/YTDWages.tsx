import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useCallback, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import { NavigationCustomSettingsKeys, NavigationDto } from "../../../types/navigation/navigation";
import useYTDWages from "./hooks/useYTDWages";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";

interface YTDWagesProps {
  useFrozenData?: boolean;
}

const YTDWages: React.FC<YTDWagesProps> = ({ useFrozenData = true }) => {
  const componentRef = useRef<HTMLDivElement>(null);
  const dispatch = useDispatch();
  const frozenProfitYear = useFiscalCloseProfitYear();
  
  // Get current navigation to check if we should use frozen year
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const frozenStateResponse = useSelector((state: RootState) => state.frozen.frozenStateResponseData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  
  const currentNavigation = useMemo(() => {
    const getNavigationObjectBasedOnId = (navigationArray?: NavigationDto[], id?: number): NavigationDto | undefined => {
      if (navigationArray) {
        for (const item of navigationArray) {
          if (item.id === id) {
            return item;
          }
          if (item.items && item.items.length > 0) {
            const found = getNavigationObjectBasedOnId(item.items, id);
            if (found) {
              return found;
            }
          }
        }
      }
      return undefined;
    };
    return getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId);
  }, [navigationList, currentNavigationId]);

  // Check if this navigation requires frozen year
  const useFrozenYear = currentNavigation?.customSettings?.[NavigationCustomSettingsKeys.useFrozenYear] === true;
  
  // Use frozen year if required by navigation, otherwise use current calendar year
  // Wait for both navigation data AND frozen state to be loaded if needed
  const profitYear = useMemo(() => {
    // If we don't have navigation data yet, return undefined to prevent premature API calls
    if (!navigationList?.navigation) {
      console.log('[YTDWages] Navigation data not loaded yet');
      return undefined;
    }
    
    console.log('[YTDWages] Navigation ID:', currentNavigationId);
    console.log('[YTDWages] Current navigation:', currentNavigation);
    console.log('[YTDWages] useFrozenYear setting:', useFrozenYear);
    console.log('[YTDWages] Frozen state response:', frozenStateResponse);
    console.log('[YTDWages] Frozen profit year:', frozenProfitYear);
    
    // If this page requires frozen year, wait for frozen state to load
    if (useFrozenYear) {
      // Don't return a year until frozen state is loaded
      if (!frozenStateResponse) {
        console.log('[YTDWages] Waiting for frozen state to load...');
        return undefined;
      }
      console.log('[YTDWages] Using frozen year:', frozenStateResponse.profitYear);
      return frozenStateResponse.profitYear;
    }
    
    // Otherwise use current calendar year immediately
    const currentYear = new Date().getFullYear();
    console.log('[YTDWages] Using calendar year:', currentYear);
    return currentYear;
  }, [navigationList, currentNavigationId, currentNavigation, useFrozenYear, frozenStateResponse, frozenProfitYear]);
  
  const { searchResults, isSearching, pagination, showData, hasResults, executeSearch } = useYTDWages({
    defaultUseFrozenData: useFrozenData,
    profitYear: profitYear // Will be undefined until frozen state loads if needed
  });
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  // Get current drawer state from Redux
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  // Handle status change - refresh grid with archive=true when status changes to Complete
  const handleStatusChange = useCallback(
    (_newStatus: string, statusName?: string) => {
      if (statusName === "Complete" && profitYear) {
        // Refresh the grid with archive=true to archive the results
        executeSearch({ profitYear, useFrozenData, archive: true }, "status-complete");
      }
    },
    [profitYear, useFrozenData, executeSearch]
  );

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  // Handler to toggle grid expansion
  const handleToggleGridExpand = () => {
    setIsGridExpanded((prev) => {
      if (!prev) {
        // Expanding: remember drawer state and close it
        setWasDrawerOpenBeforeExpand(isDrawerOpen || false);
        dispatch(closeDrawer());
        dispatch(setFullscreen(true));
      } else {
        // Collapsing: restore previous drawer state
        dispatch(setFullscreen(false));
        if (wasDrawerOpenBeforeExpand) {
          dispatch(openDrawer());
        }
      }
      return !prev;
    });
  };

  //const recordCount = searchResults?.response?.total || 0;

  return (
    <PageErrorBoundary pageName="YTD Wages Extract">
      <Page
        label={isGridExpanded ? "" : `${CAPTIONS.YTD_WAGES_EXTRACT}`}
        actionNode={isGridExpanded ? undefined : renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          {!isGridExpanded && (
            <Grid width={"100%"}>
              <Divider />
            </Grid>
          )}
          {!isGridExpanded && (
            <Grid
              width={"100%"}
              hidden={true}>
              <DSMAccordion title="Filter">
                <YTDWagesSearchFilter
                  onSearch={executeSearch}
                  isSearching={isSearching}
                  defaultUseFrozenData={useFrozenData}
                />
              </DSMAccordion>
            </Grid>
          )}

          <Grid width="100%">
            <YTDWagesGrid
              innerRef={componentRef}
              data={searchResults}
              isLoading={isSearching}
              showData={showData}
              hasResults={hasResults ?? false}
              pagination={pagination}
              onSortChange={pagination.handleSortChange}
              isGridExpanded={isGridExpanded}
              onToggleExpand={handleToggleGridExpand}
            />
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default YTDWages;
