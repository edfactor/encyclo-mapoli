import {
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  MenuItem,
  Radio,
  RadioGroup,
  Select,
  TextField
} from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { MasterInquiryRequest, MasterInquirySearch, MissiveResponse } from "reduxstore/types";
import {
  clearMasterInquiryData,
  clearMasterInquiryRequestParams,
  setMasterInquiryRequestParams
} from "reduxstore/slices/inquirySlice";
import { useDispatch, useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import { RootState } from "reduxstore/store";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";

const schema = yup.object().shape({
  badgeNumber: yup.string().required(),
  psnSuffix: yup.string().required()
});

interface BeneficiaryInquirySearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  setMissiveAlerts: (alerts: MissiveResponse[]) => void;
}

// const BeneficiaryInquirySearchFilter: React.FC<BeneficiaryInquirySearchFilterProps> = ({
//   setInitialSearchLoaded,
//   setMissiveAlerts
// }) => {
    const BeneficiaryInquirySearchFilter= () => {


    const {
        control,
        register,
        formState: { errors, isValid },
        setValue,
        handleSubmit,
        reset,
        setFocus,
        watch
      } = useForm({
        resolver: yupResolver(schema)
      });
      const onSubmit = (data:any) => {
        console.log(data)
    };

      const handleReset = ()=>{
        reset({badgeNumber: '', psnSuffix: ''});
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
              isFetching= {false}
              disabled={!isValid}
            /> 
          </Grid2>
        </Grid2>
      </Grid2>
    </form>
  );
};

export default BeneficiaryInquirySearchFilter;