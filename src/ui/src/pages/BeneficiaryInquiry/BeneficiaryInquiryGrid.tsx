import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Paged, Pagination } from "smart-ui-library";
import { BeneficiaryInquiryGridColumns } from "./BeneficiaryInquiryGridColumn";
import { BeneficiaryDto, BeneficiaryRequestDto, MasterInquiryRequest } from "reduxstore/types";
import { CAPTIONS } from "../../constants";
import { useLazyGetBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";
interface BeneficiaryInquiryGridProps {
  // initialSearchLoaded: boolean;
  // setInitialSearchLoaded: (loaded: boolean) => void;
  badgeNumber: number;
  selectedMember: any;
  change: number;
}

const BeneficiaryInquiryGrid: React.FC<BeneficiaryInquiryGridProps> = ({  badgeNumber, selectedMember }) => {
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
  // );
  const createBeneficiaryInquiryRequest =
    (skip: number, sortBy: string, isSortDescending: boolean,take:number, badgeNumber: number): BeneficiaryRequestDto | null => {
      let request: BeneficiaryRequestDto = {
        badgeNumber : badgeNumber,
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

    const request = createBeneficiaryInquiryRequest(0, update.sortBy, update.isSortDescending,25,badgeNumber);
    if (!request) return;

    triggerSearch(request, false).unwrap().then((value)=>{
      setBeneficiaryList(value);
    });
  };

  const columnDefs = useMemo(() => BeneficiaryInquiryGridColumns(), []);

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
    const request = createBeneficiaryInquiryRequest(pageNumber * pageSize, _sortParams.sortBy, _sortParams.isSortDescending,25,selectedMember?.badgeNumber);
    if (!request) return;

     triggerSearch(request, false).unwrap().then((value)=>{
      setBeneficiaryList(value)
     });
  },[selectedMember]
);

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [selectedMember,pageNumber, pageSize, _sortParams]);

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
