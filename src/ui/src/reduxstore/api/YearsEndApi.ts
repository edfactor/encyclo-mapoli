import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";

import {RootState} from "reduxstore/store";
import {
    DemographicBadgesNotInPayprofitRequestDto,
    DemographicBadgesNotInPayprofitResponse,
    DistributionsAndForfeitures,
    DistributionsAndForfeituresRequestDto,
    FrozenReportsByAgeRequest,
    DuplicateNameAndBirthday,
    DuplicateNameAndBirthdayRequestDto,
    DuplicateSSNDetail,
    DuplicateSSNsRequestDto,
    EligibleEmployeeResponseDto,
    EligibleEmployeesRequestDto,
    ExecutiveHoursAndDollars,
    ExecutiveHoursAndDollarsRequestDto,
    MasterInquryRequest,
    MilitaryAndRehire,
    MilitaryAndRehireForfeiture,
    MilitaryAndRehireForfeituresRequestDto,
    MilitaryAndRehireProfitSummary,
    MilitaryAndRehireProfitSummaryRequestDto,
    MilitaryAndRehireRequestDto,
    MissingCommasInPYName,
    MissingCommasInPYNameRequestDto,
    NegativeEtvaForSSNsOnPayProfit,
    NegativeEtvaForSSNsOnPayprofitRequestDto,
    PagedReportResponse,
    ProfitSharingDistributionsByAge,
    ContributionsByAge,
    ForfeituresByAge,
    BalanceByAge,
    VestedAmountsByAge,
    MasterInquiryResponseType,
    ProfitYearRequest,
    BalanceByYears,
    TerminationResponse,
    TerminationRequest,
    ProfitShareUpdateRequest,
    ProfitShareUpdateResponse,
    ProfitShareEditResponse,
    ProfitShareMasterResponse
} from "reduxstore/types";
import {
    setDemographicBadgesNotInPayprofitData,
    setDistributionsAndForfeitures,
    setDistributionsByAge,
    setContributionsByAge,
    setForfeituresByAge,
    setBalanceByAge,
    setDuplicateNamesAndBirthdays,
    setDuplicateSSNsData,
    setEligibleEmployees,
    setExecutiveHoursAndDollars,
    setMasterInquiryData,
    setMilitaryAndRehireDetails,
    setMilitaryAndRehireForfeituresDetails,
    setMilitaryAndRehireProfitSummaryDetails,
    setMissingCommaInPYName,
    setVestingAmountByAge,
    setNegativeEtvaForSssnsOnPayprofit, setBalanceByYears,
    setTermination,
    setProfitUpdate, clearProfitUpdate, setProfitEdit, clearProfitEdit, setProfitMasterApply, setProfitMasterRevert
} from "reduxstore/slices/yearsEndSlice";
import {url} from "./api";

