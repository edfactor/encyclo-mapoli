import { BeneficiaryDetailAPIRequest } from "@/types";
import { Delete, Edit } from "@mui/icons-material";
import { Alert, Button, TextField, Typography } from "@mui/material";
import { FocusEvent, JSX, useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import {
  useLazyDeleteBeneficiaryQuery,
  useLazyGetBeneficiariesQuery,
  useLazyUpdateBeneficiaryQuery
} from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Paged, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { SortParams, useGridPagination } from "../../hooks/useGridPagination";
import { BeneficiaryDetail, BeneficiaryDto } from "../../types";
import { BeneficiaryInquiryGridColumns } from "./BeneficiaryInquiryGridColumns";
import { GetBeneficiaryOfGridColumns } from "./BeneficiaryOfGridColumns";
import DeleteBeneficiaryDialog from "./DeleteBeneficiaryDialog";

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
  const [errorPercentage, setErrorPercentage] = useState<boolean>(false);
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [beneficiaryList, setBeneficiaryList] = useState<Paged<BeneficiaryDto> | undefined>();
  const [beneficiaryOfList, setBeneficiaryOfList] = useState<Paged<BeneficiaryDto> | undefined>();
  const [triggerSearch, { isFetching }] = useLazyGetBeneficiariesQuery();
  const [triggerUpdate] = useLazyUpdateBeneficiaryQuery();
  const [triggerDeleteBeneficiary] = useLazyDeleteBeneficiaryQuery();
  const [openDeleteConfirmationDialog, setOpenDeleteConfirmationDialog] = useState(false);
  const [deleteBeneficiaryId, setDeleteBeneficiaryId] = useState<number>(0);
  const [deleteInProgress, setDeleteInProgress] = useState<boolean>(false);
  const [internalChange, setInternalChange] = useState<number>(0);

  const createBeneficiaryInquiryRequest = (
    skip: number,
    sortBy: string,
    isSortDescending: boolean,
    take: number,
    badgeNumber?: number,
    psnSuffix?: number
  ): BeneficiaryDetailAPIRequest | null => {
    // if either identifier is missing return null so callers can guard on that
    if (badgeNumber == null || psnSuffix == null) return null;

    const request: BeneficiaryDetailAPIRequest = {
      badgeNumber: badgeNumber,
      psnSuffix: psnSuffix,
      isSortDescending: isSortDescending,
      skip: skip,
      sortBy: sortBy,
      take: take
    };
    return request;
  };

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "psnSuffix",
    initialSortDescending: true,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (selectedMember?.badgeNumber && selectedMember?.psnSuffix) {
          const request = createBeneficiaryInquiryRequest(
            pageNum * pageSz,
            sortPrms.sortBy,
            sortPrms.isSortDescending,
            pageSz,
            selectedMember?.badgeNumber,
            selectedMember?.psnSuffix
          );
          if (request) {
            triggerSearch(request, false)
              .unwrap()
              .then((res) => {
                setBeneficiaryList(res.beneficiaries);
                setBeneficiaryOfList(res.beneficiaryOf);
              });
          }
        }
      },
      [selectedMember?.badgeNumber, selectedMember?.psnSuffix, triggerSearch]
    )
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
          setInternalChange((prev) => prev + 1);
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

  const validatePercentageOfBeneficiaries = (e: FocusEvent<HTMLInputElement | HTMLTextAreaElement>, id: number) => {
    let sum: number = 0;
    const currentValue = e.target.value ? parseInt(e.target.value) : 0;
    beneficiaryList?.results.forEach((value) => {
      sum += value.id == id ? currentValue : value.percent;
    });
    const prevObj = beneficiaryList?.results.filter((x) => x.id == id);

    if (sum <= 100) {
      setErrorPercentage(false);
      //call api to save the percentage.

      triggerUpdate({ id: id, percentage: currentValue }, false)
        .unwrap()
        .then((_res) => {
          if (hasToken) onSearch();
        });
    } else {
      setErrorPercentage(true);
      e.target.value = prevObj ? prevObj[0].percent + "" : "";
    }
  };

  const percentageFieldRenderer = (percentage: number, id: number) => {
    return (
      <>
        <TextField
          type="number"
          defaultValue={percentage}
          onBlur={(e) => validatePercentageOfBeneficiaries(e, id)}
          onClick={() => console.log(id)}></TextField>
      </>
    );
  };

  const beneficiaryOfColumnDefs = useMemo(() => GetBeneficiaryOfGridColumns(), []);

  const columnDefs = useMemo(() => {
    const columns = BeneficiaryInquiryGridColumns(percentageFieldRenderer);
    return [...columns];
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [actionButtons]);

  const onSearch = useCallback(() => {
    console.log("Beneficiary Inquiry Grid - onSearch called");
    console.log("Selected Member:", selectedMember);
    const request = createBeneficiaryInquiryRequest(
      pageNumber * pageSize,
      sortParams.sortBy,
      sortParams.isSortDescending,
      pageSize,
      selectedMember?.badgeNumber,
      selectedMember?.psnSuffix
    );
    if (!request) return;

    triggerSearch(request, false)
      .unwrap()
      .then((res) => {
        console.log("Beneficiary Inquiry Search Result:", res);
        setBeneficiaryList(res.beneficiaries);
        setBeneficiaryOfList(res.beneficiaryOf);
      });
  }, [selectedMember, sortParams, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [selectedMember, count, internalChange, onSearch, hasToken]);

  return (
    <>
      <DeleteBeneficiaryDialog
        open={openDeleteConfirmationDialog}
        onConfirm={() => handleDeleteConfirmationDialog(true)}
        onCancel={() => handleDeleteConfirmationDialog(false)}
        isDeleting={deleteInProgress}
      />
      {beneficiaryOfList && beneficiaryOfList.results.length > 0 && (
        <>
          <div className="beneficiary-of-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", paddingX: "24px", marginY: "8px" }}>
              {`Beneficiary Of (${beneficiaryOfList?.total || 0} ${beneficiaryOfList?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.BENEFICIARY_OF}
            isLoading={isFetching}
            providedOptions={{
              rowData: beneficiaryOfList?.results,
              columnDefs: beneficiaryOfColumnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!beneficiaryList && (
        <>
          {errorPercentage ? (
            <Alert
              variant="filled"
              severity="error">
              % Percentage should be equal to 100%
            </Alert>
          ) : (
            <></>
          )}

          <div className="beneficiaries-list-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", paddingX: "24px", marginY: "8px" }}>
              {`Beneficiaries (${beneficiaryList?.total || 0} ${beneficiaryList?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.BENEFICIARIES_LIST}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: beneficiaryList?.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!beneficiaryList && beneficiaryList.results && beneficiaryList?.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={beneficiaryList?.total}
        />
      )}
    </>
  );
};

export default BeneficiaryRelationshipsGrids;
