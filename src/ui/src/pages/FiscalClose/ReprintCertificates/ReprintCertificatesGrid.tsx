import { Typography } from "@mui/material";
import { SelectionChangedEvent } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { ISortParams } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
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
  const [_selectedRowIds, setSelectedRowIds] = useState<number[]>([]);

  const { certificates } = useSelector((state: RootState) => state.yearsEnd);
  const [getCertificatesReport, { isFetching }] = useLazyGetCertificatesReportQuery();

  const buildApiRequest = useCallback(
    (pn: number, ps: number, sp: ISortParams): CertificatePrintRequest => {
      const request: CertificatePrintRequest = {
        profitYear: filterParams.profitYear,
        skip: pn * ps,
        take: ps,
        sortBy: sp.sortBy,
        isSortDescending: sp.isSortDescending
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
    },
    [filterParams]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.REPRINT_CERTIFICATES,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: ISortParams) => {
        if (certificates && (pageNum > 0 || sortPrms.sortBy !== "badgeNumber" || sortPrms.isSortDescending !== false)) {
          const request = buildApiRequest(pageNum, pageSz, sortPrms);
          getCertificatesReport(request);
        }
      },
      [certificates, buildApiRequest, getCertificatesReport]
    )
  });

  const { pageNumber, pageSize, sortParams } = pagination;

  useEffect(() => {
    if (
      certificates &&
      (pageNumber > 0 || sortParams.sortBy !== "badgeNumber" || sortParams.isSortDescending !== false)
    ) {
      const request = buildApiRequest(pageNumber, pageSize, sortParams);
      getCertificatesReport(request);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber, pageSize, sortParams]);

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

  const columnDefs = useMemo(() => GetReprintCertificatesGridColumns(), []);

  const onSelectionChanged = useCallback(
    (event: SelectionChangedEvent<ReprintCertificateEmployee>) => {
      const selectedNodes = event.api.getSelectedNodes();
      const selectedIds = selectedNodes
        .map((node) => node.data?.badge)
        .filter((badge): badge is number => badge !== undefined);
      setSelectedRowIds(selectedIds);
      onSelectionChange?.(selectedIds);
    },
    [onSelectionChange]
  );

  const totalCount = certificates?.response?.total || 0;

  return (
    <DSMPaginatedGrid<ReprintCertificateEmployee>
      preferenceKey={GRID_KEYS.REPRINT_CERTIFICATES}
      data={gridData}
      columnDefs={columnDefs}
      totalRecords={totalCount}
      isLoading={isFetching}
      pagination={pagination}
      onSortChange={pagination.handleSortChange}
      showPagination={gridData.length > 0 || totalCount > 0}
      header={
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Print Profit Certificates ({totalCount})
        </Typography>
      }
      gridOptions={{
        rowSelection: {
          mode: "multiRow",
          checkboxes: true,
          headerCheckbox: true,
          enableClickSelection: false
        },
        onSelectionChanged: onSelectionChanged as (event: unknown) => void
      }}
    />
  );
};

export default ReprintCertificatesGrid;
