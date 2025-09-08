import { Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetCertificatesReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { CertificatePrintRequest } from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { ReprintCertificatesFilterParams } from "./ReprintCertificatesFilterSection";
import { GetReprintCertificatesGridColumns, ReprintCertificateEmployee } from "./ReprintCertificatesGridColumns";

interface ReprintCertificatesGridProps {
  filterParams: ReprintCertificatesFilterParams;
  onSelectionChange?: (selectedBadgeNumbers: number[]) => void;
}

const ReprintCertificatesGrid: React.FC<ReprintCertificatesGridProps> = ({ filterParams, onSelectionChange }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);

  const { certificates } = useSelector((state: RootState) => state.yearsEnd);
  const [getCertificatesReport, { isFetching }] = useLazyGetCertificatesReportQuery();

  const buildApiRequest = useCallback((): CertificatePrintRequest => {
    const request: CertificatePrintRequest = {
      profitYear: filterParams.profitYear,
      skip: pageNumber * pageSize,
      take: pageSize,
      sortBy: sortParams.sortBy,
      isSortDescending: sortParams.isSortDescending
    };

    if (filterParams.badgeNumber) {
      const badgeNumbers = filterParams.badgeNumber
        .split(",")
        .map((num) => parseInt(num.trim()))
        .filter((num) => !isNaN(num));
      if (badgeNumbers.length > 0) {
        request.badgeNumbers = badgeNumbers;
      }
    }

    if (filterParams.socialSecurityNumber) {
      const ssns = filterParams.socialSecurityNumber
        .split(",")
        .map((ssn) => parseInt(ssn.replace(/\D/g, "")))
        .filter((ssn) => !isNaN(ssn) && ssn > 0);
      if (ssns.length > 0) {
        request.ssns = ssns;
      }
    }

    return request;
  }, [filterParams, pageNumber, pageSize, sortParams]);

  useEffect(() => {
    if (
      certificates &&
      (pageNumber > 0 || sortParams.sortBy !== "badgeNumber" || sortParams.isSortDescending !== false)
    ) {
      const request = buildApiRequest();
      getCertificatesReport(request);
    }
  }, [pageNumber, pageSize, sortParams]);

  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
  };

  const gridData = useMemo(() => {
    if (!certificates?.response?.results) return [];

    return certificates.response.results.map(
      (cert): ReprintCertificateEmployee => ({
        badge: cert.badgeNumber,
        name: cert.fullName,
        eoyBalance: cert.beginningBalance,
        forfeitures2024: cert.forfeitures,
        withdrawals2024: cert.distributions,
        balance: cert.endingBalance,
        vestedPortion: cert.vestedAmount,
        singleLifeAnnuity: cert.annuitySingleRate || 0,
        qualifiedJoinAndSurvivor: cert.annuityJointRate || 0
      })
    );
  }, [certificates]);

  const columnDefs = useMemo(
    () => GetReprintCertificatesGridColumns(),
    //selectedRowIds,
    //() => {},
    //() => {}
    []
  );

  const onSelectionChanged = useCallback(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (event: any) => {
      const selectedNodes = event.api.getSelectedNodes();
      const selectedIds = selectedNodes.map((node: any) => node.data.badge);
      setSelectedRowIds(selectedIds);
      onSelectionChange?.(selectedIds);
    },
    [onSelectionChange]
  );

  const totalCount = certificates?.response?.total || 0;

  return (
    <>
      <div style={{ padding: "0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Print Profit Certificates ({totalCount})
        </Typography>
      </div>

      <DSMGrid
        preferenceKey="REPRINT_CERTIFICATES_GRID"
        isLoading={isFetching}
        handleSortChanged={sortEventHandler}
        maxHeight={400}
        providedOptions={{
          rowData: gridData,
          columnDefs: columnDefs,
          rowSelection: "multiple",
          suppressRowClickSelection: true,
          onSelectionChanged: onSelectionChanged
        }}
      />
      {(gridData.length > 0 || totalCount > 0) && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => setPageNumber(value - 1)}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={totalCount}
        />
      )}
    </>
  );
};

export default ReprintCertificatesGrid;
