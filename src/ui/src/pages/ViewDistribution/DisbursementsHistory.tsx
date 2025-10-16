import { Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useLazySearchDistributionsQuery } from "../../reduxstore/api/DistributionApi";
import DisbursementGrid from "./DisbursementGrid";

interface DisbursementsHistoryProps {
  badgeNumber: number;
  memberType: number; // 1 for employee, 2 for beneficiary
}

const DisbursementsHistory: React.FC<DisbursementsHistoryProps> = ({ badgeNumber, memberType }) => {
  const [triggerSearch, { data, isFetching }] = useLazySearchDistributionsQuery();
  const [searchParams, setSearchParams] = useState({
    skip: 0,
    take: 25,
    sortBy: "paymentSequence",
    isSortDescending: false
  });

  useEffect(() => {
    const fetchHistoricalDisbursements = async () => {
      try {
        await triggerSearch({
          badgeNumber,
          memberType,
          distributionStatusIds: ["P"],
          ...searchParams
        }).unwrap();
      } catch (error) {
        console.error("Failed to fetch historical disbursements:", error);
      }
    };

    fetchHistoricalDisbursements();
  }, [badgeNumber, memberType, searchParams, triggerSearch]);

  const handlePaginationChange = useCallback(async (pageNumber: number, pageSize: number, sortParams: any) => {
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
        title="Disbursements History"
        data={data?.results ?? null}
        totalRecords={data?.total ?? 0}
        isLoading={isFetching}
        rowsPerPageOptions={[10, 25, 50]}
        onPaginationChange={handlePaginationChange}
      />
    </Grid>
  );
};

export default DisbursementsHistory;
