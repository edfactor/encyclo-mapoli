import { Tooltip, Divider, Button } from "@mui/material";
import { Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import { AddOutlined } from "@mui/icons-material";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { setAdditionalExecutivesChosen } from "reduxstore/slices/yearsEndSlice";

interface RenderAddButtonProps {
  setOpenModal: React.Dispatch<React.SetStateAction<boolean>>;
}

const RenderAddButton = ({ setOpenModal }: RenderAddButtonProps) => {
  const dispatch = useDispatch();
  const { executiveRowsSelected } = useSelector((state: RootState) => state.yearsEnd);

  const addEnabled = executiveRowsSelected && executiveRowsSelected.length > 0;

  const addButton = (
    <Button
      disabled={!addEnabled}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<AddOutlined color={addEnabled ? "primary" : "disabled"} />}
      onClick={async () => {
        // So what we need to do here is to take the array of selected rows
        // and add them to the additional executives and the main grid will
        // pick them up upon re-render
        if (executiveRowsSelected) {
          dispatch(setAdditionalExecutivesChosen(executiveRowsSelected));
          setOpenModal(false);
        }
      }}>
      Add to Main Grid
    </Button>
  );

  if (!addEnabled) {
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
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  setOpenModal: React.Dispatch<React.SetStateAction<boolean>>;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const SearchAndAddExecutive = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  setOpenModal,
  pageNumberReset,
  setPageNumberReset
}: SearchAndAddExecutiveProps) => {
  return (
    <Page
      label="Add New Executive"
      actionNode={<div className="mr-2 flex justify-end gap-24">{RenderAddButton({ setOpenModal })}</div>}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ManageExecutiveHoursAndDollarsSearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              isModal={true}
              setPageNumberReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>
        <Grid width="100%">
          <ManageExecutiveHoursAndDollarsGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            isModal={true}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default SearchAndAddExecutive;
