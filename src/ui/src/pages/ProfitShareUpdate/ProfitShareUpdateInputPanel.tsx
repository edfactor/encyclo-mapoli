import Grid2 from "@mui/material/Unstable_Grid2";
import {useForm, Controller} from "react-hook-form";
import {
    FormHelperText,
    FormLabel,
    Button,
    TextField
} from "@mui/material";
import {
    useLazyGetProfitShareUpdateQuery,
} from "reduxstore/api/YearsEndApi";
import {yupResolver} from "@hookform/resolvers/yup";
import * as yup from "yup";
import {useDispatch, useSelector} from "react-redux";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import {clearMasterInquiryData, clearProfitUpdate} from "../../reduxstore/slices/yearsEndSlice";

interface ProfitShareUpdateInputPanelProps {
    startProfitYear?: Date | null;
    contributionPercent?: number | null | undefined;
    earningsPercent?: number | null;
    incomingForfeiturePercent?: number | null;
    maxAllowedContributions: number | null;

    adjustmentBadge?: number | null;
    adjustmentContributionAmount?: number | null;
    adjustmentEarningsAmount?: number | null;
    adjustmentIncomingForfeitureAmount?: number | null;

    adjustmentSecondaryBadge?: number | null;
    adjustmentSecondaryEarningsAmount?: number | null;
}

const schema = yup.object().shape({
    startProfitYear: yup
        .date()
        .min(new Date(2020, 0, 1), "Year must be 2020 or later")
        .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
        .typeError("Invalid date")
        .nullable(),
    contributionPercent: yup
        .number()
        .typeError("Contribution must be a number")
        .min(0, "Contribution must be positive")
        .nullable(),
    earningsPercent: yup.number().typeError("Earnings must be a number").min(0, "Earnings must be positive").nullable(),
    incomingForfeiturePercent: yup.number().typeError("Incoming Forfeiture must be a number").min(0, "Forfeiture must be positive").nullable(),
    maxAllowedContributions: yup.number().typeError("Max Allowed Contributions must be a number").min(0, "Max Allowed Contributions must be positive").nullable(),
    adjustmentBadge: yup
        .number()
        .typeError("Badge must be a number")
        .integer("Badge must be an integer")
        .nullable(),
    adjustmentContributionAmount: yup
        .number()
        .typeError("Contribution must be a number")
        .min(0, "Contribution must be positive")
        .nullable(),
    adjustmentEarningsAmount: yup
        .number()
        .typeError("Earnings must be a number")
        .nullable(),
    adjustmentIncomingForfeitureAmount: yup
        .number()
        .typeError("Adjusted Incoming Forfeiture must be a number")
        .min(0, "Adjusted Incoming Forfeiture must be positive")
        .nullable(),
    adjustmentSecondaryBadge: yup
        .number()
        .typeError("Badge must be a number")
        .integer("Badge must be an integer")
        .nullable(),
    adjustmentSecondaryEarningsAmount: yup
        .number()
        .typeError("Earnings must be a number")
        .nullable()
});

