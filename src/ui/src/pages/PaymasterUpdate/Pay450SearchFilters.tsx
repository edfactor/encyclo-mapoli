import { FormHelperText } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

interface ProfitYearSearch {
    profitYear: number;
}

const schema = yup.object().shape({
    profitYear: yup
        .number()
        .required("Profit Year is required")
        .typeError("Please enter a valid year")
        .min(2000, "Year must be 2000 or later")
        .max(2100, "Year must be 2100 or earlier")
});

interface ProfitYearSearchFilterProps {
    onSearch?: (data: ProfitYearSearch) => void;
    isFetching?: boolean;
}

const Pay450SearchFilters: React.FC<ProfitYearSearchFilterProps> = ({
    onSearch,
    isFetching = false
}) => {
    const fiscalCloseProfitYear = useFiscalCloseProfitYear();
    
    const {
        control,
        handleSubmit,
        formState: { errors, isValid },
        reset
    } = useForm<ProfitYearSearch>({
        resolver: yupResolver(schema),
        defaultValues: {
            profitYear: fiscalCloseProfitYear
        }
    });

    const validateAndSubmit = handleSubmit((data) => {
        if (isValid && onSearch) {
            onSearch(data);
        }
    });

    const handleReset = () => {
        reset({
            profitYear: fiscalCloseProfitYear
        });
    };

    return (
        <form onSubmit={validateAndSubmit}>
            <Grid2
                container
                paddingX="24px"
                alignItems="flex-end"
                gap="24px">
                <Grid2 size={{ xs: 12, sm: 6, md: 3 }} >
                    <Controller
                        name="profitYear"
                        control={control}
                        render={({ field }) => (
                            <DsmDatePicker
                                id="profitYear"
                                onChange={(value: Date | null) => field.onChange(value?.getFullYear() || null)}
                                value={field.value ? new Date(field.value, 0) : null}
                                required={true}
                                label="Profit Year"
                                disableFuture
                                views={["year"]}
                                error={errors.profitYear?.message}
                                disabled={true}
                            />
                        )}
                    />
                    {errors.profitYear && (
                        <FormHelperText error>{errors.profitYear.message}</FormHelperText>
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

export default Pay450SearchFilters;