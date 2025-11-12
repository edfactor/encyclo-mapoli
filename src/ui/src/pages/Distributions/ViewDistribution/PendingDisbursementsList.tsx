import { ServiceErrorResponse } from "@/types/errors/errors";
import { Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { SortParams } from "../../../hooks/useGridPagination";
import { useLazySearchDistributionsQuery } from "../../../reduxstore/api/DistributionApi";
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
        // If memberType is 2, we need to create a new variable for psnSuffix that is the last four digits and then use the numbers before that as the badge number
        let psnSuffix = 0;
        let effectiveBadgeNumber = badgeNumber;
        if (memberType === 2) {
          const identifierStr = badgeNumber.toString();
          psnSuffix = parseInt(identifierStr.slice(-4), 10);
          effectiveBadgeNumber = parseInt(identifierStr.slice(0, -4), 10);
          console.log("Parsed badgeNumber:", effectiveBadgeNumber, "psnSuffix:", psnSuffix);
        }

        await triggerSearch({
          badgeNumber: effectiveBadgeNumber,
          psnSuffix: psnSuffix !== 0 ? psnSuffix : undefined,
          memberType,
          distributionStatusIds: ["H", "Y", "C"],
          ...searchParams
        }).unwrap();
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;

        // Check if it's a 500 error where it couldn't find the badge number and PSN suffix combination
        // Because this is expected if there are no pending disbursements, we won't log it
        if (
          serviceError?.data.status === 500 &&
          serviceError?.data?.title !== "Badge number and PSN suffix combination not found."
        ) {
          console.error("Pending Disbursements search failed:", error);
        }
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
