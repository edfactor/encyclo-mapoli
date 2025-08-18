import { Delete, Edit } from "@mui/icons-material";
import { Alert, Button, TextField, Typography } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import { FocusEvent, JSX, useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBeneficiariesQuery, useLazyUpdateBeneficiaryQuery } from "reduxstore/api/BeneficiariesApi";
import { RootState } from "reduxstore/store";
import { BeneficiaryDto, BeneficiaryRequestDto } from "reduxstore/types";
import { DSMGrid, ISortParams, Paged, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { BeneficiaryInquiryGridColumns } from "./BeneficiaryInquiryGridColumns";
import { BeneficiaryOfGridColumns } from "./BeneficiaryOfGridColumn";
interface BeneficiaryInquiryGridProps {
  // initialSearchLoaded: boolean;
  // setInitialSearchLoaded: (loaded: boolean) => void;
  selectedMember: any;
  count: number;
  createOrUpdateBeneficiary: (selectedMember: BeneficiaryDto) => any;
  deleteBeneficiary: (id: number) => any;
  refresh: () => any;
}

const BeneficiaryInquiryGrid: React.FC<BeneficiaryInquiryGridProps> = ({
  refresh,
  selectedMember,
  count,
  createOrUpdateBeneficiary,
  deleteBeneficiary
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "psnSuffix",
    isSortDescending: true
  });
  const [errorPercentage, setErrorPercentage] = useState<boolean>(false);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  // const { beneficiaryList, beneficiaryRequest } = useSelector((state: RootState) => state.beneficiaries);
  const [beneficiaryList, setBeneficiaryList] = useState<Paged<BeneficiaryDto> | undefined>();
  const [beneficiaryOfList, setBeneficiaryOfList] = useState<Paged<BeneficiaryDto> | undefined>();
  const [triggerSearch, { isFetching }] = useLazyGetBeneficiariesQuery();
  const [triggerUpdate] = useLazyUpdateBeneficiaryQuery();

  // const createBeneficiaryInquiryRequest = useCallback(
  //   (skip: number, sortBy: string, isSortDescending: boolean, badgeNumber: number): BeneficiaryRequestDto | null => {
  //     if (!beneficiaryRequest) return null;
  //     return beneficiaryRequest;
  //   },
  //   [beneficiaryRequest, pageSize, _sortParams]
  // );1
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

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "psnSuffix";
      update.isSortDescending = true;
    }
    setSortParams(update);
    setPageNumber(0);

    // const request = createBeneficiaryInquiryRequest(
    //   0,
    //   update.sortBy,
    //   update.isSortDescending,
    //   25,
    //   selectedMember?.badgeNumber,
    //   selectedMember?.psnSuffix
    // );
    // if (!request) return;

    // triggerSearch(request, false)
    //   .unwrap()
    //   .then((value) => {
    //     setBeneficiaryList(value);
    //   });
  };
  const actionButtons = (data: any): JSX.Element => {
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
        .then((res) => {
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
  }, [beneficiaryList]);

  // const onSearch = useCallback(async () => {
  //   const request = createBeneficiaryInquiryRequest(pageNumber * pageSize, _sortParams.sortBy, _sortParams.isSortDescending,badgeNumber);
  //   if (!request) return;

  //   await triggerSearch(request, false);
  // }, [createBeneficiaryInquiryRequest, pageNumber, pageSize, _sortParams, triggerSearch]);

  // useEffect(() => {
  //   if (hasToken) {
  //     onSearch();
  //   }
  // }, [pageNumber, pageSize, _sortParams, onSearch]);

  const onSearch = useCallback(() => {
    const request = createBeneficiaryInquiryRequest(
      pageNumber * pageSize,
      _sortParams.sortBy,
      _sortParams.isSortDescending,
      25,
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
  }, [selectedMember, _sortParams]);

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [selectedMember, pageNumber, pageSize, _sortParams, count]);

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
            setPageNumber(value - 1);
            //setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            //setInitialSearchLoaded(true);
          }}
          recordCount={beneficiaryList?.total}
        />
      )}
    </>
  );
};

export default BeneficiaryInquiryGrid;
