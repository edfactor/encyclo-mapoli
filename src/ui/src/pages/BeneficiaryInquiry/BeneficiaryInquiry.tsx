import { CloseSharp } from "@mui/icons-material";
import { Button, CircularProgress, Divider, Grid, IconButton, Typography } from "@mui/material";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import MasterInquiryMemberGrid from "pages/MasterInquiry/MasterInquiryMemberGrid";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import {
  useLazyBeneficiarySearchFilterQuery,
  useLazyDeleteBeneficiaryQuery,
  useLazyGetBeneficiaryDetailQuery,
  useLazyGetBeneficiaryKindQuery,
  useLazyGetBeneficiarytypesQuery
} from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { BeneficiaryDetailRequest, BeneficiaryDetailResponse, BeneficiaryDto, BeneficiaryKindDto, BeneficiarySearchFilterRequest, BeneficiarySearchFilterResponse, BeneficiaryTypeDto, MasterInquiryRequest } from "reduxstore/types";
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
  const[triggerBeneficiaryDetail, {isSuccess}]  = useLazyGetBeneficiaryDetailQuery();
  const [open, setOpen] = useState(false);
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);
  const [badgeNumber, setBadgeNumber] = useState(0);
  const [beneficiaryKind, setBeneficiaryKind] = useState<BeneficiaryKindDto[]>([]);
  const [beneficiaryType, setBeneficiaryType] = useState<BeneficiaryTypeDto[]>([]);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>();
  const [selectedMember, setSelectedMember] = useState<BeneficiaryDetailResponse | null>();
  const [noResults, setNoResults] = useState(false);
  const [change, setChange] = useState<number>(0);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [deleteBeneficiaryId, setDeleteBeneficairyId] = useState<number>(0);
  const [deleteInProgress, setDeleteInProgress] = useState<boolean>(false);
  const [beneficiaryDialogTitle, setBeneficiaryDialogTitle] = useState<string>();
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiarySearchFilterResponse>>();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "name",
    isSortDescending: true
  });
  const [initialSearch, setInitateSearch] = useState<number>(0);
  const [beneficiarySearchFilterRequest, setBeneficiarySearchFilterRequest] = useState<BeneficiarySearchFilterRequest | undefined>();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();
  const onBadgeClick = (data: BeneficiarySearchFilterResponse) => {
    if (data) {
      const request:BeneficiaryDetailRequest = {
        badgeNumber: data.badgeNumber,
        psnSuffix: data.psnSuffix
      }
      triggerBeneficiaryDetail(request).unwrap().then(res=>{
        setSelectedMember(res);
      })
    }

  };




  useEffect(() => {

    if (beneficiarySearchFilterRequest) {
      const updatedRequest = {
        ...beneficiarySearchFilterRequest,
        isSortDescending: _sortParams.isSortDescending,
        skip: pageNumber * pageSize,
        sortBy: _sortParams.sortBy,
        take: pageSize
      };
      triggerSearch(updatedRequest).unwrap().then(res => {
        onSearch(res);
      });
    }
  }, [initialSearch, pageSize, pageNumber, _sortParams]);
  // const BeneficiarySearchFilter = useCallback(() => {
  //   if (beneficiarySearchFilterRequest) {
  //     setBeneficiarySearchFilterRequest(params=>{
  //       return {
  //         ...params,
  //         isSortDescending: _sortParams.isSortDescending,
  //         skip: pageNumber*pageSize,
  //         sortBy: _sortParams.sortBy,
  //         take: pageSize
  //       }
  //     })
  //     triggerSearch(beneficiarySearchFilterRequest).unwrap().then(res => {
  //       onSearch(res);
  //     });
  //   }
  // }, [beneficiarySearchFilterRequest, pageSize,pageNumber,_sortParams])

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
    if(res?.total == 1) //only 1 record
    {
      onBadgeClick(res.results[0]);
    }
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
                onSearch={(req) => { setBeneficiarySearchFilterRequest(req); setInitateSearch(param => param + 1) }}
                beneficiaryType={beneficiaryType}
                searchClicked={currentBadge}></BeneficiaryInquirySearchFilter>
            </DSMAccordion>
          </Grid>

          <Grid
            size={{ xs: 12 }}
            width="100%">
            {beneficiarySearchFilterResponse && beneficiarySearchFilterResponse?.total > 0 && (
              <>
                <DSMGrid
                  preferenceKey={CAPTIONS.BENEFICIARY_SEARCH_FILTER}
                  isLoading={isFetching}
                  providedOptions={{
                    rowData: beneficiarySearchFilterResponse.results,
                    columnDefs: columnDefs,
                    suppressMultiSort: true,
                    onRowClicked: (event) => {
                      if (event.data) {
                        onBadgeClick(event.data); // or pass whatever field you need
                      }
                    }
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
            {(isSuccess && selectedMember) && (
              <>
                <Typography
                  variant="h2"
                  sx={{ color: "#0258A5",paddingTop:10 }}>
                  {`Beneficiary Details`}
                </Typography>
                <Grid container spacing={5}>
                  <Grid size={6}>
                    <p><strong>{selectedMember?.name}</strong></p>
                    <p>{selectedMember?.street}<br />{selectedMember?.city} {selectedMember?.state} {selectedMember?.zip}</p>
                  </Grid>
                  <Grid size={6}>
                    <p><strong>DOB</strong> {selectedMember.dateOfBirth}</p>
                    <p><strong>SSN</strong> {selectedMember.ssn}</p>
                    <p><strong>Balance</strong> {selectedMember.currentBalance}</p>
                    </Grid>
                </Grid>
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
