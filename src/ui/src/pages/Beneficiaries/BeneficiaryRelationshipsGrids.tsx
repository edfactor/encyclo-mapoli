import { Delete, Edit } from "@mui/icons-material";
import { Button, TextField, Typography } from "@mui/material";
import { FocusEvent, JSX, useCallback, useMemo, useState } from "react";
import { useLazyDeleteBeneficiaryQuery } from "reduxstore/api/BeneficiariesApi";
import { DSMGrid, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { SortParams, useGridPagination } from "../../hooks/useGridPagination";
import { BeneficiaryDetail, BeneficiaryDto } from "../../types";
import { GetBeneficiariesListGridColumns } from "./BeneficiariesListGridColumns";
import { GetBeneficiaryOfGridColumns } from "./BeneficiaryOfGridColumns";
import DeleteBeneficiaryDialog from "./DeleteBeneficiaryDialog";
import { useBeneficiaryRelationshipData } from "./hooks/useBeneficiaryRelationshipData";
import { useBeneficiaryPercentageUpdate } from "./hooks/useBeneficiaryPercentageUpdate";

interface BeneficiaryRelationshipsProps {
  selectedMember: BeneficiaryDetail | null;
  count: number;
  onEditBeneficiary: (selectedMember: BeneficiaryDto | undefined) => void;
}

const BeneficiaryRelationshipsGrids: React.FC<BeneficiaryRelationshipsProps> = ({
  selectedMember,
  count,
  onEditBeneficiary
}) => {
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);
  const [deleteBeneficiaryId, setDeleteBeneficiaryId] = useState<number>(0);
  const [deleteInProgress, setDeleteInProgress] = useState<boolean>(false);
  const [triggerDeleteBeneficiary] = useLazyDeleteBeneficiaryQuery();

  // Use custom hooks for data fetching and percentage update
  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "psnSuffix",
    initialSortDescending: true
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

  const sortEventHandler = (update: SortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "psnSuffix";
      update.isSortDescending = true;
    }
    handleSortChange(update);
  };
  const deleteBeneficiary = (id: number) => {
    setDeleteBeneficiaryId(id);
    setOpenDeleteConfirmationDialog(true);
  };

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

  const actionButtons = (data: BeneficiaryDto): JSX.Element => {
    return (
      <>
        <Button
          onClick={() => {
            onEditBeneficiary(data);
          }}
          size="small"
          color="primary">
          <Edit fontSize="small" />
        </Button>
        <Button
          onClick={() => deleteBeneficiary(data.id)}
          size="small"
          color="error">
          <Delete fontSize="small" />
        </Button>
      </>
    );
  };

  const validatePercentageOfBeneficiaries = useCallback(
    async (e: FocusEvent<HTMLInputElement | HTMLTextAreaElement>, id: number) => {
      const currentValue = e.target.value ? parseInt(e.target.value) : 0;

      if (!relationships.beneficiaryList?.results) {
        return;
      }

      const result = await percentageUpdate.validateAndUpdate(id, currentValue, relationships.beneficiaryList.results);

      if (!result.success && e.target.value) {
        // Restore previous value on validation failure
        e.target.value = result.previousValue?.toString() || "";
      }
    },
    [relationships.beneficiaryList, percentageUpdate]
  );

  const percentageFieldRenderer = (percentage: number, id: number) => {
    return (
      <>
        <TextField
          type="number"
          defaultValue={percentage}
          onBlur={(e) => validatePercentageOfBeneficiaries(e, id)}></TextField>
      </>
    );
  };

  const beneficiaryOfColumnDefs = useMemo(() => GetBeneficiaryOfGridColumns(), []);

  const columnDefs = useMemo(() => {
    const columns = GetBeneficiariesListGridColumns(percentageFieldRenderer);
    return [...columns];
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [actionButtons]);

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
            preferenceKey={CAPTIONS.BENEFICIARY_OF}
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
            preferenceKey={CAPTIONS.BENEFICIARIES_LIST}
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
            setPageNumber={(value: number) => {
              handlePaginationChange(value - 1, pageSize);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              handlePaginationChange(0, value);
            }}
            recordCount={relationships.beneficiaryList?.total}
          />
        )}
    </>
  );
};

export default BeneficiaryRelationshipsGrids;
