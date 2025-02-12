import { FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";

interface DateRangeSearch {
    startDate: Date;
    endDate: Date;
}

const schema = yup.object().shape({
    startDate: yup
        .date()
        .required("Start Date is required")
        .typeError("Please enter a valid date"),
    endDate: yup
        .date()
        .required("End Date is required")
        .typeError("Please enter a valid date")
        .min(
            yup.ref('startDate'),
            "End Date must be after Start Date"
        )
});

interface DateRangeSearchFilterProps {
    onSearch?: (data: DateRangeSearch) => void;
    isFetching?: boolean;
}

const ProfCtrlSheetSearchFilters: React.FC<DateRangeSearchFilterProps> = ({
    onSearch,
    isFetching = false
}) => {
    const {
        control,
        handleSubmit,
        formState: { errors, isValid },
        reset
    } = useForm<DateRangeSearch>({
        resolver: yupResolver(schema),
        defaultValues: {
            startDate: undefined,
            endDate: undefined
        }
    });

    const validateAndSubmit = handleSubmit((data) => {
        if (isValid && onSearch) {
            onSearch(data);
        }
    });

    const handleReset = () => {
        reset({
            startDate: undefined,
            endDate: undefined
        });
    };

    return (
        <form onSubmit={validateAndSubmit}>
            <Grid2
                container
                paddingX="24px"
                alignItems="flex-end"
                gap="24px">
                <Grid2
                    xs={12}
                    sm={6}
                    md={3}>
                    <Controller
                        name="startDate"
                        control={control}
                        render={({ field }) => (
                            <DsmDatePicker
                                id="startDate"
                                onChange={(value: Date | null) => field.onChange(value)}
                                value={field.value ?? null}
                                required={true}
                                label="Start Date"
                                disableFuture
                                error={errors.startDate?.message}
                            />
                        )}
                    />
                    {errors.startDate && (
                        <FormHelperText error>{errors.startDate.message}</FormHelperText>
                    )}
                </Grid2>

                <Grid2
                    xs={12}
                    sm={6}
                    md={3}>
                    <Controller
                        name="endDate"
                        control={control}
                        render={({ field }) => (
                            <DsmDatePicker
                                id="endDate"
                                onChange={(value: Date | null) => field.onChange(value)}
                                value={field.value ?? null}
                                required={true}
                                label="End Date"
                                disableFuture
                                error={errors.endDate?.message}
                            />
                        )}
                    />
                    {errors.endDate && (
                        <FormHelperText error>{errors.endDate.message}</FormHelperText>
                    )}
                </Grid2>
            </Grid2>

            <Grid2
                width="100%"
                paddingX="24px">
                <SearchAndReset
                    handleReset={handleReset}
                    handleSearch={validateAndSubmit}
                    isFetching={isFetching}
                    disabled={!isValid}
                />
            </Grid2>
        </form>
    );
};

export default ProfCtrlSheetSearchFilters;