import { FormHelperText, TextField, FormLabel } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useForm, Controller } from "react-hook-form";
import { useDispatch} from "react-redux";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetGrossWagesReportQuery } from "reduxstore/api/YearsEndApi";
import DsmDatePicker from "../../components/DsmDatePicker/DsmDatePicker";
import { setGrossWagesReportQueryParams } from "reduxstore/slices/yearsEndSlice";

interface GrossReportParams {
    profitYear: number;
    gross?: number;
}

const schema = yup.object().shape({
    profitYear: yup
        .number()
        .typeError("Year must be a number")
        .integer("Year must be an integer")
        .min(2020, "Year must be 2020 or later")
        .max(2100, "Year must be 2100 or earlier")
        .required("Year is required"),
    gross: yup.number().optional()
});

interface ProfitShareGrossReportParametersProps {
    setPageReset: (reset: boolean) => void;
}

const ProfitShareGrossReportParameters: React.FC<ProfitShareGrossReportParametersProps> = ({ setPageReset }) => {
    const fiscalCloseProfitYear = useFiscalCloseProfitYear();
    const dispatch = useDispatch();
    const [triggerSearch, { isFetching }] = useLazyGetGrossWagesReportQuery();
    const {
        control,
        handleSubmit,
        formState: { errors, isValid },
        reset
    } = useForm<GrossReportParams>({
        resolver: yupResolver(schema),
        defaultValues: {
            profitYear: fiscalCloseProfitYear,
            gross: 50000
        }
    });

    const validateAndSubmit = handleSubmit((data) => {
        if (isValid) {
            setPageReset(true);
            triggerSearch({
                profitYear: data.profitYear,
                minGrossAmount: data.gross,
                pagination: { skip: 0, take: 25 }
            },false).unwrap();
            dispatch(setGrossWagesReportQueryParams({
                profitYear: data.profitYear,
                minGrossAmount: data.gross || 0,
            }))
        }
    });

    const handleReset = () => {
        setPageReset(true);
        reset({
            profitYear: fiscalCloseProfitYear,
            gross: 50000
        });
    };

    return (
        <form onSubmit={validateAndSubmit}>
            <Grid2
                container
                paddingX="24px"
                alignItems="flex-start"
                gap="24px">
            <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
                <Controller
                    name="profitYear"
                    control={control}
                    render={({ field }) => (
                    <DsmDatePicker
                        id="profitYear"
                        onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
                        value={field.value ? new Date(field.value, 0) : null}
                        required={true}
                        label="Profit Year"
                        disableFuture
                        views={["year"]}
                        error={errors.profitYear?.message}
                    />
                    )}
                />
                {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
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
                    isFetching={isFetching}
                    disabled={!isValid}
                />
            </Grid2>
        </form>
    );
};

export default ProfitShareGrossReportParameters;