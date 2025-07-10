import { Button, CircularProgress, Divider } from "@mui/material";
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Grid2 from '@mui/material/Grid2';
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import MasterInquiryMemberGrid from "pages/MasterInquiry/MasterInquiryMemberGrid";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyDeleteBeneficiaryQuery, useLazyGetBeneficiaryKindQuery, useLazyGetBeneficiarytypesQuery } from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { BeneficiaryDto, BeneficiaryKindDto, BeneficiaryTypeDto, MasterInquiryRequest } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import BeneficiaryInquiryGrid from "./BeneficiaryInquiryGrid";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import CreateBeneficiary from "./CreateBeneficiary";


const BeneficiaryInquiry = () => {
  const { token, appUser, username: stateUsername } = useSelector((state: RootState) => state.security);
  const [triggerGetBeneficiaryKind] = useLazyGetBeneficiaryKindQuery();
  const [triggerGetBeneficiaryType] = useLazyGetBeneficiarytypesQuery();
  const [triggerDeleteBeneficiary] = useLazyDeleteBeneficiaryQuery();
  const [open, setOpen] = useState(false);
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);
  const [badgeNumber, setBadgeNumber] = useState(0);
  const [beneficiaryKind, setBeneficiaryKind] = useState<BeneficiaryKindDto[]>([]);
  const [beneficiaryType, setBeneficiaryType] = useState<BeneficiaryTypeDto[]>([]);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [selectedMember, setSelectedMember] = useState<{ memberType: number; id: number, ssn: number, badgeNumber: number, psnSuffix: number } | null>(null);
  const [noResults, setNoResults] = useState(false);
  const [change, setChange] = useState<number>(0);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [deleteBeneficiaryId, setDeleteBeneficairyId] = useState<number>(0);
  const[deleteInProgress,setDeleteInProgress] = useState<boolean>(false);


  const handleClickOpen = () => {
    setOpen(true);
  };
  const onBadgeClick = (data: any) => {
    setSelectedMember(data);
    setChange(change + 1);
  }

  const deleteBeneficiary = (id: number) => {
    setDeleteBeneficairyId(id);
    setOpenDeleteConfirmationDialog(true);
  }
  const handleDeleteConfirmationDialog = (del: boolean) => {
    if (del) {
      setDeleteInProgress(true);
      triggerDeleteBeneficiary({ id: deleteBeneficiaryId }).unwrap().then((res: any) => {
        setChange(prev => prev + 1);
      }).catch((err: any) => {
        console.error(`Something went wrong! Error: ${err.data.title}`)
      }).finally(() => { setOpenDeleteConfirmationDialog(false); setDeleteBeneficairyId(0); setDeleteInProgress(false); });
    }
    else {
      setOpenDeleteConfirmationDialog(false);
    }
  }

  const currentBadge = (badgeNumber: number) => {
    setBadgeNumber(badgeNumber);
  }
  const onBeneficiarySaveSuccess = () => {
    setOpen(false);
    setChange(prev => prev + 1);
  }

  const handleClose = () => {
    setOpen(false);
  };
  const createOrUpdateBeneficiary = (data?: BeneficiaryDto) => {
    setSelectedBeneficiary(data);
    setOpen(true);

  }

  useEffect(() => {
    if (token) {
      triggerGetBeneficiaryKind({}).unwrap().then((data) => {
        setBeneficiaryKind(data.beneficiaryKindList ?? []);
      }).catch((reason) => { console.error(reason); });
      triggerGetBeneficiaryType({}).unwrap().then((data) => {
        setBeneficiaryType(data.beneficiaryTypeList ?? []);
      }).catch((reason) => console.error(reason));
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
            <CreateBeneficiary selectedBeneficiary={selectedBeneficiary} beneficiaryKind={beneficiaryKind} badgeNumber={selectedMember?.badgeNumber ?? 0} psnSuffix={selectedMember?.psnSuffix ?? 0} onSaveSuccess={onBeneficiarySaveSuccess}></CreateBeneficiary>

          </DialogContent>
        </Dialog>
        <Dialog
          open={openDeleteConfirmationDialog}
        >
          <DialogTitle>Confirmation</DialogTitle>
          <DialogContent>
            <p>Are you sure you want to delete ?</p>
          </DialogContent>
          <DialogActions>
            <Button autoFocus onClick={() => handleDeleteConfirmationDialog(false)}>
              Cancel
            </Button>
            <Button color={"error"} onClick={() => handleDeleteConfirmationDialog(true)}>
              Delete it! &nbsp;
              {deleteInProgress? <CircularProgress size={"15px"} color={"error"}  /> :<></>}
              </Button>
          </DialogActions>
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
              }} beneficiaryType={beneficiaryType} searchClicked={currentBadge}></BeneficiaryInquirySearchFilter>
          </DSMAccordion>
        </Grid2>

        <Grid2 size={{ xs: 12 }} width="100%">
          {/* <Button onClick={handleClickOpen}>Add Beneficiary</Button> */}

          {/* <BeneficiaryInquiryGrid initialSearchLoaded={initialSearchLoaded} setInitialSearchLoaded={setInitialSearchLoaded} /> */}
          {searchParams && (
            <MasterInquiryMemberGrid {...searchParams} onBadgeClick={onBadgeClick} />
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
                  onClick={() => createOrUpdateBeneficiary(undefined)}>
                  Add Beneficiary
                </Button>
              </div>

              <BeneficiaryInquiryGrid count={change} selectedMember={selectedMember} createOrUpdateBeneficiary={createOrUpdateBeneficiary} deleteBeneficiary={deleteBeneficiary} />
            </>

          )}

        </Grid2>
      </Grid2>
    </Page>
  );
};

export default BeneficiaryInquiry;
