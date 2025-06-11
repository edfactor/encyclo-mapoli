import {
    FormHelperText,
    FormLabel,
    TextField
} from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { BeneficiaryRequestDto } from "reduxstore/types";
import { useDispatch } from "react-redux";
import { useLazyGetBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";
import { setBeneficiaryRequest } from "reduxstore/slices/beneficiarySlice";

const schema = yup.object().shape({
    badgeNumber: yup.string().required(),
    psnSuffix: yup.string().required()
});
interface bRequest{
    badgeNumber:string;
    psnSuffix:string;
}
// Define the type of props
type Props = {
  searchClicked: (badgeNumber:number) => void;
};


// const BeneficiaryInquirySearchFilter: React.FC<BeneficiaryInquirySearchFilterProps> = ({
//   setInitialSearchLoaded,
//   setMissiveAlerts
// }) => {
const BeneficiaryInquirySearchFilter:React.FC<Props> = ({searchClicked}) => {
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
        resolver: yupResolver(schema)
    });
    const onSubmit = (data: any) => {
        const {badgeNumber, psnSuffix} = data;
        searchClicked(badgeNumber);
        if (isValid && data["badgeNumber"]) { 
            const beneficiaryRequestDto: BeneficiaryRequestDto = {
                badgeNumber: badgeNumber,
                psnSuffix: psnSuffix,
                skip:0,
                take:255,
                isSortDescending: true,
                sortBy: "id"
            };
            triggerSearch(beneficiaryRequestDto);
            dispatch(setBeneficiaryRequest(beneficiaryRequestDto));
        }
        console.log({bnumber: badgeNumber, psn: psnSuffix});
    };

    const handleReset = () => {
        reset({ badgeNumber: '', psnSuffix: '' });
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <Grid2
                container
                paddingX="24px">
                <Grid2
                    container
                    spacing={3}
                    width="100%">
                    <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
                        <FormLabel>Badge Number</FormLabel>
                        <Controller
                            name="badgeNumber"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.badgeNumber}
                                    onChange={(e) => {
                                        const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                        field.onChange(parsedValue);
                                    }}
                                />
                            )}
                        />
                        {errors?.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
                    </Grid2>

                    <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
                        <FormLabel>PSN Suffix</FormLabel>
                        <Controller
                            name="psnSuffix"
                            control={control}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    size="small"
                                    variant="outlined"
                                    value={field.value ?? ""}
                                    error={!!errors.psnSuffix}
                                    onChange={(e) => {
                                        field.onChange(e.target.value);
                                    }}
                                />
                            )}
                        />
                        {errors.psnSuffix && <FormHelperText error>{errors.psnSuffix.message}</FormHelperText>}
                    </Grid2>
                </Grid2>

                <Grid2
                    container
                    justifyContent="flex-end"
                    paddingY="16px">
                    <Grid2 size={{ xs: 12 }}>
                        <SearchAndReset
                            handleReset={handleReset}
                            handleSearch={onSubmit}
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