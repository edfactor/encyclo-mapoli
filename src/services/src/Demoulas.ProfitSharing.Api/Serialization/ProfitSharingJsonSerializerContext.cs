using System.Text.Json.Serialization;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Api.Serialization;

/// <summary>
/// JSON source generation context for Profit Sharing API responses.
/// This provides compile-time JSON serialization for better performance and reduced reflection overhead.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
// Common response types (generics)
[JsonSerializable(typeof(PaginatedResponseDto<MemberDetails>))]
[JsonSerializable(typeof(PaginatedResponseDto<MasterInquiryResponseDto>))]
[JsonSerializable(typeof(PaginatedResponseDto<GroupedProfitSummaryDto>))]
[JsonSerializable(typeof(PaginatedResponseDto<BeneficiarySearchFilterResponse>))]
[JsonSerializable(typeof(PaginatedResponseDto<DistributionSearchResponse>))]
[JsonSerializable(typeof(PaginatedResponseDto<MilitaryContributionResponse>))]
[JsonSerializable(typeof(PaginatedResponseDto<FrozenStateResponse>))]
[JsonSerializable(typeof(PaginatedResponseDto<ProfitSharingLabelResponse>))]
[JsonSerializable(typeof(PaginatedResponseDto<NewProfitSharingLabelResponse>))]
[JsonSerializable(typeof(PaginatedResponseDto<PayBenReportResponse>))]
[JsonSerializable(typeof(ReportResponseBase<AdhocTerminatedEmployeeResponse>))]
[JsonSerializable(typeof(ReportResponseBase<UnforfeituresResponse>))]
[JsonSerializable(typeof(ReportResponseBase<WagesCurrentYearResponse>))]
[JsonSerializable(typeof(ReportResponseBase<ExecutiveHoursAndDollarsResponse>))]
[JsonSerializable(typeof(ReportResponseBase<EligibleEmployee>))]
[JsonSerializable(typeof(ReportResponseBase<ProfitShareEditMemberRecordResponse>))]
[JsonSerializable(typeof(ReportResponseBase<ProfitShareUpdateMemberResponse>))]
// Post-Frozen Reports
[JsonSerializable(typeof(ProfitSharingUnder21ReportResponse))]
[JsonSerializable(typeof(ProfitSharingUnder21BreakdownByStoreResponse))]
[JsonSerializable(typeof(ProfitSharingUnder21InactiveNoBalanceResponse))]
[JsonSerializable(typeof(ProfitSharingUnder21TotalsResponse))]
[JsonSerializable(typeof(ProfitSharingLabelResponse))]
[JsonSerializable(typeof(NewProfitSharingLabelResponse))]
[JsonSerializable(typeof(ProfitControlSheetResponse))]
// Year-End Frozen Reports
[JsonSerializable(typeof(BalanceByAge))]
[JsonSerializable(typeof(ContributionsByAge))]
[JsonSerializable(typeof(DistributionsByAge))]
[JsonSerializable(typeof(ForfeituresByAge))]
[JsonSerializable(typeof(BalanceByYears))]
[JsonSerializable(typeof(VestedAmountsByAge))]
[JsonSerializable(typeof(YearEndProfitSharingReportResponse))]
[JsonSerializable(typeof(YearEndProfitSharingReportSummaryResponse))]
[JsonSerializable(typeof(GrossWagesReportResponse))]
[JsonSerializable(typeof(UpdateSummaryReportResponse))]
[JsonSerializable(typeof(ProfitShareUpdateResponse))]
[JsonSerializable(typeof(ForfeituresAndPointsForYearResponseWithTotals))]
// Year-End Operations
[JsonSerializable(typeof(ProfitMasterUpdateResponse))]
[JsonSerializable(typeof(ProfitMasterRevertResponse))]
[JsonSerializable(typeof(SuggestedForfeitureAdjustmentResponse))]
[JsonSerializable(typeof(GrandTotalsByStoreResponseDto))]
[JsonSerializable(typeof(GetEligibleEmployeesResponse))]
[JsonSerializable(typeof(ProfitShareEditResponse))]
[JsonSerializable(typeof(WagesCurrentYearResponse))]
[JsonSerializable(typeof(UnforfeituresResponse))]
[JsonSerializable(typeof(ExecutiveHoursAndDollarsResponse))]
[JsonSerializable(typeof(DistributionsAndForfeitureTotalsResponse))]
[JsonSerializable(typeof(DistributionsAndForfeitureResponse))]
// Master Inquiry
[JsonSerializable(typeof(MemberDetails))]
[JsonSerializable(typeof(MemberProfitPlanDetails))]
[JsonSerializable(typeof(MasterInquiryResponseDto))]
[JsonSerializable(typeof(GroupedProfitSummaryDto))]
// Beneficiaries
[JsonSerializable(typeof(BeneficiaryResponse))]
[JsonSerializable(typeof(BeneficiaryDetailResponse))]
[JsonSerializable(typeof(BeneficiarySearchFilterResponse))]
[JsonSerializable(typeof(BeneficiaryTypesResponseDto))]
[JsonSerializable(typeof(BeneficiaryKindResponseDto))]
[JsonSerializable(typeof(UpdateBeneficiaryResponse))]
[JsonSerializable(typeof(UpdateBeneficiaryContactResponse))]
[JsonSerializable(typeof(CreateBeneficiaryResponse))]
[JsonSerializable(typeof(CreateBeneficiaryContactResponse))]
// Distributions
[JsonSerializable(typeof(DistributionSearchResponse))]
[JsonSerializable(typeof(CreateOrUpdateDistributionResponse))]
// Adhoc Reports
[JsonSerializable(typeof(AdhocTerminatedEmployeeResponse))]
[JsonSerializable(typeof(AdhocBeneficiariesReportResponse))]
[JsonSerializable(typeof(PayBenReportResponse))]
// Lookups
[JsonSerializable(typeof(StateTaxLookupResponse))]
[JsonSerializable(typeof(CalendarResponseDto))]
[JsonSerializable(typeof(CommentTypeResponse))]
// Navigation
[JsonSerializable(typeof(NavigationResponseDto))]
[JsonSerializable(typeof(GetNavigationStatusResponseDto))]
[JsonSerializable(typeof(UpdateNavigationStatusResponseDto))]
// Military
[JsonSerializable(typeof(MilitaryContributionResponse))]
// IT Operations
[JsonSerializable(typeof(FrozenStateResponse))]
// Validation
[JsonSerializable(typeof(ChecksumValidationResponse))]
// Other
[JsonSerializable(typeof(IdsResponse))]
[JsonSerializable(typeof(BalanceEndpointResponse))]
[JsonSerializable(typeof(CertificateReprintResponse))]
public partial class ProfitSharingJsonSerializerContext : JsonSerializerContext
{
}
