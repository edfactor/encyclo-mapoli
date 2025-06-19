import {
    FormHelperText,
    FormLabel,
    MenuItem,
    Select,
    TextField
} from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm, Resolver } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { BeneficiaryKindDto, BeneficiaryRequestDto, BeneficiaryTypeDto } from "reduxstore/types";
import { useDispatch } from "react-redux";
import { useLazyGetBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";
import { setBeneficiaryRequest } from "reduxstore/slices/beneficiarySlice";
import { ElectricScooterRounded } from "@mui/icons-material";

const schema = yup.object().shape({
    badgePsn: yup.string().notRequired(),
    name: yup.string().notRequired(),
    address: yup.string().notRequired(),
    city: yup.string().notRequired(),
    state: yup.string().notRequired(),
    ssn: yup.number().notRequired(),
    percentage: yup.number().notRequired(),
    kindId: yup.string().notRequired()
});
interface bRequest {
    badgePsn?:string;
    name: string;
    address: string;
    city: string;
    state: string;
    ssn: number;
    percentage:number;
    kindId: string;
}
// Define the type of props
type Props = {
    beneficiaryKind: BeneficiaryKindDto[],
  searchClicked: (badgeNumber:number) => void;
};


// const BeneficiaryInquirySearchFilter: React.FC<BeneficiaryInquirySearchFilterProps> = ({
//   setInitialSearchLoaded,
//   setMissiveAlerts
// }) => {
const BeneficiaryInquirySearchFilter:React.FC<Props> = ({searchClicked,beneficiaryKind}) => {
    const [triggerSearch, {data,isLoading,isError,isFetching}] = useLazyGetBeneficiariesQuery();
    const dispatch = useDispatch();

    const {
        control,
        register,
        formState: { errors, isValid },
        setValue,
        handleSubmit,
        reset,
        setFocus,
        watch
    } = useForm<bRequest>({
        resolver: yupResolver(schema) as Resolver<bRequest>
    });

    const onSubmit = (data: any) => {
        const { badgePsn, name, ssn, address, city, state,percentage } = data;

        const badge= badgePsn?parseInt(badgePsn.slice(0, -4)): 0;
        const psn = badgePsn?parseInt(badgePsn.slice(-4)): 0;
        searchClicked(badge);
        if (isValid) {
            const beneficiaryRequestDto: BeneficiaryRequestDto = {
                badgeNumber: badge??0,
                psnSuffix: psn??0,
                name: name,
                ssn: ssn,
                address: address,
                city: city,
                state: state,
                percentage: percentage??0,
                skip: 0,
                take: 255,
                isSortDescending: true,
                sortBy: "id"
            };
            triggerSearch(beneficiaryRequestDto);
            dispatch(setBeneficiaryRequest(beneficiaryRequestDto));
        }
    };
    const validateAndSubmit = handleSubmit(onSubmit);

    const handleReset = () => {
        reset({ badgePsn:undefined, address: undefined, city: undefined, name: undefined, ssn: undefined, state: undefined });
    }

    return (
        <form onSubmit={validateAndSubmit}>
            <Grid2
                container
                paddingX="24px">
                <Grid2
                    container
                    spacing={2}
                    width="100%">
                    <Grid2 size={{ xs: 12, sm: 3, md: 3 }}>
                        <FormLabel>Badge/Psn</FormLabel>
                        <Controller
                            name="badgePsn"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.badgePsn}
                                    onChange={(e) => {
                                        const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                        field.onChange(parsedValue);
                                    }}
                                />
                            )}
                        />
                        {errors?.badgePsn && <FormHelperText error>{errors.badgePsn.message}</FormHelperText>}
                    </Grid2>

                    
                    <Grid2 size={{ xs: 12, sm: 3, md: 3 }}>
                        <FormLabel>Name</FormLabel>
                        <Controller
                            name="name"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.name}
                                    onChange={(e) => {
                                        field.onChange(e.target.value);
                                    }}
                                />
                            )}
                        />
                        {errors?.name && <FormHelperText error>{errors.name.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 size={{ xs: 12, sm: 3, md: 3 }}>
                        <FormLabel>SSN</FormLabel>
                        <Controller
                            name="ssn"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.ssn}
                                    onChange={(e) => {
                                        const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                        field.onChange(parsedValue);
                                    }}
                                />
                            )}
                        />
                        {errors?.ssn && <FormHelperText error>{errors.ssn.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
                        <FormLabel>Address</FormLabel>
                        <Controller
                            name="address"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.address}
                                    onChange={(e) => {
                                        field.onChange(e.target.value);
                                    }}
                                />
                            )}
                        />
                        {errors?.address && <FormHelperText error>{errors.address.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 size={{ xs: 12, sm: 2, md: 2 }}>
                        <FormLabel>City</FormLabel>
                        <Controller
                            name="city"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.city}
                                    onChange={(e) => {
                                        field.onChange(e.target.value);
                                    }}
                                />
                            )}
                        />
                        {errors?.city && <FormHelperText error>{errors.city.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 size={{ xs: 12, sm: 1, md: 1 }}>
                        <FormLabel>State</FormLabel>
                        <Controller
                            name="state"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.state}
                                    onChange={(e) => {
                                        field.onChange(e.target.value);
                                    }}
                                />
                            )}
                        />
                        {errors?.state && <FormHelperText error>{errors.state.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 size={{ xs: 12, sm: 1, md: 1 }}>
                        <FormLabel>Percentage</FormLabel>
                        <Controller
                            name="percentage"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.percentage}
                                    onChange={(e) => {
                                        const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                        field.onChange(parsedValue);
                                    }}
                                />
                            )}
                        />
                        {errors?.percentage && <FormHelperText error>{errors.percentage.message}</FormHelperText>}
                    </Grid2>
                    <Grid2 size={{xs:12, sm:2, md:2}}>
                            <FormLabel>Beneficiary Kind</FormLabel>
                            <Controller
                                name="kindId"
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        {...field}
                                        fullWidth
                                        size="small"
                                        variant="outlined"
                                        labelId="kindId"
                                        id="kindId"
                                        value={field.value}
                                        label="Beneficiary Kind"
                                        onChange={(e) => field.onChange(e.target.value)}
                                    >
                                        {beneficiaryKind.map((d) => (
                                            <MenuItem value={d.id}>{d.name}</MenuItem>
                                        ))}
                                    </Select>
                                )}
                            />
                    </Grid2>
                </Grid2>


                <Grid2
                    container
                    justifyContent="flex-end"
                    paddingY="16px">
                    <Grid2 size={{ xs: 12 }}>
                        <SearchAndReset
                            handleReset={handleReset}
                            handleSearch={validateAndSubmit}
                            isFetching={isFetching}
                            disabled={!isValid}
                        />
                    </Grid2>
                </Grid2>
            </Grid2>
        </form>
    );
};

export default BeneficiaryInquirySearchFilter;