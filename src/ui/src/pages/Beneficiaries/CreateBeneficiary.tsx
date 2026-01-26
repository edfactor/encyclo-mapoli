import { FormControlLabel, FormLabel, Grid, TextField, Typography } from "@mui/material";

import { yupResolver } from "@hookform/resolvers/yup";
import Checkbox from "@mui/material/Checkbox";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { useCallback, useEffect, useState } from "react";
import { Controller, ControllerRenderProps, Resolver, useForm } from "react-hook-form";
import {
  useCreateBeneficiariesMutation,
  useCreateBeneficiaryContactMutation,
  useUpdateBeneficiaryMutation
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
import { VisuallyHidden } from "utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "utils/accessibilityUtils";
import { tryddmmyyyyToDate } from "utils/dateUtils";
import { ssnValidator } from "utils/FormValidators";
import { ARIA_DESCRIPTIONS, formatSSNInput, formatZipCode, INPUT_PLACEHOLDERS } from "utils/inputFormatters";
import * as yup from "yup";
import { decomposePSNSuffix } from "./utils/badgeUtils";

const schema = yup.object().shape({
  beneficiarySsn: ssnValidator.required("SSN is required"),
  relationship: yup.string().required("Relationship is required"),
  //percentage: yup.number().required(),
  dateOfBirth: yup.date().required("Date of Birth is required"),
  street: yup.string().when("addressSameAsBeneficiary", {
    is: false,
    then: (schema) => schema.required("Address is required"),
    otherwise: (schema) => schema.optional()
  }),
  city: yup.string().when("addressSameAsBeneficiary", {
    is: false,
    then: (schema) => schema.required("City is required"),
    otherwise: (schema) => schema.optional()
  }),
  state: yup.string().when("addressSameAsBeneficiary", {
    is: false,
    then: (schema) => schema.required("State is required"),
    otherwise: (schema) => schema.optional()
  }),
  postalCode: yup.string().when("addressSameAsBeneficiary", {
    is: false,
    then: (schema) => schema.required("Zipcode is required"),
    otherwise: (schema) => schema.optional()
  }),
  firstName: yup.string().required("First Name is required"),
  lastName: yup.string().required("Last Name is required"),
  addressSameAsBeneficiary: yup.boolean().notRequired()
});

export interface CreateBeneficiaryForm {
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
  selectedMember
}) => {
  const [triggerAdd, { isLoading }] = useCreateBeneficiariesMutation();
  const [triggerCreateBeneficiaryContact] = useCreateBeneficiaryContactMutation();
  const [triggerUpdateBeneficiary] = useUpdateBeneficiaryMutation();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    control,
    formState: { errors, isValid },
    handleSubmit,
    reset,
    watch
  } = useForm<CreateBeneficiaryForm>({
    resolver: yupResolver(schema) as Resolver<CreateBeneficiaryForm>,
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
          addressSameAsBeneficiary: false
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
          addressSameAsBeneficiary: false
        }
  });

  const addressSameAsBeneficiary = watch("addressSameAsBeneficiary");

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const handleReset = () => {
    reset();
  };

  // Live SSN formatting handler
  const handleSSNChange = useCallback(
    (
      e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
      field: ControllerRenderProps<CreateBeneficiaryForm, "beneficiarySsn">
    ) => {
      const { display, raw } = formatSSNInput(e.target.value);
      e.target.value = display; // Update visual display
      field.onChange(raw === "" ? null : raw); // Store raw value
    },
    []
  );

  // Live zip code formatting handler
  const handleZipChange = useCallback(
    (
      e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
      field: ControllerRenderProps<CreateBeneficiaryForm, "postalCode">
    ) => {
      const formatted = formatZipCode(e.target.value);
      e.target.value = formatted.display;
      field.onChange(formatted.display);
    },
    []
  );

  const createBeneficiary = (data: CreateBeneficiaryForm) => {
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

  const updateBeneficiary = (data: CreateBeneficiaryForm) => {
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
      //percentage: data.percentage,
      relationship: data.relationship
    };
    triggerUpdateBeneficiary(request)
      .unwrap()
      .then((_res: UpdateBeneficiaryResponse) => {
        onSaveSuccess();
      });
  };

  const onSubmit = (data: CreateBeneficiaryForm) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      if (selectedBeneficiary) {
        updateBeneficiary(data);
      } else {
        createBeneficiary(data);
      }
    }
  };

  const saveBeneficiary = (beneficiaryContactId: number, data: CreateBeneficiaryForm) => {
    const { firstLevel, secondLevel, thirdLevel } = decomposePSNSuffix(psnSuffix);

    const request: CreateBeneficiaryRequest = {
      beneficiaryContactId: beneficiaryContactId,
      employeeBadgeNumber: badgeNumber,
      firstLevelBeneficiaryNumber: firstLevel === 0 ? null : firstLevel,
      //percentage: data.percentage,
      relationship: data.relationship,
      secondLevelBeneficiaryNumber: secondLevel === 0 ? null : secondLevel,
      thirdLevelBeneficiaryNumber: thirdLevel === 0 ? null : thirdLevel
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
            <FormLabel htmlFor={generateFieldId("firstName")}>
              First Name{" "}
              <Typography
                component="span"
                color="error">
                *
              </Typography>
            </FormLabel>
            <Controller
              name="firstName"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("firstName")}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.firstName}
                    autoComplete="off"
                    placeholder={INPUT_PLACEHOLDERS.NAME}
                    aria-invalid={!!errors.firstName || undefined}
                    aria-describedby={getAriaDescribedBy("firstName", !!errors.firstName, false)}
                    onChange={(e) => {
                      field.onChange(e.target.value);
                    }}
                  />
                  {errors.firstName && (
                    <div
                      id="firstName-error"
                      aria-live="polite"
                      aria-atomic="true">
                      <Typography
                        variant="caption"
                        color="error">
                        {errors.firstName.message}
                      </Typography>
                    </div>
                  )}
                </>
              )}
            />
          </Grid>
          <Grid
            offset={1}
            size={{ md: 5, xs: 12 }}>
            <FormLabel htmlFor={generateFieldId("lastName")}>
              Last Name{" "}
              <Typography
                component="span"
                color="error">
                *
              </Typography>
            </FormLabel>
            <Controller
              name="lastName"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("lastName")}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.lastName}
                    autoComplete="off"
                    placeholder={INPUT_PLACEHOLDERS.NAME}
                    aria-invalid={!!errors.lastName || undefined}
                    aria-describedby={getAriaDescribedBy("lastName", !!errors.lastName, false)}
                    onChange={(e) => {
                      field.onChange(e.target.value);
                    }}
                  />
                  {errors.lastName && (
                    <div
                      id="lastName-error"
                      aria-live="polite"
                      aria-atomic="true">
                      <Typography
                        variant="caption"
                        color="error">
                        {errors.lastName.message}
                      </Typography>
                    </div>
                  )}
                </>
              )}
            />
          </Grid>
          <Grid size={{ md: 5, xs: 12 }}>
            <FormLabel htmlFor={generateFieldId("beneficiarySsn")}>
              SSN{" "}
              <Typography
                component="span"
                color="error">
                *
              </Typography>
            </FormLabel>
            <Controller
              name="beneficiarySsn"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("beneficiarySsn")}
                    fullWidth
                    type="text"
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.beneficiarySsn}
                    autoComplete="off"
                    placeholder={INPUT_PLACEHOLDERS.SSN}
                    inputProps={{ inputMode: "numeric" }}
                    aria-invalid={!!errors.beneficiarySsn || undefined}
                    aria-describedby={getAriaDescribedBy("beneficiarySsn", !!errors.beneficiarySsn, true)}
                    onChange={(e) => {
                      handleSSNChange(e, field as ControllerRenderProps<CreateBeneficiaryForm, "beneficiarySsn">);
                    }}
                  />
                  <VisuallyHidden id="beneficiarySsn-hint">{ARIA_DESCRIPTIONS.SSN_FORMAT}</VisuallyHidden>
                  {errors.beneficiarySsn && (
                    <div
                      id="beneficiarySsn-error"
                      aria-live="polite"
                      aria-atomic="true">
                      <Typography
                        variant="caption"
                        color="error">
                        {errors.beneficiarySsn.message}
                      </Typography>
                    </div>
                  )}
                </>
              )}
            />
          </Grid>
          <Grid
            offset={1}
            size={{ md: 5, xs: 12 }}>
            <FormLabel>
              Date of Birth{" "}
              <Typography
                component="span"
                color="error">
                *
              </Typography>
            </FormLabel>
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
            <Controller
              name="addressSameAsBeneficiary"
              control={control}
              render={({ field }) => (
                <FormControlLabel
                  control={
                    <Checkbox
                      {...field}
                      checked={!!field.value}
                      onChange={(e) => {
                        field.onChange(e.target.checked);
                      }}
                    />
                  }
                  label="Is Beneficiary Address the same as Employee Address?"
                />
              )}
            />
          </Grid>
          <Grid size={{ md: 5, xs: 12 }}>
            <FormLabel htmlFor={generateFieldId("street")}>
              Address{" "}
              {!addressSameAsBeneficiary && (
                <Typography
                  component="span"
                  color="error">
                  *
                </Typography>
              )}
            </FormLabel>
            <Controller
              name="street"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("street")}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.street}
                    autoComplete="off"
                    placeholder={INPUT_PLACEHOLDERS.STREET_ADDRESS}
                    aria-invalid={!!errors.street || undefined}
                    aria-describedby={getAriaDescribedBy("street", !!errors.street, false)}
                    onChange={(e) => {
                      field.onChange(e.target.value);
                    }}
                  />
                  {errors.street && (
                    <div
                      id="street-error"
                      aria-live="polite"
                      aria-atomic="true">
                      <Typography
                        variant="caption"
                        color="error">
                        {errors.street.message}
                      </Typography>
                    </div>
                  )}
                </>
              )}
            />
          </Grid>
          <Grid
            container
            columnSpacing={4}
            size={{ xs: 12, md: 12 }}>
            <Grid size={{ md: 5, xs: 12 }}>
              <FormLabel htmlFor={generateFieldId("city")}>
                City{" "}
                {!addressSameAsBeneficiary && (
                  <Typography
                    component="span"
                    color="error">
                    *
                  </Typography>
                )}
              </FormLabel>
              <Controller
                name="city"
                control={control}
                render={({ field }) => (
                  <>
                    <TextField
                      {...field}
                      id={generateFieldId("city")}
                      fullWidth
                      size="small"
                      variant="outlined"
                      value={field.value ?? ""}
                      error={!!errors.city}
                      autoComplete="off"
                      placeholder={INPUT_PLACEHOLDERS.CITY}
                      aria-invalid={!!errors.city || undefined}
                      aria-describedby={getAriaDescribedBy("city", !!errors.city, false)}
                      onChange={(e) => {
                        field.onChange(e.target.value);
                      }}
                    />
                    {errors.city && (
                      <div
                        id="city-error"
                        aria-live="polite"
                        aria-atomic="true">
                        <Typography
                          variant="caption"
                          color="error">
                          {errors.city.message}
                        </Typography>
                      </div>
                    )}
                  </>
                )}
              />
            </Grid>
            <Grid size={{ md: 2, xs: 12 }}>
              <FormLabel htmlFor={generateFieldId("state")}>
                State{" "}
                {!addressSameAsBeneficiary && (
                  <Typography
                    component="span"
                    color="error">
                    *
                  </Typography>
                )}
              </FormLabel>
              <Controller
                name="state"
                control={control}
                render={({ field }) => (
                  <>
                    <TextField
                      {...field}
                      id={generateFieldId("state")}
                      fullWidth
                      size="small"
                      variant="outlined"
                      value={field.value ?? ""}
                      error={!!errors.state}
                      autoComplete="off"
                      placeholder={INPUT_PLACEHOLDERS.STATE}
                      aria-invalid={!!errors.state || undefined}
                      aria-describedby={getAriaDescribedBy("state", !!errors.state, false)}
                      onChange={(e) => {
                        field.onChange(e.target.value);
                      }}
                    />
                    {errors.state && (
                      <div
                        id="state-error"
                        aria-live="polite"
                        aria-atomic="true">
                        <Typography
                          variant="caption"
                          color="error">
                          {errors.state.message}
                        </Typography>
                      </div>
                    )}
                  </>
                )}
              />
            </Grid>
            <Grid size={{ md: 3, xs: 12 }}>
              <FormLabel htmlFor={generateFieldId("postalCode")}>
                Zipcode{" "}
                {!addressSameAsBeneficiary && (
                  <Typography
                    component="span"
                    color="error">
                    *
                  </Typography>
                )}
              </FormLabel>
              <Controller
                name="postalCode"
                control={control}
                render={({ field }) => (
                  <>
                    <TextField
                      {...field}
                      id={generateFieldId("postalCode")}
                      fullWidth
                      size="small"
                      variant="outlined"
                      value={field.value ?? ""}
                      error={!!errors.postalCode}
                      autoComplete="off"
                      placeholder={INPUT_PLACEHOLDERS.ZIP_CODE}
                      inputProps={{ inputMode: "numeric" }}
                      aria-invalid={!!errors.postalCode || undefined}
                      aria-describedby={getAriaDescribedBy("postalCode", !!errors.postalCode, true)}
                      onChange={(e) => {
                        handleZipChange(e, field as ControllerRenderProps<CreateBeneficiaryForm, "postalCode">);
                      }}
                    />
                    <VisuallyHidden id="postalCode-hint">{ARIA_DESCRIPTIONS.ZIP_FORMAT}</VisuallyHidden>
                    {errors.postalCode && (
                      <div
                        id="postalCode-error"
                        aria-live="polite"
                        aria-atomic="true">
                        <Typography
                          variant="caption"
                          color="error">
                          {errors.postalCode.message}
                        </Typography>
                      </div>
                    )}
                  </>
                )}
              />
            </Grid>
          </Grid>

          <Grid
            container
            columnSpacing={4}
            size={{ xs: 12, md: 12 }}>
            <Grid size={{ md: 5, xs: 12 }}>
              <FormLabel htmlFor={generateFieldId("relationship")}>
                Relationship{" "}
                <Typography
                  component="span"
                  color="error">
                  *
                </Typography>
              </FormLabel>
              <Controller
                name="relationship"
                control={control}
                render={({ field }) => (
                  <>
                    <TextField
                      {...field}
                      id={generateFieldId("relationship")}
                      fullWidth
                      size="small"
                      variant="outlined"
                      value={field.value ?? ""}
                      error={!!errors.relationship}
                      autoComplete="off"
                      aria-invalid={!!errors.relationship || undefined}
                      aria-describedby={getAriaDescribedBy("relationship", !!errors.relationship, false)}
                      onChange={(e) => {
                        field.onChange(e.target.value);
                      }}
                    />
                    {errors.relationship && (
                      <div
                        id="relationship-error"
                        aria-live="polite"
                        aria-atomic="true">
                        <Typography
                          variant="caption"
                          color="error">
                          {errors.relationship.message}
                        </Typography>
                      </div>
                    )}
                  </>
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
                isFetching={isLoading || isSubmitting}
                disabled={!isValid || isLoading || isSubmitting}
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
