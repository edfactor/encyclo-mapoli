import { Divider, FormLabel, MenuItem, Select, TextField } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useEffect, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";

import BeneficiaryInquiryGrid from "./BeneficiaryInquiryGrid";
import { Controller, Resolver, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { BeneficiaryTypeDto, BeneficiaryTypesResponseDto, CreateBeneficiaryContactRequest, CreateBeneficiaryContactResponse, CreateBeneficiaryRequest } from "reduxstore/types";
import { useLazyCreateBeneficiariesQuery, useLazyCreateBeneficiaryContactQuery, useLazyGetBeneficiarytypesQuery } from "reduxstore/api/BeneficiariesApi";
import Checkbox from '@mui/material/Checkbox';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs from 'dayjs';
import { useFieldState } from "@mui/x-date-pickers/internals/hooks/useField/useFieldState";
import SubmitAndReset from "./SubmitAndReset";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { tryddmmyyyyToDate } from "utils/dateUtils";

const schema = yup.object().shape({
    beneficiarySsn: yup.number().required(),
    relationship: yup.string().required(),
    percentage: yup.number().required(),
    dateOfBirth: yup.string().required(),
    street: yup.string().required(),
    city: yup.string().required(),
    state: yup.string().required(),
    postalCode: yup.string().required(),
    firstName: yup.string().required(),
    lastName: yup.string().required(),
    addressSameAsBeneficiary: yup.boolean().notRequired(),
    beneficiaryTypeId: yup.number().required()
});

export interface cb {
    beneficiarySsn: number;
    relationship: string;
    percentage: number;
    dateOfBirth: string;
    street: string;
    city: string;
    state: string;
    postalCode: string;
    firstName: string;
    lastName: string;
    addressSameAsBeneficiary: boolean;
    beneficiaryTypeId: number;
}
type Props = {
    badgeNumber:number;
    beneficiaryTypes: BeneficiaryTypeDto[];
    onSaveSuccess: ()=>void;
}

const CreateBeneficiary: React.FC<Props> = ({badgeNumber, onSaveSuccess,beneficiaryTypes}) => {
    const [triggerAdd, { isFetching }] = useLazyCreateBeneficiariesQuery();
    
    const [triggerCreateBeneficiaryContact, createBeneficiaryContactResponse] = useLazyCreateBeneficiaryContactQuery();

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
        resolver: yupResolver(schema) as Resolver<cb>
    });

    const handleReset = () => {
        reset();
    }

    const onSubmit = (data: cb) => {
        let request: CreateBeneficiaryContactRequest = {
            contactSsn: data.beneficiarySsn,
            city: data.city,
            countryIso: '',
            dateOfBirth: new Date(data.dateOfBirth),
            emailAddress: '',
            firstName: data.firstName,
            lastName: data.lastName,
            middleName: '',
            mobileNumber: '',
            phoneNumber: '',
            postalCode: data.postalCode,
            state: data.state,
            street: data.street,
            street2: '',
            street3: '',
            street4: ''
        }
        triggerCreateBeneficiaryContact(request).unwrap().then((res: CreateBeneficiaryContactResponse) => {

            console.log(res);
            saveBeneficiary(res.id, data);
        }).catch((err) => { console.error(err) });
    };

    const saveBeneficiary  = (beneficiaryContactId:number, data:cb)=>{
        let request: CreateBeneficiaryRequest = {
            beneficiaryContactId: beneficiaryContactId,
            employeeBadgeNumber: badgeNumber,
            firstLevelBeneficiaryNumber: 1,
            kindId: 'P', 
            percentage : data.percentage,
            relationship: data.relationship,
            secondLevelBeneficiaryNumber: 0,
            thirdLevelBeneficiaryNumber: 0
        }
        triggerAdd(request).unwrap().then((value)=>{
            console.log('saved successfully');
            onSaveSuccess();
        }).catch((err)=>{console.error(err)})
    }
    const validateAndSubmit = handleSubmit(onSubmit);
    useEffect(() => {
    }, [])

    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <form onSubmit={validateAndSubmit}>
                <Grid2 container size={12} rowSpacing={3}>

                    <Grid2 size={{ md: 5, xs: 12 }}>
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
                    </Grid2>
                    <Grid2 offset={1} size={{ md: 5, xs: 12 }}>
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
                    </Grid2>
                    <Grid2 size={{ md: 5, xs: 12 }}>
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
                    </Grid2>
                    <Grid2 offset={1} size={{ md: 5, xs: 12 }}>
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
                    </Grid2>
                    <Grid2 size={{ md: 12, xs: 12 }}>
                        <FormLabel>
                            <Controller
                                name="addressSameAsBeneficiary"
                                control={control}
                                render={({ field }) => (
                                    <Checkbox  {...field}
                                        checked={!!field.value} // Ensure it's a boolean
                                        onChange={(e) => {
                                            field.onChange(e.target.checked); // Use checked, not value
                                        }}
                                    />
                                )}
                            />
                            Address the same as beneficiary ?
                        </FormLabel>

                    </Grid2>
                    <Grid2 size={{ md: 5, xs: 12 }}>
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
                    </Grid2>
                    <Grid2 container columnSpacing={4} size={{ xs: 12, md: 12 }}>
                        <Grid2 size={{ md: 5, xs: 12 }}>
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
                        </Grid2>
                        <Grid2 size={{ md: 2, xs: 12 }}>
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
                        </Grid2>
                        <Grid2 size={{ md: 3, xs: 12 }}>
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
                        </Grid2>
                    </Grid2>

                    <Grid2 container columnSpacing={4} size={{ xs: 12, md: 12 }}>
                        <Grid2 size={{ md: 5, xs: 12 }}>
                            <FormLabel>Beneficiary Type</FormLabel>
                            <Controller
                                name="beneficiaryTypeId"
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        {...field}
                                        fullWidth
                                        size="small"
                                        variant="outlined"
                                        labelId="beneficiaryTypeId"
                                        id="beneficiaryTypeId"
                                        value={field.value}
                                        label="Beneficiary Type"
                                        onChange={(e) => field.onChange(e.target.value)}
                                    >
                                        {beneficiaryTypes.map((d) => (
                                            <MenuItem value={d.id}>{d.name}</MenuItem>
                                        ))}
                                    </Select>
                                )}
                            />
                        </Grid2>
                        <Grid2 size={{ md: 4, xs: 12 }}>
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
                        </Grid2>
                        <Grid2 size={{ md: 3, xs: 12 }}>
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
                        </Grid2>
                    </Grid2>

                    <Grid2
                        container
                        justifyContent="flex-end"
                        paddingY="16px">
                        <Grid2 size={{ xs: 12 }}>
                            <SubmitAndReset
                                handleReset={handleReset}
                                handleSearch={validateAndSubmit}
                                isFetching={isFetching}
                                disabled={!isValid}
                            />
                        </Grid2>
                    </Grid2>


                </Grid2>
            </form>
        </LocalizationProvider>

    );
};

export default CreateBeneficiary;
