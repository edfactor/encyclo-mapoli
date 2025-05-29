import { Divider, FormLabel, TextField } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useEffect, useState } from "react";
import { DSMAccordion, Page, SearchAndReset } from "smart-ui-library";
import BeneficiaryInquiryGrid from "./BeneficiaryInquiryGrid";
import { Controller, Resolver, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { CreateBeneficiaryRequestDto } from "reduxstore/types";
import { useLazyCreateBeneficiariesQuery } from "reduxstore/api/BeneficiariesApi";

const schema = yup.object().shape({
    employeeBadgeNumber: yup.number().required(),
    beneficiarySsn: yup.number().required(),
    firstLevelBeneficiaryNumber: yup.number().nullable().notRequired(),
    secondLevelBeneficiaryNumber: yup.number().nullable().notRequired(),
    thirdLevelBeneficiaryNumber: yup.number().nullable().notRequired(),
    relationship: yup.string().required(),
    kindId: yup.string().required(),
    percentage: yup.number().required(),
    dateOfBirth: yup.string().required(),
    street: yup.string().required(),
    street2: yup.string().nullable().notRequired(),
    street3: yup.string().nullable().notRequired(),
    street4: yup.string().nullable().notRequired(),
    city: yup.string().required(),
    state: yup.string().required(),
    postalCode: yup.string().required(),
    countryIso: yup.string().nullable().notRequired(),
    firstName: yup.string().required(),
    lastName: yup.string().required(),
    middleName: yup.string().nullable().notRequired(),
    phoneNumber: yup.string().nullable().notRequired(),
    mobileNumber: yup.string().nullable().notRequired(),
    emailAddress: yup.string().nullable().notRequired()
});

const CreateBeneficiary = () => {
    const [triggerAdd, {data,isLoading,isError,isFetching}] = useLazyCreateBeneficiariesQuery();

    const {
        control,
        register,
        formState: { errors, isValid },
        setValue,
        handleSubmit,
        reset,
        setFocus,
        watch
    } = useForm<CreateBeneficiaryRequestDto>({
        resolver: yupResolver(schema) as Resolver<CreateBeneficiaryRequestDto>
    });

    const handleReset = () => {
        reset();
    }

    const onSubmit = (data: any) => {

        console.log(data);
    };

    return (
        <Grid2 container size={12} rowSpacing={3}>
            <h3>Add Beneficiary</h3>
            <Grid2 container>
                <Grid2 spacing={3} size={{ md: 4, xs: 12 }}>
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
                                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                    field.onChange(parsedValue);
                                }}
                            />
                        )}
                    />
                </Grid2>
                <Grid2 spacing={3} size={{ md: 4, xs: 12 }}>
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
                                error={!!errors.firstName}
                                onChange={(e) => {
                                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                    field.onChange(parsedValue);
                                }}
                            />
                        )}
                    />
                </Grid2>
            </Grid2>
            <Grid2>
                <Grid2 spacing={3} size={{ md: 4, xs: 12 }}>
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
                                error={!!errors.firstName}
                                onChange={(e) => {
                                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                    field.onChange(parsedValue);
                                }}
                            />
                        )}
                    />
                </Grid2>
                <Grid2 spacing={3} size={{ md: 4, xs: 12 }}>
                    <FormLabel>Date of Birth</FormLabel>
                    <Controller
                        name="dateOfBirth"
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
                                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                                    field.onChange(parsedValue);
                                }}
                            />
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
                        handleSearch={onSubmit}
                        isFetching={isFetching}
                        disabled={!isValid}
                    />
                </Grid2>
            </Grid2>

        </Grid2>
    );
};

export default CreateBeneficiary;
