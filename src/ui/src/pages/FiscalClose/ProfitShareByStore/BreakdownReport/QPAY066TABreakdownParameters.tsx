import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, Grid, MenuItem, Select, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { useDispatch } from "react-redux";
import {
  clearBreakdownByStore,
  clearBreakdownByStoreMangement,
  clearBreakdownByStoreTotals,
  clearBreakdownGrandTotals,
  setBreakdownByStoreQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DuplicateSsnGuard from "../../../../components/DuplicateSsnGuard";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";

interface BreakdownSearchParams {
  store?: number | null;
  employeeStatus?: string;
  badgeId?: number | null;
  employeeName?: string;
  sortBy: string;
  isSortDescending: boolean;
}

interface OptionItem {
  id: number;
  label: string;
}

const schema = yup.object().shape({
  store: yup.number(),
  employeeStatus: yup.string(),
  badgeId: yup.number(),
  employeeName: yup.string(),
  sortBy: yup.string(),
  isSortDescending: yup.boolean()
});

interface QPAY066TABreakdownParametersProps {
  activeTab: "all" | "stores" | "summaries" | "totals";
  onStoreChange?: (store: number | null) => void;
  onReset?: () => void;
}

const QPAY066TABreakdownParameters: React.FC<QPAY066TABreakdownParametersProps> = ({
  activeTab,
  onStoreChange,
  onReset
}) => {
  const dispatch = useDispatch();
  const profitYear = useDecemberFlowProfitYear();

  const [employeeStatuses] = useState<OptionItem[]>([
    { id: 700, label: "700 - Retired - Drawing Pension" },
    { id: 701, label: "701 - Active - Drawing Pension" },
    { id: 800, label: "800 - Terminated" },
    { id: 801, label: "801 - Terminated w/ Zero Balance" },
    { id: 802, label: "802 - Terminated w/ Balance but No Vesting" },
    { id: 900, label: "900 - Monthly Payroll" }
  ]);

  const {
    control,
    handleSubmit,
    formState: { isValid },
    reset,
    setValue,
    watch
  } = useForm<BreakdownSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      store: null,
      employeeStatus: "",
      badgeId: null,
      employeeName: "",
      sortBy: "badgeNumber",
      isSortDescending: true
    }
  });

  const employeeStatus = useWatch({
    control,
    name: "employeeStatus"
  });

  useEffect(() => {
    if (employeeStatus && employeeStatus !== "") {
      setValue("store", employeeStatus);
      if (onStoreChange) {
        onStoreChange(employeeStatus);
      }
    }
  }, [employeeStatus, setValue, onStoreChange]);

  const validateAndSubmit = handleSubmit((data) => {
    console.log("Form data being submitted:", data);

    // Always submit regardless of isValid to enable searching with any combination
    if (onStoreChange && data.store) {
      onStoreChange(data.store);
    }

    dispatch(
      setBreakdownByStoreQueryParams({
        profitYear: profitYear,
        storeNumber: data.store,
        badgeNumber: data.badgeId,
        employeeName: data.employeeName,
        pagination: {
          take: 25,
          skip: 0,
          sortBy: data.sortBy,
          isSortDescending: data.isSortDescending
        }
      })
    );
  });

  const handleReset = () => {
    reset({
      store: null,
      employeeStatus: "",
      badgeId: null,
      employeeName: "",
      sortBy: "badgeNumber",
      isSortDescending: true
    });
    dispatch(clearBreakdownByStore());
    dispatch(clearBreakdownByStoreMangement());
    dispatch(clearBreakdownByStoreTotals());
    dispatch(clearBreakdownGrandTotals());

    if (onStoreChange) {
      onStoreChange(null);
    }

    // allow parent to clear grids
    if (onReset) {
      onReset();
    }
  };

  const getGridSizes = () => {
    if (activeTab === "all" || activeTab === "stores") {
      return { xs: 12, sm: 6, md: 3 };
    } else {
      return { xs: 12, sm: 6, md: 4 };
    }
  };

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        // Ensure the form values are captured and submitted
        validateAndSubmit();
      }}>
      <Grid
        container
        paddingX="24px"
        alignItems="flex-end"
        spacing={1.5}>
        <Grid size={getGridSizes()}>
          <Controller
            name="store"
            control={control}
            render={({ field }) => (
              <>
                <FormLabel
                  htmlFor="store-input"
                  sx={{ display: "block", marginBottom: "8px" }}>
                  Store
                </FormLabel>
                <TextField
                  id="store-input"
                  {...field}
                  size="small"
                  fullWidth
                  type="number"
                  value={field.value || ""}
                  onChange={(e) => {
                    const value = e.target.value === "" ? "" : Number(e.target.value);
                    field.onChange(value);
                    if (onStoreChange && value !== "") {
                      onStoreChange(Number(value));
                    }
                  }}
                />
              </>
            )}
          />
        </Grid>

        {(activeTab === "all" || activeTab === "stores") && (
          <>
            <Grid size={getGridSizes()}>
              <Controller
                name="employeeStatus"
                control={control}
                render={({ field }) => (
                  <>
                    <FormLabel htmlFor="status-select">Employee Status</FormLabel>
                    <Select
                      id="status-select"
                      {...field}
                      size="small"
                      fullWidth>
                      <MenuItem value="">
                        <em>Clear Selection</em>
                      </MenuItem>
                      {employeeStatuses.map((status) => (
                        <MenuItem
                          key={status.id}
                          value={status.id}>
                          {status.label}
                        </MenuItem>
                      ))}
                    </Select>
                  </>
                )}
              />
            </Grid>

            <Grid size={getGridSizes()}>
              <Controller
                name="badgeId"
                control={control}
                render={({ field }) => (
                  <>
                    <FormLabel htmlFor="badge-id">Badge Number</FormLabel>
                    <TextField
                      id="badge-id"
                      {...field}
                      size="small"
                      fullWidth
                    />
                  </>
                )}
              />
            </Grid>

            <Grid size={getGridSizes()}>
              <Controller
                name="employeeName"
                control={control}
                render={({ field }) => (
                  <>
                    <FormLabel htmlFor="employee-name">Employee Name</FormLabel>
                    <TextField
                      id="employee-name"
                      {...field}
                      size="small"
                      fullWidth
                    />
                  </>
                )}
              />
            </Grid>
          </>
        )}
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        marginTop={2}>
        <DuplicateSsnGuard>
          {({ prerequisitesComplete }) => (
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={(e) => {
                e.preventDefault();
                // Call validateAndSubmit which contains the handleSubmit logic
                validateAndSubmit();
              }}
              isFetching={false}
              disabled={!prerequisitesComplete}
              searchButtonText="Search"
            />
          )}
        </DuplicateSsnGuard>
      </Grid>
    </form>
  );
};

export default QPAY066TABreakdownParameters;