export const YearsEndApi = createApi({
    baseQuery: fetchBaseQuery({
        baseUrl: `${url}/api/`,
        prepareHeaders: (headers, {getState}) => {
            const token = (getState() as RootState).security.token;
            const impersonating = (getState() as RootState).security.impersonating;
            if (token) {
                headers.set("authorization", `Bearer ${token}`);
            }
            if (impersonating) {
                headers.set("impersonation", impersonating);
            } else {
                const localImpersonation = localStorage.getItem("impersonatingRole");
                !!localImpersonation && headers.set("impersonation", localImpersonation);
            }
            return headers;
        }
    }),
    reducerPath: "yearsEndApi",
    endpoints: (builder) => ({
        getDuplicateSSNs: builder.query<PagedReportResponse<DuplicateSSNDetail>, DuplicateSSNsRequestDto>({
            query: (params) => ({
                url: `yearend/duplicate-ssns`,
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip,
                    profitYear: params.profitYear
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setDuplicateSSNsData(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getDemographicBadgesNotInPayprofit: builder.query<
            DemographicBadgesNotInPayprofitResponse,
            DemographicBadgesNotInPayprofitRequestDto
        >({
            query: (params) => ({
                url: `yearend/demographic-badges-not-in-payprofit`,
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setDemographicBadgesNotInPayprofitData(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getDistributionsAndForfeitures: builder.query<
            PagedReportResponse<DistributionsAndForfeitures>,
            DistributionsAndForfeituresRequestDto
        >({
            query: (params) => ({
                url: `yearend/distributions-and-forfeitures`,
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    includeOutgoingForfeitures: params.includeOutgoingForfeitures,
                    startMonth: params.startMonth,
                    endMonth: params.endMonth,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setDistributionsAndForfeitures(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getDuplicateNamesAndBirthdays: builder.query<
            PagedReportResponse<DuplicateNameAndBirthday>,
            DuplicateNameAndBirthdayRequestDto
        >({
            query: (params) => ({
                url: "yearend/duplicate-names-and-birthdays",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setDuplicateNamesAndBirthdays(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getMilitaryAndRehire: builder.query<PagedReportResponse<MilitaryAndRehire>, MilitaryAndRehireRequestDto>({
            query: (params) => ({
                url: "yearend/military-and-rehire",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setMilitaryAndRehireDetails(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getMilitaryAndRehireForfeitures: builder.query<
            PagedReportResponse<MilitaryAndRehireForfeiture>,
            MilitaryAndRehireForfeituresRequestDto
        >({
            query: (params) => ({
                url: `yearend/military-and-rehire-forfeitures/${params.reportingYear}`,
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setMilitaryAndRehireForfeituresDetails(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getMilitaryAndRehireProfitSummary: builder.query<
            PagedReportResponse<MilitaryAndRehireProfitSummary>,
            MilitaryAndRehireProfitSummaryRequestDto
        >({
            query: (params) => ({
                url: `yearend/military-and-rehire-profit-summary/${params.reportingYear}`,
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setMilitaryAndRehireProfitSummaryDetails(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getMismatchedSSNsPayprofitAndDemoOnSameBadge: builder.query({
            query: (params) => ({
                url: "yearend/mismatched-ssns-payprofit-and-demo-on-same-badge",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted({queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getNamesMissingCommas: builder.query<PagedReportResponse<MissingCommasInPYName>, MissingCommasInPYNameRequestDto>({
            query: (params) => ({
                url: "yearend/names-missing-commas",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setMissingCommaInPYName(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getNegativeEVTASSN: builder.query<
            PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>,
            NegativeEtvaForSSNsOnPayprofitRequestDto
        >({
            query: (params) => ({
                url: "yearend/negative-evta-ssn",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setNegativeEtvaForSssnsOnPayprofit(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getPayprofitBadgeWithoutDemographics: builder.query({
            query: (params) => ({
                url: "yearend/payprofit-badges-without-demographics",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted({dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getWagesCurrentYear: builder.query({
            query: (params) => ({
                url: "yearend/wages-current-year",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted({dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getWagesPreviousYear: builder.query({
            query: (params) => ({
                url: "yearend/wages-previous-year",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted({dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getExecutiveHoursAndDollars: builder.query<
            PagedReportResponse<ExecutiveHoursAndDollars>,
            ExecutiveHoursAndDollarsRequestDto
        >({
            query: (params) => ({
                url: "yearend/executive-hours-and-dollars",
                method: "GET",
                params: {
                    take: params.pagination.take,
                    skip: params.pagination.skip,
                    profitYear: params.profitYear,
                    badgeNumber: params.badgeNumber,
                    fullNameContains: params.fullNameContains,
                    hasExecutiveHoursAndDollars: params.hasExecutiveHoursAndDollars
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setExecutiveHoursAndDollars(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getEligibleEmployees: builder.query<EligibleEmployeeResponseDto, EligibleEmployeesRequestDto>({
            query: (params) => ({
                url: "yearend/eligible-employees",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setEligibleEmployees(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),

        getDistributionsByAge: builder.query<ProfitSharingDistributionsByAge, FrozenReportsByAgeRequest>({
            query: (params) => ({
                url: "yearend/frozen/distributions-by-age",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    reportType: params.reportType
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setDistributionsByAge(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getContributionsByAge: builder.query<ContributionsByAge, FrozenReportsByAgeRequest>({
            query: (params) => ({
                url: "yearend/frozen/contributions-by-age",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    reportType: params.reportType
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setContributionsByAge(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getForfeituresByAge: builder.query<ForfeituresByAge, FrozenReportsByAgeRequest>({
            query: (params) => ({
                url: "yearend/frozen/forfeitures-by-age",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    reportType: params.reportType
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setForfeituresByAge(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getBalanceByAge: builder.query<BalanceByAge, FrozenReportsByAgeRequest>({
            query: (params) => ({
                url: "yearend/frozen/balance-by-age",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    reportType: params.reportType
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setBalanceByAge(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getBalanceByYears: builder.query<BalanceByYears, FrozenReportsByAgeRequest>({
            query: (params) => ({
                url: "yearend/frozen/balance-by-years",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    reportType: params.reportType
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setBalanceByYears(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getProfitMasterInquiry: builder.query<MasterInquiryResponseType, MasterInquryRequest>({
            query: (params) => ({
                url: "master/master-inquiry",
                method: "GET",
                params: {
                    startProfitYear: params.startProfitYear,
                    endProfitYear: params.endProfitYear,
                    startProfitMonth: params.startProfitMonth,
                    endProfitMonth: params.endProfitMonth,
                    profitCode: params.profitCode,
                    contributionAmount: params.contributionAmount,
                    earningsAmount: params.earningsAmount,
                    forfeitureAmount: params.forfeitureAmount,
                    paymentAmount: params.paymentAmount,
                    socialSecurity: params.socialSecurity,
                    comment: params.comment,
                    take: params.pagination.take,
                    skip: params.pagination.skip
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setMasterInquiryData(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getVestingAmountByAge: builder.query<VestedAmountsByAge, ProfitYearRequest & { acceptHeader: string }>({
            query: (params) => ({
                url: "yearend/frozen/vested-amounts-by-age",
                method: "GET",
                params: {
                    profitYear: params.profitYear
                },
                headers: {
                    Accept: params.acceptHeader
                },
                responseHandler: async (response) => {
                    if (params.acceptHeader === 'text/csv') {
                        return response.blob();
                    }
                    return response.json();
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setVestingAmountByAge(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getTerminationReport: builder.query<TerminationResponse, TerminationRequest>({
            query: (params) => ({
                url: "yearend/terminated-employee-and-beneficiary",
                method: "GET",
                params: {
                    profitYear: params.profitYear,
                    skip: params.pagination.skip,
                    take: params.pagination.take
                }
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setTermination(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        }),
        getProfitShareUpdate: builder.query<ProfitShareUpdateResponse, ProfitShareUpdateRequest>({
            query: (params) => ({
                url: "yearend/profit-sharing-update",
                method: "GET",
                params: params
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setProfitUpdate(data));
                } catch (err) {
                    console.log("Err: " + err);
                    dispatch(clearProfitUpdate());
                }
            }
        }),
        getProfitShareEdit: builder.query<ProfitShareEditResponse, ProfitShareUpdateRequest>({
            query: (params) => ({
                url: "yearend/profit-share-edit",
                method: "GET",
                params: params
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setProfitEdit(data));
                } catch (err) {
                    console.log("Err: " + err);
                    dispatch(clearProfitEdit());
                }
            }
        }),
        getMasterApply: builder.query<ProfitShareMasterResponse, ProfitShareUpdateRequest>({
            query: (params) => ({
                url: "yearend/profit-master-update",
                method: "GET",
                params: params
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setProfitMasterApply(data));
                } catch (err) {
                    console.log("Err: " + err);
                    dispatch(clearProfitEdit());
                }
            }
        }),
        getMasterRevert: builder.query<ProfitShareMasterResponse, ProfitYearRequest>({
            query: (params) => ({
                url: "yearend/profit-master-revert",
                method: "GET",
                params: params
            }),
            async onQueryStarted(arg, {dispatch, queryFulfilled}) {
                try {
                    const {data} = await queryFulfilled;
                    dispatch(setProfitMasterRevert(data));
                } catch (err) {
                    console.log("Err: " + err);
                    dispatch(clearProfitEdit());
                }
            }
        }),

    })
});

export const {
    useLazyGetDemographicBadgesNotInPayprofitQuery,
    useLazyGetDuplicateSSNsQuery,
    useLazyGetDuplicateNamesAndBirthdaysQuery,
    useLazyGetMilitaryAndRehireForfeituresQuery,
    useLazyGetMilitaryAndRehireProfitSummaryQuery,
    useLazyGetMilitaryAndRehireQuery,
    useLazyGetNamesMissingCommasQuery,
    useLazyGetNegativeEVTASSNQuery,
    useLazyGetDistributionsAndForfeituresQuery,
    useLazyGetExecutiveHoursAndDollarsQuery,
    useLazyGetEligibleEmployeesQuery,
    useLazyGetDistributionsByAgeQuery,
    useLazyGetContributionsByAgeQuery,
    useLazyGetForfeituresByAgeQuery,
    useLazyGetBalanceByAgeQuery,
    useLazyGetBalanceByYearsQuery,
    useLazyGetProfitMasterInquiryQuery,
    useLazyGetVestingAmountByAgeQuery,
    useLazyGetTerminationReportQuery,
    useLazyGetProfitShareUpdateQuery,
    useLazyGetProfitShareEditQuery,
    useLazyGetMasterApplyQuery,
    useLazyGetMasterRevertQuery,
} = YearsEndApi;
