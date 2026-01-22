import { yupResolver } from "@hookform/resolvers/yup";
import { Autocomplete, FormLabel, Grid, TextField } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import {
  clearBreakdownByStore,
  clearBreakdownByStoreManagement,
  clearBreakdownByStoreTotals,
  clearBreakdownGrandTotals,
  setBreakdownByStoreQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { useGetStoresQuery } from "reduxstore/api/LookupsApi";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DuplicateSsnGuard from "../../../../components/DuplicateSsnGuard";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { mustBeNumberValidator } from "../../../../utils/FormValidators";

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
  store: mustBeNumberValidator().nullable(),
  employeeStatus: yup.string(),
  badgeId: yup.number(),
  employeeName: yup.string(),
  sortBy: yup.string(),
  isSortDescending: yup.boolean()
});

interface QPAY066TABreakdownParametersProps {
  onStoreChange?: (store: number | null) => void;
  onReset?: () => void;
  isLoading?: boolean;
  onSearch?: () => void;
  /** Initial store value to populate the form (used when remounting after grid collapse) */
  initialStore?: number | null;
}

const QPAY066TABreakdownParameters: React.FC<QPAY066TABreakdownParametersProps> = ({
  onStoreChange,
  onReset,
  isLoading = false,
  onSearch,
  initialStore = null
}) => {
  const dispatch = useDispatch();
  const profitYear = useDecemberFlowProfitYear();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [storeInputValue, setStoreInputValue] = useState("");
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  
  // Fetch stores from API
  const { data: apiStores, isError: isStoresError, isLoading: isStoresLoading } = useGetStoresQuery(undefined, {
    skip: !hasToken
  });

  // Combine blank entry, "All Stores", and API-fetched stores
  const stores: OptionItem[] = useMemo(() => [
    { id: 0, label: "" },
    { id: -1, label: "All Stores" },
    { id: 700, label: "700 - Retired - Drawing Pension" },
    { id: 701, label: "701 - Active - Drawing Pension" },
    { id: 800, label: "800 - Terminated" },
    { id: 801, label: "801 - Terminated w/ Zero Balance" },
    { id: 802, label: "802 - Terminated w/ Balance but No Vesting" },
    { id: 900, label: "900 - Monthly Payroll" },
    ...(apiStores || [])
  ], [apiStores]);

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const {
    control,
    handleSubmit,
    getValues,
    reset,
    setValue
  } = useForm<BreakdownSearchParams>({
    resolver: yupResolver(schema) as Resolver<BreakdownSearchParams>,
    defaultValues: {
      store: initialStore,
      employeeStatus: "",
      badgeId: undefined,
      employeeName: "",
      sortBy: "badgeNumber",
      isSortDescending: true
    }
  });

  const employeeStatus = useWatch({
    control,
    name: "employeeStatus"
  });

  const store = useWatch({
    control,
    name: "store"
  });

  // Sync storeInputValue with the selected store value
  useEffect(() => {
    if (store === null || store === undefined || store === 0) {
      setStoreInputValue("");
    } else {
      const selectedStore = stores.find(s => s.id === store);
      if (selectedStore) {
        setStoreInputValue(selectedStore.label);
      }
    }
  }, [store, stores]);

  // Check if the current store input is valid (matches an option or is empty)
  // Use startsWith to allow partial matching during typing
  const isStoreInputValid = storeInputValue === "" || 
    stores.some(s => s.label.toLowerCase().startsWith(storeInputValue.toLowerCase()));

  useEffect(() => {
    if (employeeStatus && employeeStatus !== "") {
      setValue("employeeStatus", employeeStatus);
    }
  }, [employeeStatus, setValue]);

  const validateAndSubmit = handleSubmit((data) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      
      // Explicitly filter badge number - only include if it's a positive number
      const badgeNumber = data.badgeId && typeof data.badgeId === 'number' && data.badgeId > 0 
        ? data.badgeId 
        : undefined;
      
      // Explicitly filter employee name - only include if it's a non-empty string
      const employeeName = data.employeeName && typeof data.employeeName === 'string' && data.employeeName.trim() !== ""
        ? data.employeeName.trim()
        : undefined;
      
      // Always submit regardless of isValid to enable searching with any combination
      if (onStoreChange) {
        onStoreChange(data.store ?? null);
      }

      // Set the query params with explicit undefined values (like reset does)
      // Don't clear the data stores - let the normal dependency mechanism handle the refetch
      dispatch(
        setBreakdownByStoreQueryParams({
          profitYear: profitYear,
          storeNumber: data.store !== null && data.store !== undefined ? data.store : undefined,
          badgeNumber: badgeNumber,
          employeeName: employeeName,
          pagination: {
            take: 25,
            skip: 0,
            sortBy: data.sortBy,
            isSortDescending: data.isSortDescending
          }
        })
      );

      // Trigger refetch in grids - this ensures grids refetch even if queryParams don't change
      if (onSearch) {
        onSearch();
      }
    }
  });

  const handleReset = () => {
    setStoreInputValue("");
    reset({
      store: null,
      employeeStatus: "",
      badgeId: undefined,
      employeeName: "",
      sortBy: "badgeNumber",
      isSortDescending: true
    });
    dispatch(clearBreakdownByStore());
    dispatch(clearBreakdownByStoreManagement());
    dispatch(clearBreakdownByStoreTotals());
    dispatch(clearBreakdownGrandTotals());

    // Clear the query params to remove any filters
    dispatch(
      setBreakdownByStoreQueryParams({
        profitYear: profitYear,
        storeNumber: undefined,
        badgeNumber: undefined,
        employeeName: undefined,
        pagination: {
          take: 25,
          skip: 0,
          sortBy: "badgeNumber",
          isSortDescending: true
        }
      })
    );

    if (onStoreChange) {
      onStoreChange(null);
    }

    // allow parent to clear grids
    if (onReset) {
      onReset();
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
        {isStoresError && (
          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              size="small"
              error
              disabled
              value="Unable to load store information. Please try again later."
              sx={{
                '& .MuiInputBase-root': {
                  backgroundColor: '#ffebee',
                  color: '#d32f2f'
                }
              }}
            />
          </Grid>
        )}
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Store</FormLabel>
          <Controller
            name="store"
            control={control}
            render={({ field }) => (
              <Autocomplete
                size="small"
                fullWidth
                disabled={isStoresError || isStoresLoading}
                options={stores}
                getOptionLabel={(option) => {
                  if (typeof option === "string") return option;
                  if (typeof option === "number") {
                    const found = stores.find(s => s.id === option);
                    return found ? found.label : String(option);
                  }
                  return option.label || "";
                }}
                isOptionEqualToValue={(option, value) => {
                  if (!value) return false;
                  if (typeof value === "number") return option.id === value;
                  return option.id === value.id;
                }}
                value={field.value === null || field.value === undefined || field.value === 0 ? null : 
                  stores.find(s => s.id === field.value) || null}
                inputValue={storeInputValue}
                onInputChange={(_e, newInputValue, reason) => {
                  setStoreInputValue(newInputValue);
                  // When clearing via the clear button or reset, also clear the field value
                  if (reason === "clear") {
                    field.onChange(null);
                    if (onStoreChange) {
                      onStoreChange(null);
                    }
                  }
                }}
                onChange={(_e, newValue) => {
                  // Handle clear/null/blank selection
                  if (newValue === null || newValue === undefined || (typeof newValue === 'object' && newValue.id === 0)) {
                    field.onChange(null);
                    if (onStoreChange) {
                      onStoreChange(null);
                    }
                    return;
                  }

                  // Handle object selection from dropdown
                  const selectedId = typeof newValue === 'object' ? newValue.id : Number(newValue);
                  
                  // Handle "All Stores" selection (value -1)
                  if (selectedId === -1) {
                    field.onChange(-1);
                    // Clear badge and employee name filters when All Stores is selected
                    setValue("badgeId", undefined);
                    setValue("employeeName", "");
                    if (onStoreChange) {
                      onStoreChange(-1);
                    }
                    return;
                  }

                  field.onChange(selectedId);
                  if (onStoreChange) {
                    onStoreChange(selectedId);
                  }
                }}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    variant="outlined"
                    placeholder="Type to search or select"
                  />
                )}
              />
            )}
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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
                  type="number"
                  disabled={store === -1 || store === null}
                  sx={{
                    '& .MuiInputBase-root': {
                      backgroundColor: (store === -1 || store === null) ? '#f5f5f5' : 'white'
                    }
                  }}
                  value={field.value ?? ""}
                  onChange={(e) => {
                    const value = e.target.value;
                    // Convert empty string to undefined, otherwise parse as number
                    const numValue = value === "" || value === null ? undefined : Number(value);
                    field.onChange(numValue === 0 ? undefined : numValue);
                  }}
                  onBlur={(e) => {
                    // On blur, if the field is empty, explicitly set to undefined
                    const value = e.target.value;
                    if (value === "" || value === null || value === undefined) {
                      field.onChange(undefined);
                      // Immediately update Redux to clear the badge filter
                      dispatch(
                        setBreakdownByStoreQueryParams({
                          profitYear: profitYear,
                          storeNumber: getValues("store") ?? undefined,
                          badgeNumber: undefined,
                          employeeName: getValues("employeeName") || undefined,
                          pagination: {
                            take: 25,
                            skip: 0,
                            sortBy: getValues("sortBy"),
                            isSortDescending: getValues("isSortDescending")
                          }
                        })
                      );
                    }
                    field.onBlur();
                  }}
                />
              </>
            )}
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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
                  disabled={store === -1 || store === null}
                  sx={{
                    '& .MuiInputBase-root': {
                      backgroundColor: (store === -1 || store === null) ? '#f5f5f5' : 'white'
                    }
                  }}
                />
              </>
            )}
          />
        </Grid>
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
              isFetching={isLoading || isSubmitting}
              disabled={!prerequisitesComplete || isLoading || isSubmitting || !isStoreInputValid}
            />
          )}
        </DuplicateSsnGuard>
      </Grid>
    </form>
  );
};

export default QPAY066TABreakdownParameters;
