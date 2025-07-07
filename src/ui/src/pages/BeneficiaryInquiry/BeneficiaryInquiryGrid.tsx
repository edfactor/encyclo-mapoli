import { Button, TextField, Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect, JSX } from "react";
import { useSelector } from "react-redux";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Paged, Pagination } from "smart-ui-library";
import { BeneficiaryInquiryGridColumns } from "./BeneficiaryInquiryGridColumn";
import { BeneficiaryDto, BeneficiaryRequestDto, MasterInquiryRequest } from "reduxstore/types";
import { CAPTIONS } from "../../constants";
import { useLazyGetBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";
import { ICellRendererParams } from "ag-grid-community";
import { ChevronLeft, Close, ExpandLess, ExpandMore, Edit, Delete } from "@mui/icons-material";
interface BeneficiaryInquiryGridProps {
  // initialSearchLoaded: boolean;
  // setInitialSearchLoaded: (loaded: boolean) => void;
  selectedMember: any;
  count: number;
  createOrUpdateBeneficiary: (selectedMember:BeneficiaryDto)=>any;
  deleteBeneficiary: (id:number)=>any;
}

const BeneficiaryInquiryGrid: React.FC<BeneficiaryInquiryGridProps> = ({ selectedMember, count, createOrUpdateBeneficiary,deleteBeneficiary }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "psnSuffix",
    isSortDescending: true
  });

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  // const { beneficiaryList, beneficiaryRequest } = useSelector((state: RootState) => state.beneficiaries);
  const [beneficiaryList, setBeneficiaryList] = useState<Paged<BeneficiaryDto> | undefined>()
  const [triggerSearch, { isFetching }] = useLazyGetBeneficiariesQuery();

  // const createBeneficiaryInquiryRequest = useCallback(
  //   (skip: number, sortBy: string, isSortDescending: boolean, badgeNumber: number): BeneficiaryRequestDto | null => {
  //     if (!beneficiaryRequest) return null;
  //     return beneficiaryRequest;
  //   },
  //   [beneficiaryRequest, pageSize, _sortParams]
  // );1
  const createBeneficiaryInquiryRequest =
    (skip: number, sortBy: string, isSortDescending: boolean, take: number, badgeNumber: number, psnSuffix: number): BeneficiaryRequestDto | null => {
      const request: BeneficiaryRequestDto = {
        badgeNumber: badgeNumber,
        psnSuffix: psnSuffix,
        isSortDescending: isSortDescending,
        skip: skip,
        sortBy: sortBy,
        take: take
      }
      return request;
    }

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "psnSuffix";
      update.isSortDescending = true;
    }
    setSortParams(update);
    setPageNumber(0);

    const request = createBeneficiaryInquiryRequest(0, update.sortBy, update.isSortDescending, 25, selectedMember?.badgeNumber, selectedMember?.psnSuffix);
    if (!request) return;

    triggerSearch(request, false).unwrap().then((value) => {
      setBeneficiaryList(value);
    });
  };
  const actionButtons = (data:any): JSX.Element => {
    return (<><Button onClick={()=>{createOrUpdateBeneficiary(data)}} size="small" color="primary"><Edit fontSize="small" /></Button><Button onClick={()=>deleteBeneficiary(data.id)} size="small" color="error"><Delete fontSize="small" /></Button></>)
  }

  const percentageFieldRenderer =(percentage: number, id:number)=>{
    return (

      <>
        <TextField type="number" defaultValue={percentage} onClick={()=>console.log(id)}></TextField>
      </>
    )
  }

  const columnDefs = useMemo(() => {

    const columns = BeneficiaryInquiryGridColumns();
    columns.splice(6, 0,{
      headerName: "Percentage",
      field: "percentage",
      colId: "percentage",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      sortable: false,
      resizable: true,
      cellRenderer: (params: ICellRendererParams)=> percentageFieldRenderer(params.data.percent, params.data.id)
    }, )
    return [ ...columns,{
      headerName: "Actions",
      field: "actions",
      lockPinned: true,
      pinned:"right",
      resizable:false,
      sortable: false,
      cellStyle: { backgroundColor: '#E8E8E8' },
      minWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      cellRenderer: (params: ICellRendererParams) => { return actionButtons(params.data); }
    }];
  }, [])

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
    const request = createBeneficiaryInquiryRequest(pageNumber * pageSize, _sortParams.sortBy, _sortParams.isSortDescending, 25, selectedMember?.badgeNumber, selectedMember?.psnSuffix);
    if (!request) return;

    triggerSearch(request, false).unwrap().then((value) => {
      setBeneficiaryList(value)
    });
  }, [selectedMember]
  );

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [selectedMember, pageNumber, pageSize, _sortParams, count]);

  return (
    <>
      {!!beneficiaryList && (
        <>
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
