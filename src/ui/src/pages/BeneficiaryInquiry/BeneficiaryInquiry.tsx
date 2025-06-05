import { Button, Divider, Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useEffect, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import BeneficiaryInquiryGrid from "./BeneficiaryInquiryGrid";
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import CreateBeneficiary from "./CreateBeneficiary";


const BeneficiaryInquiry = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [open, setOpen] = useState(false);

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  return (
    <Page label="BENEFICIARY INQUIRY">
      <>
        <Dialog
          open={open}
          onClose={handleClose}
        // slotProps={{
        //   paper: {
        //     component: 'form',
        //     onSubmit: (event: React.FormEvent<HTMLFormElement>) => {
        //       event.preventDefault();
        //       const formData = new FormData(event.currentTarget);
        //       const formJson = Object.fromEntries((formData as any).entries());
        //       const email = formJson.email;
        //       console.log(email);
        //       handleClose();
        //     },
        //   },
        // }}
        >
          <DialogTitle>Add Beneficiary</DialogTitle>
          <DialogContent>
            <CreateBeneficiary></CreateBeneficiary>

          </DialogContent>
        </Dialog>
      </>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <DSMAccordion title="Filter">
            {/* <MasterInquirySearchFilter setInitialSearchLoaded={setInitialSearchLoaded} 
            setMissiveAlerts={setMissiveAlerts}
            /> */}
            <BeneficiaryInquirySearchFilter></BeneficiaryInquirySearchFilter>
          </DSMAccordion>
        </Grid2>

        <Grid2 size={{ xs: 12 }} width="100%">
          {/* <Button onClick={handleClickOpen}>Add Beneficiary</Button> */}
          <div
            style={{
              padding: "0 24px 24px 24px",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center"
            }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Beneficiaries`}
            </Typography>

            <Button
              variant="contained"
              color="primary"
              onClick={handleClickOpen}>
              Add Beneficiary
            </Button>
          </div>
          <BeneficiaryInquiryGrid initialSearchLoaded={initialSearchLoaded} setInitialSearchLoaded={setInitialSearchLoaded} />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default BeneficiaryInquiry;
