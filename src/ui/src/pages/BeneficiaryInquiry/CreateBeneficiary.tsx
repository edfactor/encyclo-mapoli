import { FormLabel, MenuItem, Select, TextField } from "@mui/material";
import { Grid } from "@mui/material";
import { useEffect } from "react";

import { yupResolver } from "@hookform/resolvers/yup";
import Checkbox from "@mui/material/Checkbox";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { Controller, Resolver, useForm } from "react-hook-form";
import {
  useLazyCreateBeneficiariesQuery,
  useLazyCreateBeneficiaryContactQuery,
  useLazyUpdateBeneficiaryQuery
} from "reduxstore/api/BeneficiariesApi";
import {
  BeneficiaryDto,
  BeneficiaryKindDto,
  CreateBeneficiaryContactRequest,
  CreateBeneficiaryContactResponse,
  CreateBeneficiaryRequest,
  UpdateBeneficiaryRequest,
  UpdateBeneficiaryResponse
} from "reduxstore/types";
import { tryddmmyyyyToDate } from "utils/dateUtils";
import * as yup from "yup";
import SubmitAndReset from "./SubmitAndReset";

const schema = yup.object().shape({
  beneficiarySsn: yup.number().required(),
  relationship: yup.string().required(),
  //percentage: yup.number().required(),
  dateOfBirth: yup.date().required(),
  street: yup.string().required(),
  city: yup.string().required(),
  state: yup.string().required(),
  postalCode: yup.string().required(),
  firstName: yup.string().required(),
  lastName: yup.string().required(),
  addressSameAsBeneficiary: yup.boolean().notRequired(),
  kindId: yup.string().required()
});

export interface cb {
  beneficiarySsn: number;
  relationship: string;
  //percentage: number;
  dateOfBirth?: Date;
  street: string;
  city: string;
  state: string;
  postalCode: string;
  firstName: string;
  lastName: string;
  addressSameAsBeneficiary: boolean;
  kindId: string;
}
type Props = {
  badgeNumber: number;
  psnSuffix: number;
  beneficiaryKind: BeneficiaryKindDto[];
  onSaveSuccess: () => void;
  selectedBeneficiary?: BeneficiaryDto;
};

