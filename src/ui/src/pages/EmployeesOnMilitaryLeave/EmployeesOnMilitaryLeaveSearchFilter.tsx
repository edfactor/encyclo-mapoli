import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { isValid } from "date-fns";
import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { useLazyGetMilitaryAndRehireQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface ISearchForm {
  startDate: Date;
  endDate: Date;
}

const schema = yup.object().shape({
  endDate: yup.date().required("End Date is required"),
  startDate: yup.date().required("Start Date is required"),
});

const MilitaryAndRehireSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireQuery();

  const { control, handleSubmit, formState: { errors, isValid }, reset } = useForm<ISearchForm>({
    resolver: yupResolver(schema),
    defaultValues: {
      startDate: undefined,
      endDate: undefined
    }
  });

  const validateAndSearch = (data: ISearchForm) => {
    // TODO: triggerSearch(
    //   {
    //     pagination: { skip: 0, take: 25 },
    //     cutoffDate: data.cutoffDate
    //   },
    //   false
    // );
  };

  const handleReset = () => {
    reset();
  };


  return (
    <form onSubmit={handleSubmit(validateAndSearch)}>
      <Grid2
        container
        paddingX="24px"
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
                id="start"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                required={true}
                label="Beginning Date"
                disableFuture
                error={errors.startDate?.message}
              />
            )}
          />
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
                label="Ending Date"
                disableFuture
                error={errors.endDate?.message}
              />
            )}
          />
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          disabled={!isValid}
          handleReset={handleReset}
          handleSearch={handleSubmit(validateAndSearch)}
          isFetching={isFetching}
        />
      </Grid2>
    </form>
  );
};

export default MilitaryAndRehireSearchFilter;
