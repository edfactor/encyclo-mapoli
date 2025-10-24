import { Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useLazySearchDistributionsQuery } from "../../reduxstore/api/DistributionApi";
import { SortParams } from "../../hooks/useGridPagination";
import DisbursementGrid from "./DisbursementGrid";

interface PendingDisbursementsListProps {
  badgeNumber: number;
  memberType: number; // 1 for employee, 2 for beneficiary
}

const PendingDisbursementsList: React.FC<PendingDisbursementsListProps> = ({ badgeNumber, memberType }) => {
  const [triggerSearch, { data, isFetching }] = useLazySearchDistributionsQuery();
  const [searchParams, setSearchParams] = useState({
    skip: 0,
    take: 5,
    sortBy: "paymentSequence",
    isSortDescending: false
  });

  useEffect(() => {
    const fetchPendingDisbursements = async () => {
      try {
        await triggerSearch({
          badgeNumber,
          memberType,
          distributionStatusIds: ["H", "Y", "C"],
          ...searchParams
        }).unwrap();
      } catch (error) {
        console.error("Failed to fetch pending disbursements:", error);
      }
    };

    fetchPendingDisbursements();
  }, [badgeNumber, memberType, searchParams, triggerSearch]);

  const handlePaginationChange = useCallback(async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
    setSearchParams({
      skip: pageNumber * pageSize,
      take: pageSize,
      sortBy: sortParams.sortBy,
      isSortDescending: sortParams.isSortDescending
    });
  }, []);

  return (
    <Grid width="100%">
      <DisbursementGrid
        title="Pending Disbursements List"
        data={data?.results ?? null}
        totalRecords={data?.total ?? 0}
        isLoading={isFetching}
        initialPageSize={5}
        rowsPerPageOptions={[5, 10, 25]}
        onPaginationChange={handlePaginationChange}
      />
    </Grid>
  );
};

export default PendingDisbursementsList;
