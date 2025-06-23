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
import { useLazyGetBeneficiarytypesQuery, useLazyGetBeneficiaryKindQuery } from "reduxstore/api/BeneficiariesApi";
import { BeneficiaryKindDto, BeneficiaryTypeDto, MasterInquiryRequest } from "reduxstore/types";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MasterInquiryMemberGrid from "pages/MasterInquiry/MasterInquiryMemberGrid";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";


const BeneficiaryInquiry = () => {
  const { token, appUser, username: stateUsername } = useSelector((state: RootState) => state.security);
  const [triggerGetBeneficiaryKind] = useLazyGetBeneficiaryKindQuery();
  const[triggerGetBeneficiaryType] = useLazyGetBeneficiarytypesQuery();
  const [open, setOpen] = useState(false);
  const [badgeNumber, setBadgeNumber] = useState(0);
  const [beneficiaryKind, setBeneficiaryKind] = useState<BeneficiaryKindDto[]>([]);
  const [beneficiaryType, setBeneficiaryType] = useState<BeneficiaryTypeDto[]>([]);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [selectedMember, setSelectedMember] = useState<{ memberType: number; id: number, ssn: number } | null>(null);
  const [noResults, setNoResults] = useState(false);

  const handleClickOpen = () => {
    setOpen(true);
  };

  const currentBadge = (badgeNumber: number) => {
    setBadgeNumber(badgeNumber);
  }
  const onBeneficiarySaveSuccess = () => {
    setOpen(false);

  }

  const handleClose = () => {
    setOpen(false);
  };

  useEffect(() => {
    if (token) {
      triggerGetBeneficiaryKind({}).unwrap().then((data) => {
        setBeneficiaryKind(data.beneficiaryKindList ?? []);
      }).catch((reason) => { console.error(reason); });
      triggerGetBeneficiaryType({}).unwrap().then((data)=>{
        setBeneficiaryType(data.beneficiaryTypeList??[]);
      }).catch((reason)=>console.error(reason));
    }


  }, [beneficiaryKind, token])


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
            <CreateBeneficiary beneficiaryKind={beneficiaryKind} badgeNumber={badgeNumber} onSaveSuccess={onBeneficiarySaveSuccess}></CreateBeneficiary>

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
            <BeneficiaryInquirySearchFilter setInitialSearchLoaded={setInitialSearchLoaded}
              onSearch={(params) => {
                setSearchParams(params);
                setSelectedMember(null);
                setNoResults(!params);
              }} beneficiaryKind={beneficiaryKind} searchClicked={currentBadge}></BeneficiaryInquirySearchFilter>
          </DSMAccordion>
        </Grid2>

        <Grid2 size={{ xs: 12 }} width="100%">
          {/* <Button onClick={handleClickOpen}>Add Beneficiary</Button> */}

          {/* <BeneficiaryInquiryGrid initialSearchLoaded={initialSearchLoaded} setInitialSearchLoaded={setInitialSearchLoaded} /> */}
          {searchParams && (
            <MasterInquiryMemberGrid {...searchParams} onBadgeClick={setSelectedMember} />
          )}

          {/* Render employee details if identifiers are present in selectedMember, or show missive if noResults */}
          {(noResults || (selectedMember && selectedMember.memberType !== undefined && selectedMember.id)) && (
            <>
              <MasterInquiryEmployeeDetails
                memberType={selectedMember?.memberType ?? 0}
                id={selectedMember?.id ?? 0}
                profitYear={searchParams?.endProfitYear}
                noResults={noResults}
              />
              <div
                style={{
                  padding: "24px",
                  display: "flex",
                  justifyContent: "right",
                  alignItems: "center"
                }}>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleClickOpen}>
                  Add Beneficiary
                </Button>
              </div>
            </>

          )}

        </Grid2>
      </Grid2>
    </Page>
  );
};

export default BeneficiaryInquiry;
