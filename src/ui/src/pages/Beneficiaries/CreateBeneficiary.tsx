import { FormLabel, Grid, MenuItem, Select, TextField, Typography } from "@mui/material";

import { yupResolver } from "@hookform/resolvers/yup";
import Checkbox from "@mui/material/Checkbox";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import {
  useLazyCreateBeneficiariesQuery,
  useLazyCreateBeneficiaryContactQuery,
  useLazyUpdateBeneficiaryQuery
} from "reduxstore/api/BeneficiariesApi";
import {
  BeneficiaryDetail,
  BeneficiaryDto,
  CreateBeneficiaryContactRequest,
  CreateBeneficiaryContactResponse,
  CreateBeneficiaryRequest,
  UpdateBeneficiaryRequest,
  UpdateBeneficiaryResponse
} from "reduxstore/types";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import { tryddmmyyyyToDate } from "utils/dateUtils";
import { ssnValidator } from "utils/FormValidators";
import * as yup from "yup";
import { useBeneficiaryKinds } from "./hooks/useBeneficiaryKinds";

const schema = yup.object().shape({
  beneficiarySsn: ssnValidator.required("SSN is required"),
  relationship: yup.string().required(),
  //percentage: yup.number().required(),
  dateOfBirth: yup.date().required(),
  street: yup.string().optional(),
  city: yup.string().optional(),
  state: yup.string().optional(),
  postalCode: yup.string().optional(),
  firstName: yup.string().required(),
  lastName: yup.string().required(),
  addressSameAsBeneficiary: yup.boolean().notRequired(),
  kindId: yup.string().required()
});

export interface cb {
  beneficiarySsn: string | null | undefined;
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
type CreateBeneficiaryProps = {
  badgeNumber: number;
  psnSuffix: number;
  onSaveSuccess: () => void;
  selectedBeneficiary?: BeneficiaryDto;
  selectedMember: BeneficiaryDetail;
  existingBeneficiaries?: BeneficiaryDto[];
};

const CreateBeneficiary: React.FC<CreateBeneficiaryProps> = ({
  badgeNumber,
  onSaveSuccess,
  psnSuffix,
  selectedBeneficiary,
  selectedMember,
  existingBeneficiaries
}) => {
  const { beneficiaryKinds } = useBeneficiaryKinds();

  // Check if employee already has a primary beneficiary
  const hasPrimaryBeneficiary = existingBeneficiaries?.some((b) => b.kindId + "" === "P") || false;

  // Filter beneficiary kinds based on existing beneficiaries
  // If editing an existing beneficiary, allow all kinds
  // If creating new and already has primary, only show secondary
  const availableBeneficiaryKinds = selectedBeneficiary
    ? beneficiaryKinds // When editing, allow all kinds (user can edit existing primary)
    : hasPrimaryBeneficiary
      ? beneficiaryKinds.filter((kind) => kind.id !== "P") // When adding new and has primary, exclude primary
      : beneficiaryKinds; // When adding new and no primary exists, allow all

  const [triggerAdd, { isFetching }] = useLazyCreateBeneficiariesQuery();
  const [triggerCreateBeneficiaryContact] = useLazyCreateBeneficiaryContactQuery();
  const [triggerUpdateBeneficiary] = useLazyUpdateBeneficiaryQuery();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    control,
    formState: { errors, isValid },
    handleSubmit,
    reset
  } = useForm<cb>({
    resolver: yupResolver(schema) as Resolver<cb>,
    mode: "onBlur",
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
          beneficiarySsn: "",
          relationship: "",
          dateOfBirth: undefined,
          street: "",
          city: "",
          state: "",
          postalCode: "",
          firstName: "",
          lastName: "",
          addressSameAsBeneficiary: false,
          kindId: hasPrimaryBeneficiary ? "S" : "" // Default to Secondary if Primary exists
        }
  });

  useEffect(() => {
    if (!isFetching) {
      setIsSubmitting(false);
    }
  }, [isFetching]);

  const handleReset = () => {
    reset();
  };

  const createBeneficiary = (data: cb) => {
    const request: CreateBeneficiaryContactRequest = {
      contactSsn: Number(data.beneficiarySsn),
      city: data.city || selectedMember?.city || "",
      countryIso: "",
      dateOfBirth: data.dateOfBirth ? data.dateOfBirth.toISOString().split("T")[0] : "",
      emailAddress: "",
      firstName: data.firstName,
      lastName: data.lastName,
      middleName: "",
      mobileNumber: "",
      phoneNumber: "",
      postalCode: data.postalCode || selectedMember?.zip || "",
      state: data.state || selectedMember?.state || "",
      street: data.street || selectedMember?.street || "",
      street2: "",
      street3: "",
      street4: ""
    };
    triggerCreateBeneficiaryContact(request)
      .unwrap()
      .then((res: CreateBeneficiaryContactResponse) => {
        saveBeneficiary(res.id, data);
      })
      .catch((err) => {
        console.error(err);
      });
  };

  const updateBeneficiary = (data: cb) => {
    const request: UpdateBeneficiaryRequest = {
      contactSsn: Number(data.beneficiarySsn),
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
      .then((_res: UpdateBeneficiaryResponse) => {
        onSaveSuccess();
      });
  };

  const onSubmit = (data: cb) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      if (selectedBeneficiary) {
        updateBeneficiary(data);
      } else {
        createBeneficiary(data);
      }
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
      .then((_value) => {
        onSaveSuccess();
      })
      .catch((err) => {
        console.error(err);
      });
  };
  const validateAndSubmit = handleSubmit(onSubmit);

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
                  type="text"
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.beneficiarySsn}
                  helperText={errors.beneficiarySsn?.message}
                  onChange={(e) => {
                    const value = e.target.value;
                    // Only allow numeric input
                    if (value !== "" && !/^\d*$/.test(value)) {
                      return;
                    }
                    // Prevent input beyond 9 characters
                    if (value.length > 9) {
                      return;
                    }
                    field.onChange(value === "" ? null : value);
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
                <DSMDatePicker
                  id="dateOfBirth"
                  onChange={(value: Date | null) => {
                    field.onChange(value || undefined);
                  }}
                  value={field.value ? tryddmmyyyyToDate(field.value) : null}
                  required={false}
                  label=""
                  disableFuture
                  minDate={new Date(1900, 0, 1)}
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
              Address the same as employee ?
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
              {!selectedBeneficiary && hasPrimaryBeneficiary && (
                <Typography
                  variant="caption"
                  color="text.secondary"
                  display="block"
                  sx={{ mb: 1 }}>
                  Primary beneficiary already exists. Only Secondary beneficiaries can be added.
                </Typography>
              )}
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
                    {availableBeneficiaryKinds.map((d) => (
                      <MenuItem
                        key={d.id}
                        value={d.id}>
                        {d.name}
                      </MenuItem>
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
              <SearchAndReset
                handleReset={handleReset}
                handleSearch={validateAndSubmit}
                isFetching={isFetching || isSubmitting}
                disabled={!isValid || isFetching || isSubmitting}
                searchButtonText="Submit"
              />
            </Grid>
          </Grid>
        </Grid>
      </form>
    </LocalizationProvider>
  );
};

export default CreateBeneficiary;
