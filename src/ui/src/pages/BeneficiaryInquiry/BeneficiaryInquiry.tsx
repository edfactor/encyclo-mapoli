import { CloseSharp } from "@mui/icons-material";
import { Button, CircularProgress, Divider, Grid, IconButton } from "@mui/material";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import MasterInquiryMemberGrid from "pages/MasterInquiry/MasterInquiryMemberGrid";
import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import {
  useLazyDeleteBeneficiaryQuery,
  useLazyGetBeneficiaryKindQuery,
  useLazyGetBeneficiarytypesQuery
} from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { BeneficiaryDto, BeneficiaryKindDto, BeneficiarySearchFilterResponse, BeneficiaryTypeDto, MasterInquiryRequest } from "reduxstore/types";
import { DSMAccordion, DSMGrid, ISortParams, Page, Pagination } from "smart-ui-library";
import BeneficiaryInquiryGrid from "./BeneficiaryInquiryGrid";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import CreateBeneficiary from "./CreateBeneficiary";
import { MissiveAlertProvider } from "pages/MasterInquiry/MissiveAlertContext";
import { Paged } from "components/DSMGrid/types";
import { CAPTIONS } from "../../constants";
import { BeneficiarySearchFilterColumns } from "./BeneficiarySearchFilterColumns";

interface SelectedMember {
  memberType: number;
  id: number;
  ssn: number;
  badgeNumber: number;
  psnSuffix: number;
}

