import { Alert, Snackbar, TextField, Typography } from "@mui/material";
import { FocusEvent, useCallback, useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useDeleteBeneficiaryMutation } from "reduxstore/api/BeneficiariesApi";
import { setDistributionHome } from "reduxstore/slices/distributionSlice";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GRID_KEYS, ROUTES } from "../../constants";
import { SortParams, useGridPagination } from "../../hooks/useGridPagination";
import { BeneficiaryDetail, BeneficiaryDto } from "../../types";
import { GetBeneficiariesListGridColumns } from "./BeneficiariesListGridColumns";
import { BeneficiaryActionHandlers } from "./BeneficiaryActions";
import { GetBeneficiaryOfGridColumns } from "./BeneficiaryOfGridColumns";
import DeleteBeneficiaryDialog from "./DeleteBeneficiaryDialog";
import { useBeneficiaryPercentageUpdate } from "./hooks/useBeneficiaryPercentageUpdate";
import { useBeneficiaryRelationshipData } from "./hooks/useBeneficiaryRelationshipData";

interface BeneficiaryRelationshipsProps {
  selectedMember: BeneficiaryDetail | null;
  count: number;
  onEditBeneficiary: (selectedMember: BeneficiaryDto | undefined) => void;
  onBeneficiariesChange?: (beneficiaries: BeneficiaryDto[]) => void;
}

