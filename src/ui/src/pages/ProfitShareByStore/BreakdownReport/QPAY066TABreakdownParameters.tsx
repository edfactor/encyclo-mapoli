import { MenuItem, Select, TextField, FormLabel } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { useState } from "react";

interface BreakdownSearchParams {
    store?: string;
    employeeStatus?: string;
    badgeId?: string;
    employeeName?: string;
}

interface OptionItem {
    id: string;
    label: string;
}

const schema = yup.object().shape({
    store: yup.string(),
    employeeStatus: yup.string(),
    badgeId: yup.string(),
    employeeName: yup.string()
});

interface QPAY066TABreakdownParametersProps {
    activeTab: 'all' | 'stores' | 'summaries' | 'totals';
    onStoreChange?: (store: string) => void;
}

const QPAY066TABreakdownParameters: React.FC<QPAY066TABreakdownParametersProps> = ({
    activeTab,
    onStoreChange
}) => {
    const [stores] = useState<OptionItem[]>([
        { id: '3', label: '3 - [City, State]' },
        { id: '4', label: '4 - [City, State]' },
        { id: '5', label: '5 - [City, State]' },
        { id: '6', label: '6 - [City, State]' },
        { id: '7', label: '7 - [City, State]' },
        { id: '8', label: '8 - [City, State]' }
    ]);
    
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
        reset
    } = useForm<BreakdownSearchParams>({
        resolver: yupResolver(schema),
        defaultValues: {
            store: '',
            employeeStatus: '',
            badgeId: '',
            employeeName: ''
        }
    });

    const validateAndSubmit = handleSubmit((data) => {
        if (isValid) {
            if (onStoreChange && data.store) {
                onStoreChange(data.store);
            }
        }
    });

    const handleReset = () => {
        reset({
            store: '3',
            employeeStatus: '700',
            badgeId: '',
            employeeName: ''
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
                                <FormLabel htmlFor="store-select" sx={{ display: 'block', marginBottom: '8px' }}>
                                    Store
                                </FormLabel>
                                <Select
                                    id="store-select"
                                    {...field}
                                    size="small"
                                    fullWidth
                                    onChange={(e) => {
                                        field.onChange(e);
                                        if (onStoreChange) {
                                            onStoreChange(e.target.value as string);
                                        }
                                    }}
                                >
                                    {stores.map((store) => (
                                        <MenuItem key={store.id} value={store.id}>
                                            {store.label}
                                        </MenuItem>
                                    ))}
                                </Select>
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