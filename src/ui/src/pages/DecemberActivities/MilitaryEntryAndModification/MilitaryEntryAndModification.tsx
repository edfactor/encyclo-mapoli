import { Button, Divider, Dialog, DialogContent, DialogTitle } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { useCallback, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MasterInquiryEmployeeDetails from "../../MasterInquiry/MasterInquiryEmployeeDetails";
import {
  useCreateMilitaryContributionMutation,
  useLazyGetMilitaryContributionsQuery
} from "reduxstore/api/MilitaryApi";
import MilitaryAndRehireEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";
import MilitaryContributionForm from "./MilitaryContributionForm";
import { MilitaryContribution } from "reduxstore/types";
import { CAPTIONS } from "../../../constants";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-material.css";

const MilitaryEntryAndModification = () => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { data: contributionsData, isFetching }] = useLazyGetMilitaryContributionsQuery();
  const [trigger] = useCreateMilitaryContributionMutation();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const handleSearch = useCallback(() => {
    if (masterInquiryEmployeeDetails) {
      fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: 2024,
        pagination: { skip: 0, take: 25 }
      });
    }
  }, [masterInquiryEmployeeDetails, fetchContributions]);

  const handleOpenForm = () => {
    setIsDialogOpen(true);
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
  };

  const handleSubmitForRows = (rows: MilitaryContribution[]) => {
    if (!masterInquiryEmployeeDetails) return;

    rows.forEach((row) => {
      if (row.contributionAmount !== null) {
        trigger({
          badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
          profitYear: 2024,
          contributionAmount: row.contributionAmount
        });
      }
    });

    setIsDialogOpen(false);
    handleSearch(); // Refresh the grid data after submission
  };

  // Define grid columns
  const columnDefs = [
    { headerName: "Contribution ID", field: "id", sortable: true, filter: true },
    { headerName: "Badge Number", field: "badgeNumber", sortable: true, filter: true },
    { headerName: "Profit Year", field: "profitYear", sortable: true, filter: true },
    { headerName: "Contribution Amount", field: "contributionAmount", sortable: true, filter: true },
    { headerName: "Date Created", field: "dateCreated", sortable: true, filter: true }
  ];

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryAndRehireEntryAndModificationSearchFilter onSearch={handleSearch} />
          </DSMAccordion>
        </Grid2>

        {masterInquiryEmployeeDetails && (
          <>
            <Grid2 width="100%">
              <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} />
            </Grid2>

            <Grid2 width="100%" paddingX="24px">
              <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '16px' }}>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleOpenForm}
                >
                  Add New Military Contribution
                </Button>
              </div>

              <div className="ag-theme-material" style={{ height: 500, width: '100%' }}>
                <AgGridReact
                  rowData={contributionsData || []}
                  columnDefs={columnDefs}
                  pagination={true}
                  paginationPageSize={10}
                  rowSelection="multiple"
                  enableCellTextSelection={true}
                  suppressRowClickSelection={true}
                />
              </div>
            </Grid2>
          </>
        )}
      </Grid2>

      {/* Military Contribution Form Dialog */}
      <Dialog
        open={isDialogOpen}
        onClose={handleCloseForm}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>Add Military Contribution</DialogTitle>
        <DialogContent>
          <MilitaryContributionForm
            onSubmit={handleSubmitForRows}
            onCancel={handleCloseForm}
          />
        </DialogContent>
      </Dialog>
    </Page>
  );
};

export default MilitaryEntryAndModification;