const CreateBeneficiary: React.FC<Props> = ({
  badgeNumber,
  onSaveSuccess,
  beneficiaryKind,
  psnSuffix,
  selectedBeneficiary
}) => {
  const [triggerAdd, { isFetching }] = useLazyCreateBeneficiariesQuery();

  const [triggerCreateBeneficiaryContact, createBeneficiaryContactResponse] = useLazyCreateBeneficiaryContactQuery();
  const [triggerUpdateBeneficiary, udpateResponse] = useLazyUpdateBeneficiaryQuery();

  const {
    control,
    register,
    formState: { errors, isValid },
    setValue,
    handleSubmit,
    reset,
    setFocus,
    watch
  } = useForm<cb>({
    resolver: yupResolver(schema) as Resolver<cb>,
    defaultValues: selectedBeneficiary
      ? {
          beneficiarySsn: undefined,
          relationship: selectedBeneficiary.relationship,
          //percentage: selectedBeneficiary.percent,
          dateOfBirth: selectedBeneficiary?.dateOfBirth,
          street: selectedBeneficiary?.street,
          city: selectedBeneficiary?.city,
          state: selectedBeneficiary?.state,
          postalCode: selectedBeneficiary?.postalCode,
          firstName: selectedBeneficiary?.firstName,
          lastName: selectedBeneficiary?.lastName,
          addressSameAsBeneficiary: false,
          kindId: selectedBeneficiary.kindId + ""
        }
      : {
          beneficiarySsn: 0,
          relationship: "",
          //percentage: 0,
          dateOfBirth: undefined,
          street: "",
          city: "",
          state: "",
          postalCode: "",
          firstName: "",
          lastName: "",
          addressSameAsBeneficiary: false,
          kindId: ""
        }
  });

  const handleReset = () => {
    reset();
  };

  const createBeneficiary = (data: cb) => {
    const request: CreateBeneficiaryContactRequest = {
      contactSsn: data.beneficiarySsn,
      city: data.city,
      countryIso: "",
      dateOfBirth: data.dateOfBirth ? data.dateOfBirth.toISOString().split("T")[0] : "",
      emailAddress: "",
      firstName: data.firstName,
      lastName: data.lastName,
      middleName: "",
      mobileNumber: "",
      phoneNumber: "",
      postalCode: data.postalCode,
      state: data.state,
      street: data.street,
      street2: "",
      street3: "",
      street4: ""
    };
    triggerCreateBeneficiaryContact(request)
      .unwrap()
      .then((res: CreateBeneficiaryContactResponse) => {
        console.log(res);
        saveBeneficiary(res.id, data);
      })
      .catch((err) => {
        console.error(err);
      });
  };

  const updateBeneficiary = (data: cb) => {
    const request: UpdateBeneficiaryRequest = {
      contactSsn: data.beneficiarySsn,
      city: data.city,
      countryIso: null,
      dateOfBirth: data.dateOfBirth ? data.dateOfBirth.toISOString().split("T")[0] : "",
      emailAddress: null,
      firstName: data.firstName,
      lastName: data.lastName,
      middleName: null,
      mobileNumber: null,
      phoneNumber: null,
      postalCode: data.postalCode,
      state: data.state,
      street1: data.street,
      street2: null,
      street3: null,
      street4: null,
      id: selectedBeneficiary?.id ?? 0,
      kindId: data.kindId,
      //percentage: data.percentage,
      relationship: data.relationship
    };
    triggerUpdateBeneficiary(request)
      .unwrap()
      .then((res: UpdateBeneficiaryResponse) => {
        console.log("update successfully");
        onSaveSuccess();
      });
  };

  const onSubmit = (data: cb) => {
    if (selectedBeneficiary) {
      updateBeneficiary(data);
    } else {
      createBeneficiary(data);
    }
  };

  const saveBeneficiary = (beneficiaryContactId: number, data: cb) => {
    const request: CreateBeneficiaryRequest = {
      beneficiaryContactId: beneficiaryContactId,
      employeeBadgeNumber: badgeNumber,
      firstLevelBeneficiaryNumber: Math.floor(psnSuffix / 1000) % 10,
      kindId: data.kindId,
      //percentage: data.percentage,
      relationship: data.relationship,
      secondLevelBeneficiaryNumber: Math.floor(psnSuffix / 100) % 10,
      thirdLevelBeneficiaryNumber: Math.floor(psnSuffix / 10) % 10
    };
    triggerAdd(request)
      .unwrap()
      .then((value) => {
        console.log("saved successfully");
        onSaveSuccess();
      })
      .catch((err) => {
        console.error(err);
      });
  };
  const validateAndSubmit = handleSubmit(onSubmit);
  useEffect(() => {}, []);

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <form onSubmit={validateAndSubmit}>
        <Grid
          container
          size={12}
          rowSpacing={3}>
          <Grid size={{ md: 5, xs: 12 }}>
            <FormLabel>First Name</FormLabel>
            <Controller
              name="firstName"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.firstName}
                  onChange={(e) => {
                    field.onChange(e.target.value);
                  }}
                />
              )}
            />
          </Grid>
          <Grid
            offset={1}
            size={{ md: 5, xs: 12 }}>
            <FormLabel>Last Name</FormLabel>
            <Controller
              name="lastName"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.lastName}
                  onChange={(e) => {
                    field.onChange(e.target.value);
                  }}
                />
              )}
            />
          </Grid>
          <Grid size={{ md: 5, xs: 12 }}>
            <FormLabel>SSN</FormLabel>
            <Controller
              name="beneficiarySsn"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.beneficiarySsn}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
          </Grid>
          <Grid
            offset={1}
            size={{ md: 5, xs: 12 }}>
            <FormLabel>Date of Birth</FormLabel>
            <Controller
              name="dateOfBirth"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="dateOfBirth"
                  onChange={(value: Date | null) => {
                    field.onChange(value || undefined);
                  }}
                  value={field.value ? tryddmmyyyyToDate(field.value) : null}
                  required={false}
                  label=""
                  disableFuture
                  error={errors.dateOfBirth?.message}
                />
              )}
            />
          </Grid>
          <Grid size={{ md: 12, xs: 12 }}>
            <FormLabel>
              <Controller
                name="addressSameAsBeneficiary"
                control={control}
                render={({ field }) => (
                  <Checkbox
                    {...field}
                    checked={!!field.value} // Ensure it's a boolean
                    onChange={(e) => {
                      field.onChange(e.target.checked); // Use checked, not value
                    }}
                  />
                )}
              />
              Address the same as beneficiary ?
            </FormLabel>
          </Grid>
          <Grid size={{ md: 5, xs: 12 }}>
            <FormLabel>Address</FormLabel>
            <Controller
              name="street"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.street}
                  onChange={(e) => {
                    field.onChange(e.target.value);
                  }}
                />
              )}
            />
          </Grid>
          <Grid
            container
            columnSpacing={4}
            size={{ xs: 12, md: 12 }}>
            <Grid size={{ md: 5, xs: 12 }}>
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
            </Grid>
            <Grid size={{ md: 2, xs: 12 }}>
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
            </Grid>
            <Grid size={{ md: 3, xs: 12 }}>
              <FormLabel>Zipcode</FormLabel>
              <Controller
                name="postalCode"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.postalCode}
                    onChange={(e) => {
                      field.onChange(e.target.value);
                    }}
                  />
                )}
              />
            </Grid>
          </Grid>

          <Grid
            container
            columnSpacing={4}
            size={{ xs: 12, md: 12 }}>
            <Grid size={{ md: 5, xs: 12 }}>
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
                    onChange={(e) => field.onChange(e.target.value)}>
                    {beneficiaryKind.map((d) => (
                      <MenuItem value={d.id}>{d.name}</MenuItem>
                    ))}
                  </Select>
                )}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 12 }}>
              <FormLabel>Relationship</FormLabel>
              <Controller
                name="relationship"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.relationship}
                    onChange={(e) => {
                      field.onChange(e.target.value);
                    }}
                  />
                )}
              />
            </Grid>
            {/* <Grid size={{ md: 3, xs: 12 }}>
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
                        </Grid> */}
          </Grid>

          <Grid
            container
            justifyContent="flex-end"
            paddingY="16px">
            <Grid size={{ xs: 12 }}>
              <SubmitAndReset
                handleReset={handleReset}
                handleSearch={validateAndSubmit}
                isFetching={isFetching}
                disabled={!isValid}
              />
            </Grid>
          </Grid>
        </Grid>
      </form>
    </LocalizationProvider>
  );
};

export default CreateBeneficiary;
