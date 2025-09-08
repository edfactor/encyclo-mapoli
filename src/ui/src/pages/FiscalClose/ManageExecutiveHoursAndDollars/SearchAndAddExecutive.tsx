import { AddOutlined } from "@mui/icons-material";
import { Button, Divider, Grid, Tooltip } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";

interface RenderAddButtonInternalProps {
  canAddExecutives: boolean;
  onAddToMainGrid: () => void;
}

const RenderAddButton = ({ canAddExecutives, onAddToMainGrid }: RenderAddButtonInternalProps) => {
  const addButton = (
    <Button
      disabled={!canAddExecutives}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<AddOutlined color={canAddExecutives ? "primary" : "disabled"} />}
      onClick={onAddToMainGrid}>
      Add to Main Grid
    </Button>
  );

  if (!canAddExecutives) {
    return (
      <Tooltip
        placement="top"
        title="You must select only one row to add.">
        <span>{addButton}</span>
      </Tooltip>
    );
  } else {
    return addButton;
  }
};

interface SearchAndAddExecutiveProps {
  executeModalSearch: (searchForm: any) => void;
  modalSelectedExecutives: any[];
  addExecutivesToMainGrid: () => void;
  isModalSearching: boolean;
  // Additional props needed for the modal grid
  modalResults: any;
  selectExecutivesInModal: (executives: any[]) => void;
  modalGridPagination: any;
}

const SearchAndAddExecutive = ({ 
  executeModalSearch,
  modalSelectedExecutives,
  addExecutivesToMainGrid,
  isModalSearching,
  modalResults,
  selectExecutivesInModal,
  modalGridPagination
}: SearchAndAddExecutiveProps) => {

  const canAddExecutives = modalSelectedExecutives.length > 0;

  const handleReset = () => {
    // Modal reset is handled by the hook
  };

  return (
    <Page
      label="Add New Executive"
      actionNode={
        <div className="mr-2 flex justify-end gap-24">
          <RenderAddButton
            canAddExecutives={canAddExecutives}
            onAddToMainGrid={addExecutivesToMainGrid}
          />
        </div>
      }>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ManageExecutiveHoursAndDollarsSearchFilter
              onSearch={executeModalSearch}
              onReset={handleReset}
              isSearching={isModalSearching}
              isModal={true}
            />
          </DSMAccordion>
        </Grid>
        <Grid width="100%">
          <ManageExecutiveHoursAndDollarsGrid 
            isModal={true}
            modalResults={modalResults}
            isSearching={isModalSearching}
            selectExecutivesInModal={selectExecutivesInModal}
            modalGridPagination={modalGridPagination}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default SearchAndAddExecutive;
