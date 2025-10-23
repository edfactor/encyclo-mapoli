import { Delete, Edit } from "@mui/icons-material";
import { Alert, Button, TextField, Typography } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import { FocusEvent, JSX, useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBeneficiariesQuery, useLazyUpdateBeneficiaryQuery } from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { BeneficiaryDto, BeneficiaryRequestDto } from "reduxstore/types";
import { DSMGrid, Paged, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { SortParams, useGridPagination } from "../../hooks/useGridPagination";
import { BeneficiaryInquiryGridColumns } from "./BeneficiaryInquiryGridColumns";
import { BeneficiaryOfGridColumns } from "./BeneficiaryOfGridColumns";

interface BeneficiaryInquiryGridProps {
  selectedMember: BeneficiaryDto | null;
  count: number;
  createOrUpdateBeneficiary: (selectedMember: BeneficiaryDto | undefined) => void;
  deleteBeneficiary: (id: number) => void;
  //refresh: () => void;
}

const BeneficiaryInquiryGrid: React.FC<BeneficiaryInquiryGridProps> = ({
  //refresh,
  selectedMember,
  count,
  createOrUpdateBeneficiary,
  deleteBeneficiary
}) => {
  const [errorPercentage, setErrorPercentage] = useState<boolean>(false);
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [beneficiaryList, setBeneficiaryList] = useState<Paged<BeneficiaryDto> | undefined>();
  const [beneficiaryOfList, setBeneficiaryOfList] = useState<Paged<BeneficiaryDto> | undefined>();
  const [triggerSearch, { isFetching }] = useLazyGetBeneficiariesQuery();
  const [triggerUpdate] = useLazyUpdateBeneficiaryQuery();

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

  const createBeneficiaryInquiryRequest = (
    skip: number,
    sortBy: string,
    isSortDescending: boolean,
    take: number,
    badgeNumber: number,
    psnSuffix: number
  ): BeneficiaryRequestDto | null => {
    const request: BeneficiaryRequestDto = {
      badgeNumber: badgeNumber,
      psnSuffix: psnSuffix,
      isSortDescending: isSortDescending,
      skip: skip,
      sortBy: sortBy,
      take: take
    };
    return request;
  };

  const sortEventHandler = (update: SortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "psnSuffix";
      update.isSortDescending = true;
    }
    handleSortChange(update);
  };
  const actionButtons = (data: BeneficiaryDto): JSX.Element => {
    return (
      <>
        <Button
          onClick={() => {
            createOrUpdateBeneficiary(data);
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

  const validatePercentageOfBeneficiaries = (
    e: FocusEvent<HTMLInputElement | HTMLTextAreaElement, Element>,
    id: number
  ) => {
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

  const beneficiaryOfColumnDefs = useMemo(() => {
    return BeneficiaryOfGridColumns();
  }, []);

  const columnDefs = useMemo(() => {
    const columns = BeneficiaryInquiryGridColumns();
    columns.splice(6, 0, {
      headerName: "Percentage",
      field: "percentage",
      colId: "percentage",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      sortable: false,
      resizable: true,
      cellRenderer: (params: ICellRendererParams) => percentageFieldRenderer(params.data.percent, params.data.id)
    });
    return [
      ...columns,
      {
        headerName: "Actions",
        field: "actions",
        lockPinned: true,
        pinned: "right",
        resizable: false,
        sortable: false,
        cellStyle: { backgroundColor: "#E8E8E8" },
        minWidth: 200,
        headerClass: "center-align",
        cellClass: "center-align",
        cellRenderer: (params: ICellRendererParams) => {
          return actionButtons(params.data);
        }
      }
    ];
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [actionButtons]);

  const onSearch = useCallback(() => {
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
        setBeneficiaryList(res.beneficiaries);
        setBeneficiaryOfList(res.beneficiaryOf);
      });
  }, [selectedMember, sortParams, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [selectedMember, count, onSearch, hasToken]);

  return (
    <>
      {beneficiaryOfList && beneficiaryOfList.results.length > 0 && (
        <>
          <div className="master-inquiry-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
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
          <div className="master-inquiry-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Beneficiary (${beneficiaryList?.total || 0} ${beneficiaryList?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.BENEFICIARY_INQUIRY}
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
      {!!beneficiaryList && beneficiaryList && beneficiaryList?.results.length > 0 && (
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

export default BeneficiaryInquiryGrid;
