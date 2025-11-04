import { Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useLazyGetCertificatesReportQuery } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import { CertificatePrintRequest } from "../../../reduxstore/types";
import { ReprintCertificatesFilterParams } from "./ReprintCertificatesFilterSection";
import { GetReprintCertificatesGridColumns, ReprintCertificateEmployee } from "./ReprintCertificatesGridColumns";

interface ReprintCertificatesGridProps {
  filterParams: ReprintCertificatesFilterParams;
  onSelectionChange?: (selectedBadgeNumbers: number[]) => void;
}

const ReprintCertificatesGrid: React.FC<ReprintCertificatesGridProps> = ({ filterParams, onSelectionChange }) => {
  const [pageNumber, setPageNumber] = useState<number>(0);
  const [pageSize, setPageSize] = useState<number>(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [_selectedRowIds, setSelectedRowIds] = useState<number[]>([]);

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

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
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
    (event: { api: { getSelectedNodes: () => Array<{ data: { badge: number } }> } }) => {
      const selectedNodes = event.api.getSelectedNodes();
      const selectedIds = selectedNodes.map((node) => node.data.badge);
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
        maxHeight={gridMaxHeight}
        providedOptions={{
          rowData: gridData,
          columnDefs: columnDefs,
          rowSelection: {
            mode: "multiRow",
            checkboxes: true,
            headerCheckbox: true,
            enableClickSelection: false
          },
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
