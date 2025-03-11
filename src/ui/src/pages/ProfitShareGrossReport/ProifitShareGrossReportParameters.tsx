import { FormHelperText, TextField, FormLabel } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";

interface GrossReportParams {
    yDate: Date;
    gross?: string;
}

const schema = yup.object().shape({
    yDate: yup
        .date()
        .required("Year Date is required")
        .typeError("Please enter a valid date"),
    gross: yup.string().optional()
});

const ProfitShareGrossReportParameters = () => {
    const {
        control,
        handleSubmit,
        formState: { errors },
        reset
    } = useForm<GrossReportParams>({
        resolver: yupResolver(schema),
        defaultValues: {
            yDate: undefined,
            gross: ''
        }
    });

    const validateAndSubmit = handleSubmit((data) => {
        console.log('Create data:', data);
    });

    const handleReset = () => {
        reset({
            yDate: undefined,
            gross: ''
        });
    };

    return (
        <form onSubmit={validateAndSubmit}>
            <Grid2
                container
                paddingX="24px"
                alignItems="flex-start"
                gap="24px">
                <Grid2 size={{ xs: 12, sm: 6, md: 2 }} >
                    <Controller
                        name="yDate"
                        control={control}
                        render={({ field }) => (
                            <DsmDatePicker
                                id="yDate"
                                onChange={(value: Date | null) => field.onChange(value)}
                                value={field.value}
                                disableFuture
                                label="YDate"
                                required
                                error={errors.yDate?.message}
                            />
                        )}
                    />
                    {errors.yDate && (
                        <FormHelperText error>{errors.yDate.message}</FormHelperText>
                    )}
                </Grid2>

                <Grid2 size={{ xs: 12, sm: 6, md: 2 }} >
                    <FormLabel>Gross</FormLabel>
                    <Controller
                        name="gross"
                        control={control}
                        render={({ field }) => (
                            <TextField
                                {...field}
                                fullWidth
                                error={!!errors.gross}
                                helperText={errors.gross?.message}
                            />
                        )}
                    />
                </Grid2>
            </Grid2>

            <Grid2
                width="100%"
                paddingX="24px">
                <SearchAndReset
                    handleReset={handleReset}
                    handleSearch={validateAndSubmit}
                    isFetching={false}
                />
            </Grid2>
        </form>
    );
};

export default ProfitShareGrossReportParameters;