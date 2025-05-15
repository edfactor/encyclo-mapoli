import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { BeneficiaryInquiryGridColumns } from "./BeneficiaryInquiryGridColumn";
import { BeneficiaryRequestDto, MasterInquiryRequest } from "reduxstore/types";
import { CAPTIONS } from "../../constants";
import { useLazyGetBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";
interface MasterInquiryGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  //handleSortChanged: (sort: ISortParams) => void;
}

const BeneficiaryInquiryGrid: React.FC<MasterInquiryGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "fullName",
    isSortDescending: true
  });

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { beneficiaryList, beneficiaryRequest } = useSelector((state: RootState) => state.beneficiaries);
  const [triggerSearch, { isFetching }] = useLazyGetBeneficiariesQuery();

  const createBeneficiaryInquiryRequest = useCallback(
    (skip: number, sortBy: string, isSortDescending: boolean): BeneficiaryRequestDto | null => {
      if (!beneficiaryRequest) return null;

      return beneficiaryRequest;
    },
    [beneficiaryRequest, pageSize, _sortParams]
  );

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "fullName";
      update.isSortDescending = true;
    }
    setSortParams(update);
    setPageNumber(0);

    const request = createBeneficiaryInquiryRequest(0, update.sortBy, update.isSortDescending);
    if (!request) return;

    triggerSearch(request, false);
  };

  const columnDefs = useMemo(() => BeneficiaryInquiryGridColumns(), []);

  const onSearch = useCallback(async () => {
    const request = createBeneficiaryInquiryRequest(pageNumber * pageSize, _sortParams.sortBy, _sortParams.isSortDescending);
    if (!request) return;

    await triggerSearch(request, false);
  }, [createBeneficiaryInquiryRequest, pageNumber, pageSize, _sortParams, triggerSearch]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, _sortParams, onSearch]);

  return (
    <>
      {!!beneficiaryList && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Beneficiary (${beneficiaryList?.beneficiaryList?.total || 0} ${beneficiaryList?.beneficiaryList?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.BENEFICIARY_INQUIRY}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: beneficiaryList?.beneficiaryList?.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!beneficiaryList && beneficiaryList.beneficiaryList && beneficiaryList.beneficiaryList?.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={beneficiaryList.beneficiaryList?.total}
        />
      )}
    </>
  );
};

export default BeneficiaryInquiryGrid;
