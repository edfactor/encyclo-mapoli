import { MenuItem, Select, TextField, FormLabel, Checkbox, FormControlLabel } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useForm, Controller, useWatch } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import { setBreakdownByStoreQueryParams } from "reduxstore/slices/yearsEndSlice";

interface BreakdownSearchParams {
    store?: string;
    employeeStatus?: string;
    badgeId?: string;
    employeeName?: string;
    under21Only?: boolean;
    sortBy?: string;
    isSortDescending?: boolean;
}

interface OptionItem {
    id: string;
    label: string;
}

const schema = yup.object().shape({
    store: yup.string(),
    employeeStatus: yup.string(),
    badgeId: yup.string(),
    employeeName: yup.string(),
    under21Only: yup.boolean(),
    sortBy: yup.string(),
    isSortDescending: yup.boolean()
});

interface QPAY066TABreakdownParametersProps {
    activeTab: 'all' | 'stores' | 'summaries' | 'totals';
    onStoreChange?: (store: string) => void;
}

const QPAY066TABreakdownParameters: React.FC<QPAY066TABreakdownParametersProps> = ({
    activeTab,
    onStoreChange
}) => {
    const dispatch = useDispatch();
    
    const [employeeStatuses] = useState<OptionItem[]>([
        { id: '700', label: '700 - Retired - Drawing Pension' },
        { id: '701', label: '701 - Active - Drawing Pension' },
        { id: '800', label: '800 - Terminated' },
        { id: '801', label: '801 - Terminated w/ Zero Balance' },
        { id: '802', label: '802 - Terminated w/ Balance but No Vesting' },
        { id: '900', label: '900 - Monthly Payroll' }
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
            store: '700',
            employeeStatus: '',
            badgeId: '',
            employeeName: '',
            under21Only: false,
            sortBy: 'badgeNumber',
            isSortDescending: true
        }
    });

    const employeeStatus = useWatch({
        control,
        name: "employeeStatus"
    });

    useEffect(() => {
        if (employeeStatus && employeeStatus !== '') {
            setValue('store', employeeStatus);
            if (onStoreChange) {
                onStoreChange(employeeStatus);
            }
        }
    }, [employeeStatus, setValue, onStoreChange]);

    const validateAndSubmit = handleSubmit((data) => {
        if (isValid) {
            if (onStoreChange && data.store) {
                onStoreChange(data.store);
            }
            
            dispatch(setBreakdownByStoreQueryParams({
                profitYear: 2024,
                storeNumber: data.store,
                under21Only: data.under21Only,
                isSortDescending: data.isSortDescending,
                pagination: {
                    take: 255,
                    skip: 0
                }
            }));
        }
    });

    const handleReset = () => {
        reset({
            store: '700',
            employeeStatus: '',
            badgeId: '',
            employeeName: '',
            under21Only: false,
            sortBy: 'badgeNumber',
            isSortDescending: true
        });
    };

    const getGridSizes = () => {
        if (activeTab === 'all' || activeTab === 'stores') {
            return { xs: 12, sm: 6, md: 3 };
        } else {
            return { xs: 12, sm: 6, md: 4 };
        }
    };

    return (
        <form onSubmit={validateAndSubmit}>
            <Grid2
                container
                paddingX="24px"
                alignItems="flex-end"
                spacing={1.5}>
                <Grid2 size={getGridSizes()}>
                    <Controller
                        name="store"
                        control={control}
                        render={({ field }) => (
                            <>
                                <FormLabel htmlFor="store-input" sx={{ display: 'block', marginBottom: '8px' }}>
                                    Store
                                </FormLabel>
                                <TextField
                                    id="store-input"
                                    {...field}
                                    size="small"
                                    fullWidth
                                    onChange={(e) => {
                                        field.onChange(e);
                                        if (onStoreChange) {
                                            onStoreChange(e.target.value);
                                        }
                                    }}
                                />
                            </>
                        )}
                    />
                </Grid2>

                {(activeTab === 'all' || activeTab === 'stores') && (
                    <>
                        <Grid2 size={getGridSizes()}>
                            <Controller
                                name="employeeStatus"
                                control={control}
                                render={({ field }) => (
                                    <>
                                        <FormLabel htmlFor="status-select">
                                            Employee Status
                                        </FormLabel>
                                        <Select
                                            id="status-select"
                                            {...field}
                                            size="small"
                                            fullWidth
                                        >
                                            <MenuItem value="">
                                                <em>All</em>
                                            </MenuItem>
                                            {employeeStatuses.map((status) => (
                                                <MenuItem key={status.id} value={status.id}>
                                                    {status.label}
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </>
                                )}
                            />
                        </Grid2>

                        <Grid2 size={getGridSizes()}>
                            <Controller
                                name="badgeId"
                                control={control}
                                render={({ field }) => (
                                    <>
                                        <FormLabel htmlFor="badge-id">
                                            Badge Number
                                        </FormLabel>
                                        <TextField
                                            id="badge-id"
                                            {...field}
                                            size="small"
                                            fullWidth
                                        />
                                    </>
                                )}
                            />
                        </Grid2>

                        <Grid2 size={getGridSizes()}>
                            <Controller
                                name="employeeName"
                                control={control}
                                render={({ field }) => (
                                    <>
                                        <FormLabel htmlFor="employee-name">
                                            Employee Name
                                        </FormLabel>
                                        <TextField
                                            id="employee-name"
                                            {...field}
                                            size="small"
                                            fullWidth
                                        />
                                    </>
                                )}
                            />
                        </Grid2>
                        
                        <Grid2 size={getGridSizes()}>
                            <Controller
                                name="under21Only"
                                control={control}
                                render={({ field }) => (
                                    <FormControlLabel
                                        control={
                                            <Checkbox
                                                {...field}
                                                checked={field.value}
                                            />
                                        }
                                        label="Under 21 Only"
                                    />
                                )}
                            />
                        </Grid2>
                    </>
                )}
            </Grid2>

            <Grid2
                width="100%"
                paddingX="24px"
                marginTop={2}>
                <SearchAndReset
                    handleReset={handleReset}
                    handleSearch={validateAndSubmit}
                    isFetching={false}
                    disabled={false}
                />
            </Grid2>
        </form>
    );
};

export default QPAY066TABreakdownParameters; 