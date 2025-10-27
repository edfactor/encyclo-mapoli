import { Button, CircularProgress, Grid, Stack, Tooltip } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetUpdateSummaryQuery, useUpdateEnrollmentMutation } from "reduxstore/api/YearsEndApi";
import { setMessage } from "reduxstore/slices/messageSlice";
import { RootState } from "reduxstore/store";
import { ApiMessageAlert, DSMAccordion, numberToCurrency, Page, SmartModal, TotalsGrid } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import PayMasterUpdateGrid from "./PayMasterUpdateGrid";
import PayMasterUpdateSearchFilters from "./PayMasterUpdateSearchFilter";

interface ProfitYearSearch {
  profitYear: number;
}

interface NavigationItem {
  id: number;
  statusName?: string;
  items?: NavigationItem[];
  [key: string]: unknown;
}

const PayMasterUpdateSummary = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const { updateSummary } = useSelector((state: RootState) => state.yearsEnd);
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const isReadOnly = useReadOnlyNavigation();
  const dispatch = useDispatch();

  const [getUpdateSummary] = useLazyGetUpdateSummaryQuery();
  const [updateEnrollment, { isLoading: isUpdating }] = useUpdateEnrollmentMutation();

  useEffect(() => {
    setInitialSearchLoaded(true);
  }, []);

  // Initialize current status from navigation state
  useEffect(() => {
    const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");

    const getNavigationObjectBasedOnId = (
      navigationArray: NavigationItem[] | undefined,
      id: number | undefined
    ): NavigationItem | undefined => {
      if (navigationArray) {
        for (const item of navigationArray) {
          if (item.id === id) {
            return item;
          }
          if (item.items && item.items.length > 0) {
            const found = getNavigationObjectBasedOnId(item.items, id);
            if (found) {
              return found;
            }
          }
        }
      }
      return undefined;
    };

    const obj = getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId ?? undefined);
    if (obj) {
      setCurrentStatus(obj.statusName || null);
    }
  }, [navigationList]);

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    // Only trigger archive when status is changing TO "Complete" (not already "Complete")
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      // Trigger getUpdateSummary with archive=true
      getUpdateSummary({
        profitYear: fiscalCloseProfitYear,
        pagination: {
          skip: 0,
          take: 255,
          sortBy: "",
          isSortDescending: false
        },
        archive: true
      });
    } else {
      setCurrentStatus(statusName || null);
    }
  };

  const handleUpdate = async () => {
    try {
      await updateEnrollment({
        profitYear: fiscalCloseProfitYear ?? 0
      }).unwrap();
      dispatch(
        setMessage({
          key: "UpdateEnrollment",
          message: {
            type: "success",
            title: "Enrollment Updated",
            message: "Enrollment has been successfully updated."
          }
        })
      );
    } catch (error: unknown) {
      dispatch(
        setMessage({
          key: "UpdateEnrollment",
          message: {
            type: "error",
            title: "Update Enrollment Failed",
            message: "Failed to update enrollment. Please try again."
          }
        })
      );
      console.error("Update enrollment failed:", error);
    } finally {
      setIsModalOpen(false);
    }
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const renderActionNode = () => {
    if (!updateSummary) return null;

    const updateButton = (
      <Button
        onClick={isReadOnly ? undefined : () => setIsModalOpen(true)}
        variant="outlined"
        disabled={isReadOnly}
        className="h-10 min-w-fit whitespace-nowrap">
        Update
      </Button>
    );

    return (
      <Stack
        direction="row"
        spacing={2}>
        {isReadOnly ? (
          <Tooltip title="You are in read-only mode and cannot update enrollment.">
            <span>{updateButton}</span>
          </Tooltip>
        ) : (
          updateButton
        )}
        <StatusDropdownActionNode onStatusChange={handleStatusChange} />
      </Stack>
    );
  };
  /*
  const updateSummarySection = [
    {
      label: "Employees Updated",
      value: updateSummary ? updateSummary.totalNumberOfEmployees.toString() : "-"
    },
    {
      label: "Beneficiaries Updated",
      value: updateSummary ? updateSummary.totalNumberOfBeneficiaries.toString() : "-"
    },
    {
      label: "Before Profit Sharing Amount",
      value: updateSummary ? numberToCurrency(updateSummary.totalBeforeProfitSharingAmount) : "-"
    },
    {
      label: "Before Vested Amount",
      value: updateSummary ? numberToCurrency(updateSummary.totalBeforeVestedAmount) : "-"
    },
    {
      label: "After Profit Sharing Amount",
      value: updateSummary ? numberToCurrency(updateSummary.totalAfterProfitSharingAmount) : "-"
    },
    {
      label: "After Vested Amount",
      value: updateSummary ? numberToCurrency(updateSummary.totalAfterVestedAmount) : "-"
    }
  ];*/
  const onSearch = (data: ProfitYearSearch) => {
    setInitialSearchLoaded(true);
    getUpdateSummary({
      profitYear: data.profitYear,
      pagination: {
        skip: 0,
        take: 255,
        sortBy: "",
        isSortDescending: false
      }
    });
  };

  return (
    <Page
      label={CAPTIONS.PAY450_SUMMARY}
      actionNode={renderActionNode()}>
      <ApiMessageAlert commonKey="UpdateEnrollment" />
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <PayMasterUpdateSearchFilters
              onSearch={onSearch}
              setPageReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>

        {updateSummary?.response && (
          <Grid paddingX="24px">
            <TotalsGrid
              displayData={[
                [
                  updateSummary ? updateSummary.totalNumberOfEmployees.toString() : "-",
                  updateSummary ? updateSummary.totalNumberOfBeneficiaries.toString() : "-",
                  updateSummary ? numberToCurrency(updateSummary.totalBeforeProfitSharingAmount) : "-",
                  updateSummary ? numberToCurrency(updateSummary.totalBeforeVestedAmount) : "-",
                  updateSummary ? numberToCurrency(updateSummary.totalAfterProfitSharingAmount) : "-",
                  updateSummary ? numberToCurrency(updateSummary.totalAfterVestedAmount) : "-"
                ]
              ]}
              leftColumnHeaders={[]}
              topRowHeaders={[
                "Employees Updated",
                "Beneficiaries Updated",
                "Before Profit Sharing Amount",
                "Before Vested Amount",
                "After Profit Sharing Amount",
                "After Vested Amount"
              ]}
              breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
            />
          </Grid>
        )}
        <Grid width="100%">
          <PayMasterUpdateGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            profitYear={fiscalCloseProfitYear}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>

      <SmartModal
        open={isModalOpen}
        onClose={handleCancel}
        actions={[
          <Button
            onClick={handleUpdate}
            variant="contained"
            color="primary"
            disabled={isUpdating}
            className="mr-2">
            Yes, Update
            {isUpdating && (
              <CircularProgress
                size={"15px"}
                color={"inherit"}
              />
            )}
          </Button>,
          <Button
            onClick={handleCancel}
            variant="outlined"
            disabled={isUpdating}>
            No, Cancel
          </Button>
        ]}
        title="Update Enrollment">
        This update will bring new employees into the Profit Sharing System as enrolled members
      </SmartModal>
    </Page>
  );
};

export default PayMasterUpdateSummary;