const BeneficiaryInquiry = () => {
  const { token } = useSelector((state: RootState) => state.security);
  const [triggerGetBeneficiaryKind] = useLazyGetBeneficiaryKindQuery();
  const [triggerGetBeneficiaryType] = useLazyGetBeneficiarytypesQuery();
  const [triggerDeleteBeneficiary] = useLazyDeleteBeneficiaryQuery();
  const [open, setOpen] = useState(false);
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);
  const [badgeNumber, setBadgeNumber] = useState(0);
  const [beneficiaryKind, setBeneficiaryKind] = useState<BeneficiaryKindDto[]>([]);
  const [beneficiaryType, setBeneficiaryType] = useState<BeneficiaryTypeDto[]>([]);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>();
  const [selectedMember, setSelectedMember] = useState<SelectedMember | null>();
  const [noResults, setNoResults] = useState(false);
  const [change, setChange] = useState<number>(0);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [deleteBeneficiaryId, setDeleteBeneficairyId] = useState<number>(0);
  const [deleteInProgress, setDeleteInProgress] = useState<boolean>(false);
  const [beneficiaryDialogTitle, setBeneficiaryDialogTitle] = useState<string>();
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiarySearchFilterResponse>>();
  const [isFetching, setIsFetching] = useState<boolean>(false);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "name",
    isSortDescending: true
  });
  const onBadgeClick = (data: any) => {
    if (data) {
      const member: SelectedMember = {
        badgeNumber: data.badgeNumber,
        id: data.id,
        memberType: data.isEmployee ? 1 : 2,
        ssn: data.ssn,
        psnSuffix: data.psnSuffix
      }
      setSelectedMember(member);
      setChange(change + 1);
    }

  };

  const RefreshBeneficiaryGrid = () => {
    setChange((prev) => prev + 1);
  };

  const columnDefs = useMemo(() => {
    const columns = BeneficiarySearchFilterColumns();
    return columns;
  }, [beneficiarySearchFilterResponse]);

  const deleteBeneficiary = (id: number) => {
    setDeleteBeneficairyId(id);
    setOpenDeleteConfirmationDialog(true);
  };
  const handleDeleteConfirmationDialog = (del: boolean) => {
    if (del) {
      setDeleteInProgress(true);
      triggerDeleteBeneficiary({ id: deleteBeneficiaryId })
        .unwrap()
        .then((res: any) => {
          setChange((prev) => prev + 1);
        })
        .catch((err: any) => {
          console.error(`Something went wrong! Error: ${err.data.title}`);
        })
        .finally(() => {
          setOpenDeleteConfirmationDialog(false);
          setDeleteBeneficairyId(0);
          setDeleteInProgress(false);
        });
    } else {
      setOpenDeleteConfirmationDialog(false);
    }
  };

  const currentBadge = (badgeNumber: number) => {
    setBadgeNumber(badgeNumber);
  };
  const onBeneficiarySaveSuccess = () => {
    setOpen(false);
    setChange((prev) => prev + 1);
  };

  const handleClose = () => {
    setOpen(false);
  };
  const createOrUpdateBeneficiary = (data?: BeneficiaryDto) => {
    setSelectedBeneficiary(data);
    setBeneficiaryDialogTitle(data ? "Edit Beneficiary" : "Add Beneficiary");
    setOpen(true);
  };

  const onSearch = (res: Paged<BeneficiarySearchFilterResponse> | undefined) => {
    setBeneficiarySearchFilterResponse(res);
  }

  const onFetch = (isFetching: boolean) => {
    setIsFetching(isFetching);
  }

  useEffect(() => {
    if (token) {
      triggerGetBeneficiaryKind({})
        .unwrap()
        .then((data) => {
          setBeneficiaryKind(data.beneficiaryKindList ?? []);
        })
        .catch((reason) => {
          console.error(reason);
        });
      triggerGetBeneficiaryType({})
        .unwrap()
        .then((data) => {
          setBeneficiaryType(data.beneficiaryTypeList ?? []);
        })
        .catch((reason) => console.error(reason));
    }
  }, [beneficiaryKind, token]);

  return (
    <MissiveAlertProvider>
      <Page label="BENEFICIARY INQUIRY">
        <>
          <Dialog
            open={open}
            onClose={handleClose}>
            <DialogTitle>{beneficiaryDialogTitle}</DialogTitle>
            <IconButton
              aria-label="close"
              onClick={handleClose}
              sx={(theme) => ({
                position: "absolute",
                right: 8,
                top: 8,
                color: theme.palette.grey[500]
              })}>
              <CloseSharp />
            </IconButton>
            <DialogContent>
              <CreateBeneficiary
                selectedBeneficiary={selectedBeneficiary}
                beneficiaryKind={beneficiaryKind}
                badgeNumber={selectedMember?.badgeNumber ?? 0}
                psnSuffix={selectedMember?.psnSuffix ?? 0}
                onSaveSuccess={onBeneficiarySaveSuccess}></CreateBeneficiary>
            </DialogContent>
          </Dialog>
          <Dialog open={openDeleteConfirmationDialog}>
            <DialogTitle>Confirmation</DialogTitle>
            <DialogContent>
              <p>Are you sure you want to delete ?</p>
            </DialogContent>
            <DialogActions>
              <Button
                autoFocus
                onClick={() => handleDeleteConfirmationDialog(false)}>
                Cancel
              </Button>
              <Button
                color={"error"}
                onClick={() => handleDeleteConfirmationDialog(true)}>
                Delete it! &nbsp;
                {deleteInProgress ? (
                  <CircularProgress
                    size={"15px"}
                    color={"error"}
                  />
                ) : (
                  <></>
                )}
              </Button>
            </DialogActions>
          </Dialog>
        </>
        <Grid
          container
          rowSpacing="24px">
          <Grid
            size={{ xs: 12 }}
            width={"100%"}>
            <Divider />
          </Grid>
          <Grid
            size={{ xs: 12 }}
            width={"100%"}>
            <DSMAccordion title="Filter">
              <BeneficiaryInquirySearchFilter
                setInitialSearchLoaded={setInitialSearchLoaded}
                onSearch={onSearch}
                beneficiaryType={beneficiaryType}
                onFetch={onFetch}
                searchClicked={currentBadge}></BeneficiaryInquirySearchFilter>
            </DSMAccordion>
          </Grid>

          <Grid
            size={{ xs: 12 }}
            width="100%">
            {/* <Button onClick={handleClickOpen}>Add Beneficiary</Button> */}

            {/* <BeneficiaryInquiryGrid initialSearchLoaded={initialSearchLoaded} setInitialSearchLoaded={setInitialSearchLoaded} /> */}
            {beneficiarySearchFilterResponse && beneficiarySearchFilterResponse?.total > 0 && (
              <>
                <DSMGrid
                  preferenceKey={CAPTIONS.BENEFICIARY_SEARCH_FILTER}
                  isLoading={isFetching}
                  providedOptions={{
                    rowData: beneficiarySearchFilterResponse.results,
                    columnDefs: columnDefs,
                    suppressMultiSort: true
                  }}
                />

                <Pagination
                          pageNumber={pageNumber}
                          setPageNumber={(value: number) => {
                            setPageNumber(value - 1);
                            //setInitialSearchLoaded(true);
                          }}
                          pageSize={pageSize}
                          setPageSize={(value: number) => {
                            setPageSize(value);
                            setPageNumber(1);
                            //setInitialSearchLoaded(true);
                          }}
                          recordCount={beneficiarySearchFilterResponse?.total}
                        />
              </>

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

                <BeneficiaryInquiryGrid
                  refresh={RefreshBeneficiaryGrid}
                  count={change}
                  selectedMember={selectedMember}
                  createOrUpdateBeneficiary={createOrUpdateBeneficiary}
                  deleteBeneficiary={deleteBeneficiary}
                />


              </>
            )}
          </Grid>
        </Grid>
      </Page>
    </MissiveAlertProvider>

  );
};

export default BeneficiaryInquiry;