const ProfitShareUpdateInputPanel = () => {
    const [triggerView, { isFetching }] = useLazyGetProfitShareUpdateQuery();
    const dispatch = useDispatch();

    const {
        control,
        handleSubmit,
        formState: {errors, isValid},
    } = useForm<ProfitShareUpdateInputPanelProps>({
        resolver: yupResolver(schema),
        defaultValues: {
            startProfitYear: new Date(2024, 1, 1),
            contributionPercent: 1.1,
            earningsPercent: 2.2,
            incomingForfeiturePercent: 3.3,
            maxAllowedContributions: 60000,

            adjustmentBadge: 4567,
            adjustmentContributionAmount: 23.44,
            adjustmentEarningsAmount: 22.55,
            adjustmentIncomingForfeitureAmount: 1.22,

            adjustmentSecondaryBadge: 898989,
            adjustmentSecondaryEarningsAmount: 1.1,
        }
    });

    const validateAndView = handleSubmit((data) => {
        if (isValid) {
            const viewParams: ProfitShareUpdateInputPanelProps = {
                ...(!!data.startProfitYear && {profitYear: data.startProfitYear.getFullYear()}),
                ...(!!data.contributionPercent && {contributionPercent: data.contributionPercent}),
                ...(!!data.earningsPercent && {earningsPercent: data.earningsPercent}),
                ...(!!data.incomingForfeiturePercent && {incomingForfeitPercent: data.incomingForfeiturePercent}),
                ...(!!data.maxAllowedContributions && {maxAllowedContributions: data.maxAllowedContributions}),

                ...(!!data.adjustmentBadge && {badgeToAdjust: data.adjustmentBadge}),
                ...(!!data.adjustmentContributionAmount && {adjustmentContributionAmount: data.adjustmentContributionAmount}),
                ...(!!data.adjustmentEarningsAmount && {adjustmentEarningsAmount: data.adjustmentEarningsAmount}),
                ...(!!data.incomingForfeiturePercent && {incomingForfeitPercent: data.incomingForfeiturePercent}),

                ...(!!data.adjustmentSecondaryBadge && {badgeToAdjust2: data.adjustmentSecondaryBadge}),
                ...(!!data.adjustmentSecondaryEarningsAmount && {adjustEarningsSecondaryAmount: data.adjustmentSecondaryEarningsAmount}),

            };
            // clears current table data
            dispatch(clearProfitUpdate());
            triggerView(viewParams, false);
        }
    });

    return (
        <form onSubmit={validateAndView}>
            <Grid2 container paddingX="24px">
                <Grid2 container spacing={3} width="100%">
                    <Grid2 xs={12} sm={6} md={2}>
                        <Controller
                            name="startProfitYear"
                            control={control}
                            render={({field}) => (
                                <DsmDatePicker
                                    id="Beginning Year"
                                    onChange={(value: Date | null) => field.onChange(value)}
                                    value={field.value ?? null}
                                    required={true}
                                    label="Profit Year"
                                    disableFuture
                                    views={["year"]}
                                    error={errors.startProfitYear?.message}
                                />
                            )}
                        />
                        {errors.startProfitYear &&
                            <FormHelperText error>{errors.startProfitYear.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Contribution %</FormLabel>
                        <Controller
                            name="contributionPercent"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.contributionPercent}
                                />
                            )}
                        />
                        {errors.contributionPercent && <FormHelperText error>{errors.contributionPercent.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Earnings %</FormLabel>
                        <Controller
                            name="earningsPercent"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.earningsPercent}
                                />
                            )}
                        />
                        {errors.earningsPercent && <FormHelperText error>{errors.earningsPercent.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Incoming Forfeiture %</FormLabel>
                        <Controller
                            name="incomingForfeiturePercent"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.incomingForfeiturePercent}
                                />
                            )}
                        />
                        {errors.incomingForfeiturePercent &&
                            <FormHelperText error>{errors.incomingForfeiturePercent.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Max Allowed Contributions</FormLabel>
                        <Controller
                            name="maxAllowedContributions"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.maxAllowedContributions}
                                />
                            )}
                        />
                        {errors.maxAllowedContributions &&
                            <FormHelperText error>{errors.maxAllowedContributions.message}</FormHelperText>}
                    </Grid2>
                </Grid2>

                <Grid2 container spacing={3} width="100%">
                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Adjustment Badge</FormLabel>
                        <Controller
                            name="adjustmentBadge"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.adjustmentBadge}
                                />
                            )}
                        />
                        {errors.adjustmentBadge &&
                            <FormHelperText error>{errors.adjustmentBadge.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Adjust Contribution Amount</FormLabel>
                        <Controller
                            name="adjustmentContributionAmount"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.adjustmentContributionAmount}
                                />
                            )}
                        />
                        {errors.adjustmentContributionAmount && <FormHelperText error>{errors.adjustmentContributionAmount.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Adjust Earnings Amount</FormLabel>
                        <Controller
                            name="adjustmentEarningsAmount"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.adjustmentEarningsAmount}
                                />
                            )}
                        />
                        {errors.adjustmentEarningsAmount && <FormHelperText error>{errors.adjustmentEarningsAmount.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Adjust Incoming Forfeiture Amount</FormLabel>
                        <Controller
                            name="adjustmentIncomingForfeitureAmount"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.adjustmentIncomingForfeitureAmount}
                                />
                            )}
                        />
                        {errors.adjustmentIncomingForfeitureAmount &&
                            <FormHelperText error>{errors.adjustmentIncomingForfeitureAmount.message}</FormHelperText>}
                    </Grid2>

                </Grid2>

                <Grid2 container spacing={3} width="100%">
                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Adjust Secondary Badge</FormLabel>
                        <Controller
                            name="adjustmentSecondaryBadge"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.adjustmentSecondaryBadge}
                                />
                            )}
                        />
                        {errors.adjustmentSecondaryBadge &&
                            <FormHelperText error>{errors.adjustmentSecondaryBadge.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 xs={12} sm={6} md={2}>
                        <FormLabel>Adjust Secondary Earnings Amount</FormLabel>
                        <Controller
                            name="adjustmentSecondaryEarningsAmount"
                            control={control}
                            render={({field}) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ''}
                                    error={!!errors.adjustmentSecondaryEarningsAmount}
                                />
                            )}
                        />
                        {errors.adjustmentSecondaryEarningsAmount && <FormHelperText error>{errors.adjustmentSecondaryEarningsAmount.message}</FormHelperText>}
                    </Grid2>

                </Grid2>
                <Grid2 xs={12} sm={6} md={4} className="mt-4">
                    <div className="flex gap-4">
                        <Button variant="contained" type="submit" onClick={validateAndView}>
                            View Updates
                        </Button>
                        <Button disabled variant="contained" type="submit" onClick={validateAndView}>
                            View Details
                        </Button>
                        <Button disabled variant="contained" type="submit" onClick={validateAndView}>
                            Apply
                        </Button>
                        <Button disabled variant="contained" type="submit" onClick={validateAndView}>
                            Revert
                        </Button>
                    </div>
                </Grid2>
            </Grid2>
        </form>
    );
};


export default ProfitShareUpdateInputPanel;