const BeneficiaryRelationshipsGrids: React.FC<BeneficiaryRelationshipsProps> = ({
  selectedMember,
  count,
  onEditBeneficiary,
  onBeneficiariesChange
}) => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);
  const [deleteBeneficiaryId, setDeleteBeneficiaryId] = useState<number>(0);
  const [deleteInProgress, setDeleteInProgress] = useState<boolean>(false);
  const [triggerDeleteBeneficiary] = useDeleteBeneficiaryMutation();

  // Snackbar state for percentage update feedback
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] = useState<"success" | "error" | "warning">("success");

  // Use custom hooks for data fetching and percentage update
  const { pageNumber, pageSize, sortParams, handlePageNumberChange, handlePageSizeChange, handleSortChange } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "psnSuffix",
      initialSortDescending: true,
      persistenceKey: GRID_KEYS.BENEFICIARIES_LIST
    });

  const relationships = useBeneficiaryRelationshipData({
    selectedMember,
    pageNumber,
    pageSize,
    sortParams,
    externalRefreshTrigger: count
  });

  const percentageUpdate = useBeneficiaryPercentageUpdate(() => {
    relationships.refresh();
  });

  // Notify parent component when beneficiaries change
  useEffect(() => {
    if (onBeneficiariesChange && relationships.beneficiaryList?.results) {
      onBeneficiariesChange(relationships.beneficiaryList.results);
    }
  }, [relationships.beneficiaryList?.results, onBeneficiariesChange]);

  const sortEventHandler = (update: SortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "psnSuffix";
      update.isSortDescending = true;
    }
    handleSortChange(update);
  };

  const deleteBeneficiary = useCallback((id: number) => {
    setDeleteBeneficiaryId(id);
    setOpenDeleteConfirmationDialog(true);
  }, []);

  const handleDeleteConfirmationDialog = (del: boolean) => {
    if (del) {
      setDeleteInProgress(true);
      triggerDeleteBeneficiary({ id: deleteBeneficiaryId })
        .unwrap()
        .then(() => {
          relationships.refresh();
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
          setDeleteBeneficiaryId(0);
          setDeleteInProgress(false);
        });
    } else {
      setOpenDeleteConfirmationDialog(false);
    }
  };

  // Handler functions for Action column
  const handleNewBeneficiaryDistribution = useCallback(
    (beneficiary: BeneficiaryDto) => {
      const psnNumber = String(beneficiary.badgeNumber) + String(beneficiary.psnSuffix);
      const memberType = 2; // Hardcoded memberType for beneficiaries

      // Set distribution home to beneficiary inquiry page
      dispatch(setDistributionHome(ROUTES.BENEFICIARY_INQUIRY));

      // Navigate to add distribution page
      navigate(`/${ROUTES.ADD_DISTRIBUTION}/${psnNumber}/${memberType}`);
    },
    [dispatch, navigate]
  );

  const handleEditBeneficiary = useCallback(
    (beneficiary: BeneficiaryDto) => {
      onEditBeneficiary(beneficiary);
    },
    [onEditBeneficiary]
  );

  const handleDeleteBeneficiary = useCallback(
    (beneficiary: BeneficiaryDto) => {
      deleteBeneficiary(beneficiary.id);
    },
    [deleteBeneficiary]
  );

  const validatePercentageOfBeneficiaries = useCallback(
    async (e: FocusEvent<HTMLInputElement | HTMLTextAreaElement>, id: number) => {
      const currentValue = e.target.value ? parseInt(e.target.value) : 0;

      if (!relationships.beneficiaryList?.results) {
        return;
      }

      const result = await percentageUpdate.validateAndUpdate(id, currentValue, relationships.beneficiaryList.results);

      if (!result.success) {
        // Show error message
        setSnackbarMessage(result.error || "Failed to update percentage");
        setSnackbarSeverity("error");
        setSnackbarOpen(true);

        // Restore previous value on validation failure
        if (e.target.value) {
          e.target.value = result.previousValue?.toString() || "";
        }
      } else {
        // Show success message (or warning if sum doesn't equal 100%)
        if (result.warning) {
          setSnackbarMessage(result.warning);
          setSnackbarSeverity("warning");
        } else {
          setSnackbarMessage("Percentage updated successfully");
          setSnackbarSeverity("success");
        }
        setSnackbarOpen(true);
      }
    },
    [relationships.beneficiaryList, percentageUpdate]
  );

  const percentageFieldRenderer = useCallback(
    (percentage: number, id: number) => {
      return (
        <>
          <TextField
            type="number"
            defaultValue={percentage}
            onBlur={(e) => validatePercentageOfBeneficiaries(e, id)}
            inputProps={{
              min: 0,
              max: 100,
              step: 1
            }}
            size="small"
          />
        </>
      );
    },
    [validatePercentageOfBeneficiaries]
  );

  const beneficiaryOfColumnDefs = useMemo(() => GetBeneficiaryOfGridColumns(), []);

  const actionHandlers: BeneficiaryActionHandlers = useMemo(
    () => ({
      onNewDistribution: handleNewBeneficiaryDistribution,
      onEdit: handleEditBeneficiary,
      onDelete: handleDeleteBeneficiary
    }),
    [handleNewBeneficiaryDistribution, handleEditBeneficiary, handleDeleteBeneficiary]
  );

  const columnDefs = useMemo(() => {
    const columns = GetBeneficiariesListGridColumns(percentageFieldRenderer, actionHandlers);
    return [...columns];
  }, [percentageFieldRenderer, actionHandlers]);

  return (
    <>
      <DeleteBeneficiaryDialog
        open={openDeleteConfirmationDialog}
        onConfirm={() => handleDeleteConfirmationDialog(true)}
        onCancel={() => handleDeleteConfirmationDialog(false)}
        isDeleting={deleteInProgress}
      />
      {relationships.beneficiaryOfList && relationships.beneficiaryOfList.results.length > 0 && (
        <>
          <div className="beneficiary-of-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", paddingX: "24px", marginY: "8px" }}>
              {`Beneficiary Of (${relationships.beneficiaryOfList?.total || 0} ${relationships.beneficiaryOfList?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={GRID_KEYS.BENEFICIARY_OF}
            isLoading={relationships.isLoading}
            providedOptions={{
              rowData: relationships.beneficiaryOfList?.results,
              columnDefs: beneficiaryOfColumnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!relationships.beneficiaryList && (
        <>
          <div className="beneficiaries-list-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", paddingX: "24px", marginY: "8px" }}>
              {`Beneficiaries (${relationships.beneficiaryList?.total || 0} ${relationships.beneficiaryList?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={GRID_KEYS.BENEFICIARIES_LIST}
            isLoading={relationships.isLoading}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: relationships.beneficiaryList?.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!relationships.beneficiaryList &&
        relationships.beneficiaryList.results &&
        relationships.beneficiaryList?.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => handlePageNumberChange(value - 1)}
            pageSize={pageSize}
            setPageSize={handlePageSizeChange}
            recordCount={relationships.beneficiaryList?.total}
          />
        )}

      {/* Snackbar for percentage update feedback */}
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={4000}
        onClose={() => setSnackbarOpen(false)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}>
        <Alert
          onClose={() => setSnackbarOpen(false)}
          severity={snackbarSeverity}
          sx={{ width: "100%" }}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </>
  );
};

export default BeneficiaryRelationshipsGrids;
