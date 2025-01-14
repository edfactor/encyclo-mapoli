import { Button, FormHelperText, FormLabel, TextField, Typography } from "@mui/material";
import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import MilitaryAndRehireEntryAndModificationEmployeeDetails from "./MilitaryAndRehireEntryAndModificationEmployeeDetails";
import Grid2 from "@mui/material/Unstable_Grid2";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { Controller, useForm } from "react-hook-form";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";

interface MilitaryAndRehireContributions {
  firstRowContributionDate: Date;
  firstRowContributionAmount: string;
  secondRowContributionDate: Date;
  secondRowContributionAmount: string;
  thirdRowContributionDate: Date;
  thirdRowContributionAmount: string;
  fourthRowContributionDate: Date;
  fourthRowContributionAmount: string;
}

const schema = yup.object().shape({
  firstRowContributionDate: yup.date().required(),
  firstRowContributionAmount: yup.string().required("Amount is required"),
  secondRowContributionDate: yup.date().required(),
  secondRowContributionAmount: yup.string().required("Amount is required"),
  thirdRowContributionDate: yup.date().required(),
  thirdRowContributionAmount: yup.string().required("Amount is required"),
  fourthRowContributionDate: yup.date().required(),
  fourthRowContributionAmount: yup.string().required("Amount is required")
});

const MilitaryAndRehireEntryAndModificationGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  // Temporarily using the masterInquiryDetails until we have a dedicated endpoint for this page.
  const { militaryAndRehireEntryAndModification, masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNegativeEVTASSNQuery();

  const { control, handleSubmit, formState: { errors, isValid }, reset } = useForm<MilitaryAndRehireContributions>({
    resolver: yupResolver(schema),
    defaultValues: {
      firstRowContributionDate: undefined,
      firstRowContributionAmount: "",
      secondRowContributionDate: undefined,
      secondRowContributionAmount: "",
      thirdRowContributionDate: undefined,
      thirdRowContributionAmount: "",
      fourthRowContributionDate: undefined,
      fourthRowContributionAmount: ""
    }
  });

  const onSubmit = (data: MilitaryAndRehireContributions) => {
    console.log(data);
    // TODO: Handle form submission
  };

  const handleCancel = () => {
    reset();
  };

  return (
    <>
      {!!masterInquiryEmployeeDetails && (
        <>
          <MilitaryAndRehireEntryAndModificationEmployeeDetails details={masterInquiryEmployeeDetails} />
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography variant="h2" sx={{ color: "#0258A5" }}>
              Profit Sharing Military (008-13)
            </Typography>
          </div>
          <form onSubmit={handleSubmit(onSubmit)}>
            <Grid2 container spacing={3} paddingX="24px">
              <Grid2 container xs={12} spacing={2}>
                <Grid2 xs={3}>
                  <Controller
                    name="firstRowContributionDate"
                    control={control}
                    render={({ field }) => (
                      <DsmDatePicker
                        id="firstRowContributionDate"
                        label="Contribution Date"
                        onChange={(value: Date | null) => field.onChange(value)}
                        value={field.value ?? null}
                        error={errors.firstRowContributionDate?.message} required={false} />
                    )}
                  />
                </Grid2>
                <Grid2 xs={3}>
                  <FormLabel>Contribution Amount</FormLabel>
                  <Controller
                    name="firstRowContributionAmount"
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        variant="outlined"
                        error={!!errors.firstRowContributionAmount}
                        onChange={(e) => {
                          field.onChange(e);
                        }}
                        inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                      />
                    )}
                  />
                  {errors.firstRowContributionAmount && (
                    <FormHelperText error>{errors.firstRowContributionAmount.message}</FormHelperText>
                  )}
                </Grid2>
              </Grid2>
              <Grid2 container xs={12} spacing={2}>
                <Grid2 xs={3}>
                  <Controller
                    name="secondRowContributionDate"
                    control={control}
                    render={({ field }) => (
                      <DsmDatePicker
                        id="secondRowContributionDate"
                        label="Contribution Date"
                        onChange={(value: Date | null) => field.onChange(value)}
                        value={field.value ?? null}
                        error={errors.secondRowContributionDate?.message} required={false} />
                    )}
                  />
                </Grid2>
                <Grid2 xs={3}>
                  <FormLabel>Contribution Amount</FormLabel>
                  <Controller
                    name="secondRowContributionAmount"
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        variant="outlined"
                        error={!!errors.secondRowContributionAmount}
                        onChange={(e) => {
                          field.onChange(e);
                        }}
                        inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                      />
                    )}
                  />
                  {errors.secondRowContributionAmount && (
                    <FormHelperText error>{errors.secondRowContributionAmount.message}</FormHelperText>
                  )}
                </Grid2>
              </Grid2>
              <Grid2 container xs={12} spacing={2}>
                <Grid2 xs={3}>
                  <Controller
                    name="thirdRowContributionDate"
                    control={control}
                    render={({ field }) => (
                      <DsmDatePicker
                        id="thirdRowContributionDate"
                        label="Contribution Date"
                        onChange={(value: Date | null) => field.onChange(value)}
                        value={field.value ?? null}
                        error={errors.thirdRowContributionDate?.message} required={false} />
                    )}
                  />
                </Grid2>
                <Grid2 xs={3}>
                  <FormLabel>Contribution Amount</FormLabel>
                  <Controller
                    name="thirdRowContributionAmount"
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        variant="outlined"
                        error={!!errors.thirdRowContributionAmount}
                        onChange={(e) => {
                          field.onChange(e);
                        }}
                        inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                      />
                    )}
                  />
                  {errors.thirdRowContributionAmount && (
                    <FormHelperText error>{errors.thirdRowContributionAmount.message}</FormHelperText>
                  )}
                </Grid2>
              </Grid2>
              <Grid2 container xs={12} spacing={2}>
                <Grid2 xs={3}>
                  <Controller
                    name="fourthRowContributionDate"
                    control={control}
                    render={({ field }) => (
                      <DsmDatePicker
                        id="fourthRowContributionDate"
                        label="Contribution Date"
                        onChange={(value: Date | null) => field.onChange(value)}
                        value={field.value ?? null}
                        error={errors.fourthRowContributionDate?.message} required={false} />
                    )}
                  />
                </Grid2>
                <Grid2 xs={3}>
                  <FormLabel>Contribution Amount</FormLabel>
                  <Controller
                    name="fourthRowContributionAmount"
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        variant="outlined"
                        error={!!errors.fourthRowContributionAmount}
                        onChange={(e) => {
                          field.onChange(e);
                        }}
                        inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                      />
                    )}
                  />
                  {errors.fourthRowContributionAmount && (
                    <FormHelperText error>{errors.fourthRowContributionAmount.message}</FormHelperText>
                  )}
                </Grid2>
              </Grid2>


              <Grid2 container xs={12} justifyContent="flex-start" spacing={2}>
                <Grid2>
                  <Button variant="contained" type="submit"> 
                    {/* TODO: Implement once we have the endpoint */}
                    Save
                  </Button>
                </Grid2>
                <Grid2>
                  <Button variant="outlined" onClick={handleCancel}>
                    Cancel
                  </Button>
                </Grid2>

              </Grid2>
            </Grid2>
          </form>
        </>
      )}
    </>
  );
};

export default MilitaryAndRehireEntryAndModificationGrid;