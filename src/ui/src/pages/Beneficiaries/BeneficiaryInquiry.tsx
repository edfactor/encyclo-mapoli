import { Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import {
  useLazyBeneficiarySearchFilterQuery,
  useLazyDeleteBeneficiaryQuery,
  useLazyGetBeneficiaryDetailQuery
} from "reduxstore/api/BeneficiariesApi";

import { DSMAccordion, Page, Paged } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../constants";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import CreateBeneficiaryDialog from "./CreateBeneficiaryDialog";
import DeleteBeneficiaryDialog from "./DeleteBeneficiaryDialog";
import IndividualBeneficiaryView from "./IndividualBeneficiaryView";
import MemberResultsGrid from "./MemberResultsGrid";

import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiaryDto, BeneficiarySearchAPIRequest } from "@/types";

const BeneficiaryInquiry = () => {
  const [triggerDeleteBeneficiary] = useLazyDeleteBeneficiaryQuery();
  const [triggerBeneficiaryDetail, { isSuccess }] = useLazyGetBeneficiaryDetailQuery();
  const [openCreateDialog, setOpenCreateDialog] = useState(false);
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);

  const [selectedMember, setSelectedMember] = useState<BeneficiaryDetail | null>();

  const [sortParams, _setSortParams] = useState<{ sortBy: string; isSortDescending: boolean }>({
    sortBy: "name",
    isSortDescending: false
  });
  const [change, setChange] = useState<number>(0);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [deleteBeneficiaryId, setDeleteBeneficairyId] = useState<number>(0);
  const [deleteInProgress, setDeleteInProgress] = useState<boolean>(false);
  const [beneficiaryDialogTitle, setBeneficiaryDialogTitle] = useState<string>();
  const [beneficiarySearchFilterResponse, setBeneficiarySearchFilterResponse] = useState<Paged<BeneficiaryDetail>>();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const [initialSearch, setInitateSearch] = useState<number>(0);
  const [beneficiarySearchFilterRequest, setBeneficiarySearchFilterRequest] = useState<
    BeneficiarySearchAPIRequest | undefined
  >();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();
  const onBadgeClick = (data: BeneficiaryDetail) => {
    if (data) {
      const request: BeneficiaryDetailAPIRequest = {
        badgeNumber: data.badgeNumber,
        psnSuffix: data.psnSuffix,
        isSortDescending: sortParams.isSortDescending,
        skip: 0,
        sortBy: sortParams.sortBy,
        take: pageSize
      };
      triggerBeneficiaryDetail(request)
        .unwrap()
        .then((res) => {
          setSelectedMember(res);
        });
    }
  };

  useEffect(() => {
    if (beneficiarySearchFilterRequest) {
      const updatedRequest = {
        ...beneficiarySearchFilterRequest,
        isSortDescending: sortParams.isSortDescending,
        skip: pageNumber * pageSize,
        sortBy: sortParams.sortBy,
        take: pageSize,
        memberType: 2
      };
      triggerSearch(updatedRequest)
        .unwrap()
        .then((res) => {
          onSearch(res);
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialSearch, pageSize, pageNumber, sortParams, beneficiarySearchFilterRequest, triggerSearch]);

  const deleteBeneficiary = (id: number) => {
    setDeleteBeneficairyId(id);
    setOpenDeleteConfirmationDialog(true);
  };
  const handleDeleteConfirmationDialog = (del: boolean) => {
    if (del) {
      setDeleteInProgress(true);
      triggerDeleteBeneficiary({ id: deleteBeneficiaryId })
        .unwrap()
        .then(() => {
          setChange((prev) => prev + 1);
        })
        .catch((err: unknown) => {
          if (err && typeof err === "object" && "data" in err) {
            const errorData = err as { data?: { title?: string } };
            console.error(`Something went wrong! Error: ${errorData.data?.title}`);
          } else {
            console.error("Something went wrong!");
          }
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

  const onBeneficiarySaveSuccess = () => {
    setOpenCreateDialog(false);
    setChange((prev) => prev + 1);
  };

  const handleClose = () => {
    setOpenCreateDialog(false);
  };
  const createOrUpdateBeneficiary = (data?: BeneficiaryDto) => {
    setSelectedBeneficiary(data);
    setBeneficiaryDialogTitle(data ? "Edit Beneficiary" : "Add Beneficiary");
    setOpenCreateDialog(true);
  };

  const onSearch = (res: Paged<BeneficiaryDetail> | undefined) => {
    setBeneficiarySearchFilterResponse(res);
    if (res?.total == 1) {
      //only 1 record
      onBadgeClick(res.results[0]);
    }
  };

  return (
    <MissiveAlertProvider>
      <Page label={CAPTIONS.BENEFICIARY_INQUIRY}>
        <>
          <CreateBeneficiaryDialog
            open={openCreateDialog}
            onClose={handleClose}
            title={beneficiaryDialogTitle ?? ""}
            selectedBeneficiary={selectedBeneficiary}
            badgeNumber={selectedMember?.badgeNumber ?? 0}
            psnSuffix={selectedMember?.psnSuffix ?? 0}
            onSaveSuccess={onBeneficiarySaveSuccess}
          />
          <DeleteBeneficiaryDialog
            open={openDeleteConfirmationDialog}
            onConfirm={() => handleDeleteConfirmationDialog(true)}
            onCancel={() => handleDeleteConfirmationDialog(false)}
            isDeleting={deleteInProgress}
          />
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
                onSearch={(req) => {
                  setBeneficiarySearchFilterRequest(req);
                  setInitateSearch((param) => param + 1);
                }}
              />
            </DSMAccordion>
          </Grid>

          <Grid
            size={{ xs: 12 }}
            width="100%">
            {beneficiarySearchFilterResponse && beneficiarySearchFilterResponse?.total > 1 && (
              <MemberResultsGrid
                searchResults={beneficiarySearchFilterResponse}
                isLoading={isFetching}
                pageNumber={pageNumber}
                pageSize={pageSize}
                onRowClick={onBadgeClick}
                onPageNumberChange={setPageNumber}
                onPageSizeChange={setPageSize}
              />
            )}

            {isSuccess && selectedMember && (
              <IndividualBeneficiaryView
                selectedMember={selectedMember}
                change={change}
                onAddBeneficiary={() => createOrUpdateBeneficiary(undefined)}
                onEditBeneficiary={createOrUpdateBeneficiary}
                onDeleteBeneficiary={deleteBeneficiary}
              />
            )}
          </Grid>
        </Grid>
      </Page>
    </MissiveAlertProvider>
  );
};

export default BeneficiaryInquiry;
