import { Divider, Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import usePayBeNext from "./hooks/usePayBeNext";
import PayBeNextGrid from "./PayBeNextGrid";
import PayBeNextSearchFilter from "./PayBeNextSearchFilter";

/**
 * PayBeNext Report Component
 *
 * Displays the Employee Beneficiaries Control Sheet report with:
 * - Dynamic profit year selection
 * - "Is Also Employee" filter
 * - Paginated grid with master-detail view for profit details
 */
const PayBeNext = () => {
  const {
    // State
    searchResults,
    formData,
    isSearching,
    pagination,
    gridData,

    // Selectors
    showData,
    hasResults,
    totalEndingBalance,

    // Actions
    executeSearch,
    resetSearch
  } = usePayBeNext();

  return (
    <Page label="PAY BE NEXT">
      <Grid
        container
        rowSpacing="24px">
        <Grid
          size={{ xs: 12 }}
          width="100%">
          <Divider />
        </Grid>

        <Grid
          size={{ xs: 12 }}
          width="100%">
          <DSMAccordion title="Filter">
            <PayBeNextSearchFilter
              onSearch={executeSearch}
              onReset={resetSearch}
              isFetching={isSearching}
              initialValues={formData}
            />
          </DSMAccordion>
        </Grid>

        <Grid
          size={{ xs: 12 }}
          width="100%">
          <PayBeNextGrid
            data={searchResults}
            gridData={gridData}
            isLoading={isSearching}
            showData={showData}
            hasResults={hasResults}
            totalEndingBalance={totalEndingBalance}
            profitYear={formData.profitYear}
            pagination={pagination}
            onSortChange={pagination.handleSortChange}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default PayBeNext;
