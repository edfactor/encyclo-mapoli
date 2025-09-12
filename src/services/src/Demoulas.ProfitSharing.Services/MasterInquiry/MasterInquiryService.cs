using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

public sealed class MasterInquiryService : IMasterInquiryService
{
    #region Private DTO

    private sealed class MasterInquiryItem
    {
        public required ProfitDetail? ProfitDetail { get; init; }
        public required InquiryDemographics Member { get; init; }
        public required ProfitCode? ProfitCode { get; init; }
        public required ZeroContributionReason? ZeroContributionReason { get; init; }
        public required TaxCode? TaxCode { get; init; }
        public required CommentType? CommentType { get; init; }
        public DateTimeOffset TransactionDate { get; init; }
    }

    public sealed class InquiryDemographics
    {
        public int BadgeNumber { get; init; }
        public required string FullName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }

        public byte PayFrequencyId { get; init; }
        public short PsnSuffix { get; init; }
        public int Ssn { get; init; }
        public decimal CurrentIncomeYear { get; init; }
        public decimal CurrentHoursYear { get; init; }
        public int Id { get; set; }
        public bool IsExecutive { get; set; }
    }

    // Internal DTO for SQL-translatable projection
    private sealed class MasterInquiryRawDto
    {
        public int Id { get; init; }
        public int Ssn { get; init; }
        public short ProfitYear { get; init; }
        public byte ProfitYearIteration { get; init; }
        public int DistributionSequence { get; init; }
        public byte ProfitCodeId { get; init; }
        public string ProfitCodeName { get; init; } = string.Empty;
        public decimal Contribution { get; init; }
        public decimal Earnings { get; init; }
        public decimal Forfeiture { get; init; }
        public byte MonthToDate { get; init; }
        public short YearToDate { get; init; }
        public string? Remark { get; init; }
        public byte? ZeroContributionReasonId { get; init; }
        public string? ZeroContributionReasonName { get; init; }
        public decimal FederalTaxes { get; init; }
        public decimal StateTaxes { get; init; }
        public char? TaxCodeId { get; init; }
        public string? TaxCodeName { get; init; }
        public int? CommentTypeId { get; init; }
        public string? CommentTypeName { get; init; }
        public string? CommentRelatedCheckNumber { get; init; }
        public string? CommentRelatedState { get; init; }
        public long? CommentRelatedOracleHcmId { get; init; }
        public short? CommentRelatedPsnSuffix { get; init; }
        public bool? CommentIsPartialTransaction { get; init; }
        public int BadgeNumber { get; init; }
        public short PsnSuffix { get; init; }
        public byte PayFrequencyId { get; init; }
        public DateTimeOffset TransactionDate { get; init; }
        public decimal CurrentIncomeYear { get; init; }
        public decimal CurrentHoursYear { get; init; }
        public decimal Payment { get; set; }
        public bool IsExecutive { get; set; }
    }


    #endregion

    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;
    private readonly ITotalService _totalService;
    private readonly IMissiveService _missiveService;
    private readonly IDemographicReaderService _demographicReaderService;

    public MasterInquiryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory,
        IMissiveService missiveService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _missiveService = missiveService;
        _demographicReaderService = demographicReaderService;
        _logger = loggerFactory.CreateLogger<MasterInquiryService>();
    }


    public async Task<PaginatedResponseDto<MemberDetails>> GetMembersAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => (await GetMasterInquiryDemographics(ctx)).Union(GetMasterInquiryBeneficiary(ctx))
            };

            query = FilterMemberQuery(req, query);

            // Get unique SSNs from the query
            var ssnList = await query.Select(x => x.Member.Ssn).ToHashSetAsync(cancellationToken);

            if (ssnList.Count == 0 && (req.Ssn != 0 || req.BadgeNumber != 0))
            {
                // If an exact match is found, then the bene or empl is added to the ssnList.
                await HandleExactBadgeOrSsn(ctx, ssnList, req.BadgeNumber, req.PsnSuffix, req.Ssn);
            }

            short currentYear = req.ProfitYear;
            short previousYear = (short)(currentYear - 1);
            var memberType = req.MemberType;
            PaginatedResponseDto<MemberDetails> detailsList;

            if (memberType == 1)
            {
                detailsList = await GetDemographicDetailsForSsns(ctx, req, ssnList, currentYear, previousYear, cancellationToken);
            }
            else if (memberType == 2)
            {
                detailsList = await GetBeneficiaryDetailsForSsns(ctx, req, ssnList, cancellationToken);
            }
            else
            {
                // For both, merge and deduplicate by SSN
                var employeeDetails = await GetAllDemographicDetailsForSsns(ctx, ssnList, currentYear, previousYear, cancellationToken);
                var beneficiaryDetails = await GetAllBeneficiaryDetailsForSsns(ctx, ssnList, cancellationToken);

                // Combine and deduplicate by SSN
                var allResults = employeeDetails.Concat(beneficiaryDetails)
                    .GroupBy(d => d.Ssn)
                    .Select(g => g.First())
                    .ToList();

                // Apply sorting based on request
                var sortedResults = ApplySorting(allResults.AsQueryable(), req).ToList();

                // Apply pagination to the final deduplicated result set
                var skip = req.Skip ?? 0;
                var take = req.Take ?? 25;
                var paginatedResults = sortedResults.Skip(skip).Take(take).ToList();

                detailsList = new PaginatedResponseDto<MemberDetails>(req) { Results = paginatedResults, Total = sortedResults.Count };
            }

            foreach (MemberDetails details in detailsList.Results)
            {

                details.Age = details.DateOfBirth.Age();
            }

            return detailsList;
        });
    }

    /* This handles the case where we are given an exact badge or ssn and there are no PROFIT_DETAIL rows */
    private async Task HandleExactBadgeOrSsn(ProfitSharingReadOnlyDbContext ctx, HashSet<int> ssnList, int? badgeNumber, short? psnSuffix, int ssn)
    {

        // Some Members do nnt have Transactions yet (aka new employees, or new Bene) - so if we are asked about a specific psn/badge, we handle that here.
        if (ssnList.Count == 0 && (ssn != 0 || badgeNumber != 0))
        {
            if (ssn != 0)
            {
                // If they gave us the ssn... then lets use that.
                ssnList.Add(ssn);
            }
            // If they gave us the badge... then lets use that.
            else if (psnSuffix > 0)
            {
                int ssnBene = ctx.Beneficiaries.Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => new { b, bc })
                    .Where(bene => bene.b.BadgeNumber == badgeNumber && bene.b.PsnSuffix == psnSuffix)
                    .Select(d => d.bc.Ssn).SingleOrDefault();
                if (ssnBene != 0)
                {
                    ssnList.Add(ssnBene);
                }
            }
            else if (badgeNumber != 0)
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                int ssnEmpl = demographics.Where(d => d.BadgeNumber == badgeNumber)
                    .Select(d => d.Ssn).SingleOrDefault();
                if (ssnEmpl != 0)
                {
                    ssnList.Add(ssnEmpl);
                }
            }
        }
    }

    public async Task<PaginatedResponseDto<GroupedProfitSummaryDto>> GetGroupedProfitDetails(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        // These are the ProfitCode IDs used in GetProfitCodesForBalanceCalc()
        byte[] balanceProfitCodes = new byte[]
        {
            ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id, ProfitCode.Constants.OutgoingForfeitures.Id, ProfitCode.Constants.OutgoingDirectPayments.Id,
            ProfitCode.Constants.OutgoingXferBeneficiary.Id, ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
        };

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => (await GetMasterInquiryDemographics(ctx)).Union(GetMasterInquiryBeneficiary(ctx))
            };

            query = FilterMemberQuery(req, query);

            // Only group by non-null ProfitDetail
            query = query.Where(x => x.ProfitDetail != null);

            return await query
                .GroupBy(x => new
                {
                    ProfitYear = x.ProfitDetail != null ? x.ProfitDetail.ProfitYear : (short)0, MonthToDate = x.ProfitDetail != null ? x.ProfitDetail.MonthToDate : (byte)0
                })
                .Select(g => new GroupedProfitSummaryDto
                {
                    ProfitYear = g.Key.ProfitYear,
                    MonthToDate = g.Key.MonthToDate,
                    TotalContribution = g.Sum(x => x.ProfitDetail != null ? x.ProfitDetail.Contribution : 0),
                    TotalEarnings = g.Sum(x => x.ProfitDetail != null ? x.ProfitDetail.Earnings : 0),
                    TotalForfeiture = g.Sum(x =>
                        x.ProfitDetail != null && !balanceProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0),
                    TotalPayment = g.Sum(x =>
                        x.ProfitDetail != null && balanceProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0),
                    TransactionCount = g.Count()
                })
                .OrderBy(x => x.ProfitYear)
                .ThenBy(x => x.MonthToDate)
                .ToPaginationResultsAsync(req, cancellationToken);
        });
    }


    public async Task<MemberProfitPlanDetails?> GetMemberVestingAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default)
    {
        short currentYear = req.ProfitYear;
        short previousYear = (short)(currentYear - 1);
        var members = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return req.MemberType switch
            {
                1 => GetDemographicDetails(ctx, req.Id, currentYear, previousYear, cancellationToken),
                2 => GetBeneficiaryDetails(ctx, req.Id, cancellationToken),
                _ => throw new ValidationException("Invalid MemberType provided")
            };
        });
        Dictionary<int, MemberDetails> memberDetailsMap = new Dictionary<int, MemberDetails> { { members.ssn, members.memberDetails ?? new MemberDetails { Id = 0 } } };

        var details = await GetVestingDetails(memberDetailsMap, currentYear, previousYear, cancellationToken);
        return details.FirstOrDefault();
    }

    public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => (await GetMasterInquiryDemographics(ctx)).Union(GetMasterInquiryBeneficiary(ctx))
            };

            var masterInquiryRequest = new MasterInquiryRequest
            {
                MemberType = req.MemberType,
                BadgeNumber = req.BadgeNumber,
                Ssn = !string.IsNullOrWhiteSpace(req.Ssn) ? int.Parse(req.Ssn) : 0,
                EndProfitYear = req.EndProfitYear,
                StartProfitMonth = req.StartProfitMonth,
                EndProfitMonth = req.EndProfitMonth,
                ProfitCode = req.ProfitCode,
                ContributionAmount = req.ContributionAmount,
                EarningsAmount = req.EarningsAmount,
                ForfeitureAmount = req.ForfeitureAmount,
                PaymentAmount = req.PaymentAmount,
                Name = req.Name,
                PaymentType = req.PaymentType
            };

            query = FilterMemberQuery(masterInquiryRequest, query);

            if (req.Id.HasValue)
            {
                query = query.Where(x => x.Member.Id == req.Id.Value);
            }

            if (req.ProfitYear.HasValue)
            {
                query = query.Where(x => x.ProfitDetail != null && x.ProfitDetail.ProfitYear == req.ProfitYear.Value);
            }

            if (req.MonthToDate.HasValue)
            {
                query = query.Where(x => x.ProfitDetail != null && x.ProfitDetail.MonthToDate == req.MonthToDate.Value);
            }

            var paymentProfitCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

            // First projection: SQL-translatable only
            var rawQuery = await query.Select(x => new MasterInquiryRawDto
            {
                Id = x.ProfitDetail != null ? x.ProfitDetail.Id : 0,
                Ssn = x.ProfitDetail != null ? x.ProfitDetail.Ssn : x.Member.Ssn,
                ProfitYear = x.ProfitDetail != null ? x.ProfitDetail.ProfitYear : (short)0,
                ProfitYearIteration = x.ProfitDetail != null ? x.ProfitDetail.ProfitYearIteration : (byte)0,
                DistributionSequence = x.ProfitDetail != null ? x.ProfitDetail.DistributionSequence : 0,
                ProfitCodeId = x.ProfitDetail != null ? x.ProfitDetail.ProfitCodeId : (byte)0,
                ProfitCodeName = x.ProfitCode != null ? x.ProfitCode.Name : string.Empty,
                Contribution = x.ProfitDetail != null ? x.ProfitDetail.Contribution : 0,
                Earnings = x.ProfitDetail != null ? x.ProfitDetail.Earnings : 0,
                Forfeiture = x.ProfitDetail != null ? !paymentProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0 : 0,
                Payment = x.ProfitDetail != null ? paymentProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0 : 0,
                MonthToDate = x.ProfitDetail != null ? x.ProfitDetail.MonthToDate : (byte)0,
                YearToDate = x.ProfitDetail != null ? x.ProfitDetail.YearToDate : (short)0,
                Remark = x.ProfitDetail != null ? x.ProfitDetail.Remark : null,
                ZeroContributionReasonId = x.ProfitDetail != null ? x.ProfitDetail.ZeroContributionReasonId : null,
                ZeroContributionReasonName = x.ZeroContributionReason != null ? x.ZeroContributionReason.Name : string.Empty,
                FederalTaxes = x.ProfitDetail != null ? x.ProfitDetail.FederalTaxes : 0,
                StateTaxes = x.ProfitDetail != null ? x.ProfitDetail.StateTaxes : 0,
                TaxCodeId = x.ProfitDetail != null && x.ProfitDetail.TaxCodeId != null ? x.ProfitDetail.TaxCodeId : TaxCode.Constants.Unknown.Id,
                TaxCodeName = x.TaxCode != null ? x.TaxCode.Name : TaxCode.Constants.Unknown.Name,
                CommentTypeId = x.ProfitDetail != null ? x.ProfitDetail.CommentTypeId : null,
                CommentTypeName = x.CommentType != null ? x.CommentType.Name : string.Empty,
                CommentRelatedCheckNumber = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedCheckNumber : null,
                CommentRelatedState = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedState : null,
                CommentRelatedOracleHcmId = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedOracleHcmId : null,
                CommentRelatedPsnSuffix = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedPsnSuffix : null,
                CommentIsPartialTransaction = x.ProfitDetail != null && x.ProfitDetail.CommentIsPartialTransaction != null ? x.ProfitDetail.CommentIsPartialTransaction : false,
                BadgeNumber = x.Member.BadgeNumber,
                PsnSuffix = x.Member.PsnSuffix,
                PayFrequencyId = x.Member.PayFrequencyId,
                TransactionDate = x.TransactionDate,
                CurrentIncomeYear = x.Member.CurrentIncomeYear,
                CurrentHoursYear = x.Member.CurrentHoursYear,
                IsExecutive = x.Member.IsExecutive
            }).ToPaginationResultsAsync(req, cancellationToken);

            var formattedResults = rawQuery.Results.Select(x => new MasterInquiryResponseDto
            {
                Id = x.Id,
                Ssn = x.Ssn.MaskSsn(),
                ProfitYear = x.ProfitYear,
                ProfitYearIteration = x.ProfitYearIteration,
                DistributionSequence = x.DistributionSequence,
                ProfitCodeId = x.ProfitCodeId,
                ProfitCodeName = x.ProfitCodeName,
                Contribution = x.Contribution,
                Earnings = x.Earnings,
                Forfeiture = x.Forfeiture,
                Payment = x.Payment,
                MonthToDate = x.MonthToDate,
                YearToDate = x.YearToDate,
                Remark = x.Remark,
                ZeroContributionReasonId = x.ZeroContributionReasonId,
                ZeroContributionReasonName = x.ZeroContributionReasonName,
                FederalTaxes = x.FederalTaxes,
                StateTaxes = x.StateTaxes,
                TaxCodeId = x.TaxCodeId,
                TaxCodeName = x.TaxCodeName,
                CommentTypeId = x.CommentTypeId,
                CommentTypeName = x.CommentTypeName,
                CommentRelatedCheckNumber = x.CommentRelatedCheckNumber,
                CommentRelatedState = x.CommentRelatedState,
                CommentRelatedOracleHcmId = x.CommentRelatedOracleHcmId,
                CommentRelatedPsnSuffix = x.CommentRelatedPsnSuffix,
                CommentIsPartialTransaction = x.CommentIsPartialTransaction,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                PayFrequencyId = x.PayFrequencyId,
                TransactionDate = x.TransactionDate,
                CurrentIncomeYear = x.CurrentIncomeYear,
                CurrentHoursYear = x.CurrentHoursYear,
                IsExecutive = x.IsExecutive
            });

            return new PaginatedResponseDto<MasterInquiryResponseDto>(req) { Results = formattedResults, Total = rawQuery.Total };
        });
    }

    private async Task<IQueryable<MasterInquiryItem>> GetMasterInquiryDemographics(ProfitSharingReadOnlyDbContext ctx)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var query = ctx.ProfitDetails
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .Join(demographics
                    .Include(d => d.PayProfits),
                pd => pd.Ssn,
                d => d.Ssn,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    TransactionDate = pd.CreatedAtUtc,
                    Member = new InquiryDemographics
                    {
                        Id = d.Id,
                        BadgeNumber = d.BadgeNumber,
                        FullName = d.ContactInfo.FullName != null ? d.ContactInfo.FullName : d.ContactInfo.LastName,
                        FirstName = d.ContactInfo.FirstName,
                        LastName = d.ContactInfo.LastName,
                        PayFrequencyId = d.PayFrequencyId,
                        Ssn = d.Ssn,
                        PsnSuffix = 0,
                        IsExecutive = d.PayFrequencyId ==PayFrequency.Constants.Monthly,
                        CurrentIncomeYear = d.PayProfits.Where(x => x.ProfitYear == pd.ProfitYear)
                            .Select(x => x.CurrentIncomeYear)
                            .FirstOrDefault(),
                        CurrentHoursYear = d.PayProfits.Where(x => x.ProfitYear == pd.ProfitYear)
                            .Select(x => x.CurrentHoursYear)
                            .FirstOrDefault()
                    }
                });

        return query;
    }

    private static IQueryable<MasterInquiryItem> GetMasterInquiryBeneficiary(ProfitSharingReadOnlyDbContext ctx)
    {
        var query = ctx.ProfitDetails
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .Join(ctx.Beneficiaries.Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => new { b, bc }),
                pd => pd.Ssn, bene => bene.bc.Ssn,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    TransactionDate = pd.CreatedAtUtc,
                    Member = new InquiryDemographics
                    {
                        Id = d.bc.Id,
                        BadgeNumber = d.b.BadgeNumber,
                        FullName = d.bc.ContactInfo.FullName != null ? d.bc.ContactInfo.FullName : d.bc.ContactInfo.LastName,
                        FirstName = d.bc.ContactInfo.FirstName,
                        LastName = d.bc.ContactInfo.LastName,
                        PayFrequencyId = 0,
                        Ssn = d.bc.Ssn,
                        PsnSuffix = d.b.PsnSuffix,
                        CurrentIncomeYear = 0,
                        CurrentHoursYear = 0,
                        IsExecutive = false,
                    }
                });

        return query;
    }
    
    private async Task<(int ssn, MemberDetails? memberDetails)> GetDemographicDetails(ProfitSharingReadOnlyDbContext ctx,
       int id, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var memberData = await demographics
            .Include(d => d.PayProfits)
            .ThenInclude(pp => pp.Enrollment)
            .Include(d => d.Department)
            .Include(d => d.TerminationCode)
            .Include(d => d.PayClassification)
            .Include(d => d.Gender)
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.ContactInfo.FirstName,
                d.ContactInfo.LastName,
                d.ContactInfo.PhoneNumber,
                d.Address.City,
                d.Address.State,
                Address = d.Address.Street,
                d.Address.PostalCode,
                d.DateOfBirth,
                d.Ssn,
                d.BadgeNumber,
                d.PayFrequencyId,
                d.ReHireDate,
                d.HireDate,
                d.TerminationDate,
                d.StoreNumber,
                DemographicId = d.Id,
                d.EmploymentStatusId,
                d.EmploymentStatus,
                IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                d.FullTimeDate,
                Department = d.Department != null ? d.Department.Name : "N/A",
                TerminationReason = d.TerminationCode != null ? d.TerminationCode.Name : "N/A",
                Gender = d.Gender != null ? d.Gender.Name : "N/A",
                PayClassification = d.PayClassification != null ? d.PayClassification.Name : "N/A",
                
                CurrentPayProfit = d.PayProfits
                    .Select(x=>
                        new
                        {
                            x.ProfitYear,
                            x.CurrentHoursYear,
                            x.Etva,
                            x.EnrollmentId,
                            x.Enrollment
                        })
                    .FirstOrDefault(x => x.ProfitYear == currentYear),
                PreviousPayProfit = d.PayProfits
                    .Select(x =>
                        new
                        {
                            x.ProfitYear,
                            x.CurrentHoursYear,
                            x.Etva,
                            x.EnrollmentId,
                            x.Enrollment,
                            x.PsCertificateIssuedDate
                        })
                    .FirstOrDefault(x => x.ProfitYear == previousYear)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (memberData == null)
        {
            return (0, new MemberDetails { Id = 0 });
        }

        var missives = await _missiveService.DetermineMissivesForSsns([memberData.Ssn], currentYear, cancellationToken);
        var missiveList = missives.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();

        return (ssn: memberData.Ssn, memberDetails: new MemberDetails
        {
            IsEmployee = true,
            Id = memberData.Id,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Age = memberData.DateOfBirth.Age(),
            Ssn = memberData.Ssn.MaskSsn(),
            YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
            HireDate = memberData.HireDate,
            ReHireDate = memberData.ReHireDate,
            FullTimeDate = memberData.FullTimeDate,
            TerminationDate = memberData.TerminationDate,
            StoreNumber = memberData.StoreNumber,
            EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
            Enrollment = memberData.CurrentPayProfit?.Enrollment?.Name,
            BadgeNumber = memberData.BadgeNumber,
            PayFrequencyId = memberData.PayFrequencyId,
            IsExecutive = memberData.IsExecutive,
            CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
            PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
            
            EmploymentStatus = memberData.EmploymentStatus?.Name,
            
            Department = memberData.Department,
            TerminationReason = memberData.TerminationReason,
            Gender = memberData.Gender,
            PayClassification = memberData.PayClassification,
            PhoneNumber = memberData.PhoneNumber,

            ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
            Missives = missiveList
        });
    }

    private async Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetails(ProfitSharingReadOnlyDbContext ctx,
     int id, CancellationToken cancellationToken)
    {
        var memberData = await ctx.Beneficiaries
            .Include(b => b.Contact)
            .Where(b => b.Id == id)
            .Select(b => new
            {
                b.Id,
                b.Contact!.ContactInfo.FirstName,
                b.Contact.ContactInfo.LastName,
                b.Contact.Address.City,
                b.Contact.Address.State,
                Address = b.Contact.Address.Street,
                b.Contact.Address.PostalCode,
                b.Contact.DateOfBirth,
                b.Contact.Ssn,
                b.BadgeNumber,
                b.PsnSuffix,
                b.DemographicId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (memberData == null)
        {
            return (0, new MemberDetails{ Id = 0 });
        }

       
        return (memberData.Ssn, new MemberDetails
        {
            IsEmployee = false,
            Id = memberData.Id,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Age = memberData.DateOfBirth.Age(),
            Ssn = memberData.Ssn.MaskSsn(),
            BadgeNumber = memberData.BadgeNumber,
            PsnSuffix = memberData.PsnSuffix,
            PayFrequencyId = 0,
            IsExecutive = false,
        });
    }

    private async Task<IEnumerable<MemberProfitPlanDetails>> GetVestingDetails(Dictionary<int, MemberDetails> memberDetailsMap,
        short currentYear, 
        short previousYear, 
        CancellationToken cancellationToken)
    {
        // Here we recognize 2024 as the transition year to relying on the SMART YE Process
        bool isPreviousYearEndComplete = (previousYear < ReferenceData.SmartTransitionYear) || await _dataContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.YearEndUpdateStatuses
                .AnyAsync(x => x.ProfitYear == previousYear && x.IsYearEndCompleted, cancellationToken));
        bool isProfitYearYearEndComplete = (currentYear < ReferenceData.SmartTransitionYear) || await _dataContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.YearEndUpdateStatuses
                .AnyAsync(x => x.ProfitYear == currentYear && x.IsYearEndCompleted, cancellationToken));
        bool isWallClockYear = currentYear == DateTime.Now.Year;
        
        var ssnCollection = memberDetailsMap.Keys.ToHashSet();
        List<BalanceEndpointResponse> currentBalance = [];
        List<BalanceEndpointResponse> previousBalance = [];
        try
        {
            var previousBalanceTask =
                _totalService.GetVestingBalanceForMembersAsync(
                    SearchBy.Ssn, ssnCollection, previousYear, cancellationToken);

            var currentBalanceTask =
                _totalService.GetVestingBalanceForMembersAsync(
                    SearchBy.Ssn, ssnCollection, currentYear, cancellationToken);

            await Task.WhenAll(previousBalanceTask, currentBalanceTask);

            currentBalance = await currentBalanceTask;
            previousBalance = await previousBalanceTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve balances for SSN {SSN}", ssnCollection);
        }

        var detailsList = new List<MemberProfitPlanDetails>(memberDetailsMap.Count);

        foreach (var kvp in memberDetailsMap)
        {
            var memberData = kvp.Value;
            var balance = currentBalance.FirstOrDefault(b => b.Id == kvp.Key, new BalanceEndpointResponse { Id = kvp.Key, Ssn = memberData.Ssn});
            var previousBalanceItem = previousBalance.FirstOrDefault(b => b.Id == kvp.Key);

            detailsList.Add(new MemberProfitPlanDetails
            {
                IsEmployee = memberData.IsEmployee,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.AddressCity,
                AddressState = memberData.AddressState,
                Address = memberData.Address,
                AddressZipCode = memberData.AddressZipCode,
                DateOfBirth = memberData.DateOfBirth,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn,
                IsExecutive = memberData.IsExecutive,
                YearToDateProfitSharingHours = memberData.YearToDateProfitSharingHours,
                HireDate = memberData.HireDate,
                ReHireDate = memberData.ReHireDate,
                TerminationDate = memberData.TerminationDate,
                StoreNumber = memberData.StoreNumber,
                EnrollmentId = memberData.EnrollmentId,
                Enrollment = memberData.Enrollment,
                CurrentEtva = memberData.CurrentEtva,
                PreviousEtva = memberData.PreviousEtva,
                EmploymentStatus = memberData.EmploymentStatus,
                Missives = memberData.Missives,
                YearsInPlan = balance?.YearsInPlan ?? 0,
                PercentageVested = balance?.VestingPercent ?? 0,
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                BeginPSAmount = isPreviousYearEndComplete ? (previousBalanceItem?.CurrentBalance ?? 0) : null,
                // "Current" is really "Now" or "At end of Year End"
                CurrentPSAmount =  isWallClockYear || isProfitYearYearEndComplete ? (balance?.CurrentBalance ?? 0) : null,
                BeginVestedAmount = isPreviousYearEndComplete ? (previousBalanceItem?.VestedBalance ?? 0) : null,
                // "Current" is really "Now" or "At end of Year End"
                CurrentVestedAmount = isWallClockYear || isProfitYearYearEndComplete ? (balance?.VestedBalance ?? 0) : null,

                FullTimeDate = memberData.FullTimeDate,
                Department = memberData.Department,
                TerminationReason = memberData.TerminationReason,
                Gender = memberData.Gender,
                PayClassification = memberData.PayClassification,
                
                AllocationToAmount = balance?.AllocationsToBeneficiary ?? 0,
                AllocationFromAmount = balance?.AllocationsFromBeneficiary ?? 0,
                ReceivedContributionsLastYear = isPreviousYearEndComplete ? memberData.ReceivedContributionsLastYear : null
            });
        }

        return detailsList;
    }

    private async Task<PaginatedResponseDto<MemberDetails>> GetDemographicDetailsForSsns(ProfitSharingReadOnlyDbContext ctx, SortedPaginationRequestDto req, ISet<int> ssns, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var query = demographics
            .Include(d => d.PayProfits)
            .ThenInclude(pp => pp.Enrollment)
            .Where(d => ssns.Contains(d.Ssn));

        if (((MasterInquiryRequest)req).BadgeNumber.HasValue && ((MasterInquiryRequest)req).BadgeNumber != 0)
        {
            query = query.Where(d => d.BadgeNumber == ((MasterInquiryRequest)req).BadgeNumber);
        }

        var members = await query
            .Select(d => new
            {
            d.Id,
            d.ContactInfo.FullName,
            d.ContactInfo.FirstName,
            d.ContactInfo.LastName,
            d.Address.City,
            d.Address.State,
            Address = d.Address.Street,
            d.Address.PostalCode,
            d.DateOfBirth,
            d.Ssn,
            d.BadgeNumber,
            d.PayFrequencyId,
            d.ReHireDate,
            d.HireDate,
            d.TerminationDate,
            d.StoreNumber,
            DemographicId = d.Id,
            d.EmploymentStatusId,
            d.EmploymentStatus,
            IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
            CurrentPayProfit = d.PayProfits.Select(x =>
                new
                {
                x.ProfitYear,
                x.CurrentHoursYear,
                x.Etva,
                x.EnrollmentId,
                x.Enrollment
                }).FirstOrDefault(x => x.ProfitYear == currentYear),
            PreviousPayProfit = d.PayProfits.Select(x =>
                new
                {
                x.ProfitYear,
                x.CurrentHoursYear,
                x.Etva,
                x.EnrollmentId,
                x.Enrollment,
                x.PsCertificateIssuedDate
                }).FirstOrDefault(x => x.ProfitYear == previousYear)
            })
            .ToPaginationResultsAsync(req, cancellationToken);

        var missivesDict = await _missiveService.DetermineMissivesForSsns(members.Results.Select(m => m.Ssn), currentYear, cancellationToken);

        var detailsList = new List<MemberDetails>();
        foreach (var memberData in members.Results)
        {
            var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();
            detailsList.Add(new MemberDetails
            {
                IsEmployee = true,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                IsExecutive = memberData.IsExecutive,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn.MaskSsn(),
                YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
                HireDate = memberData.HireDate,
                ReHireDate = memberData.ReHireDate,
                TerminationDate = memberData.TerminationDate,
                StoreNumber = memberData.StoreNumber,
                EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
                Enrollment = memberData.CurrentPayProfit?.Enrollment?.Name,
                BadgeNumber = memberData.BadgeNumber,
                PayFrequencyId = memberData.PayFrequencyId,
                CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
                PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
                EmploymentStatus = memberData.EmploymentStatus?.Name,
                ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                Missives = missiveList
            });
        }

        return new PaginatedResponseDto<MemberDetails>(req) { Results = detailsList, Total = members.Total };
    }

    private Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsns(ProfitSharingReadOnlyDbContext ctx,
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken)
    {
        var membersQuery = ctx.Beneficiaries
            .Include(b => b.Contact)
            .Where(b => b.Contact != null && ssns.Contains(b.Contact.Ssn));

        // Only filter by BadgeNumber if provided and not 0
        var badgeNumber = ((MasterInquiryRequest)req).BadgeNumber;
        if (badgeNumber.HasValue && badgeNumber != 0)
        {
            membersQuery = membersQuery.Where(b => b.BadgeNumber == badgeNumber);
        }

        var members = membersQuery
            .Select(b => new
            {
            b.Id,
            b.Contact!.ContactInfo.FullName,
            b.Contact!.ContactInfo.FirstName,
            b.Contact.ContactInfo.LastName,
            b.Contact.Address.City,
            b.Contact.Address.State,
            Address = b.Contact.Address.Street,
            b.Contact.Address.PostalCode,
            b.Contact.DateOfBirth,
            b.Contact.Ssn,
            b.BadgeNumber,
            b.PsnSuffix,
            DemographicId = b.Id
            });


        return members.Select(memberData => new MemberDetails
            {
                Id = memberData.Id,
                IsEmployee = false,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Ssn = memberData.Ssn.MaskSsn(),
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                IsExecutive = false,
            })
            .ToPaginationResultsAsync(req, cancellationToken);
    }

    private static IQueryable<MasterInquiryItem> FilterMemberQuery(MasterInquiryRequest req, IQueryable<MasterInquiryItem> query)
    {
        if (req.BadgeNumber.HasValue && req.BadgeNumber > 0 )
        {
            query = query.Where(x => x.Member.BadgeNumber == req.BadgeNumber);
        }

        if (req.MemberType != 1 /* Employee Only */ && req.PsnSuffix > 0)
        {
            query = query.Where(x => x.Member.PsnSuffix == req.PsnSuffix);
        }

        if (req.EndProfitYear.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.ProfitYear <= req.EndProfitYear));
        }

        if (req.StartProfitMonth.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.MonthToDate >= req.StartProfitMonth));
        }

        if (req.EndProfitMonth.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.MonthToDate <= req.EndProfitMonth));
        }

        if (req.ProfitCode.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId == req.ProfitCode));
        }

        if (req.ContributionAmount.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.Contribution == req.ContributionAmount));
        }

        if (req.EarningsAmount.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.Earnings == req.EarningsAmount));
        }

        if (req.Ssn != 0)
        {
            query = query.Where(x => (x.Member.Ssn == req.Ssn));
        }

        if (req.ForfeitureAmount.HasValue)
        {
            query = query.Where(x =>
                (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id) &&
                (x.ProfitDetail == null || x.ProfitDetail.Forfeiture == req.ForfeitureAmount));
        }

        if (req.PaymentAmount.HasValue)
        {
            query = query.Where(x =>
                (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id) &&
                (x.ProfitDetail == null || x.ProfitDetail.Forfeiture == req.PaymentAmount));
        }

        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            var pattern = $"%{req.Name.ToUpperInvariant()}%";
            query = query.Where(x => EF.Functions.Like(x.Member.FullName.ToUpper(), pattern));
        }

        if (req.PaymentType.HasValue)
        {
            switch (req.PaymentType)
            {
                case 1: // Hardship/Distribution
                    List<byte?> array = [CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 2: // payoffs
                    array = [CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 3: // rollovers
                    array = [CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
            }
        }

        return query;
    }

    private async Task<List<MemberDetails>> GetAllDemographicDetailsForSsns(ProfitSharingReadOnlyDbContext ctx, ISet<int> ssns, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var query = demographics
            .Include(d => d.PayProfits)
            .ThenInclude(pp => pp.Enrollment)
            .Where(d => ssns.Contains(d.Ssn));

        var members = await query
            .Select(d => new
            {
            d.Id,
            d.ContactInfo.FirstName,
            d.ContactInfo.LastName,
            d.Address.City,
            d.Address.State,
            Address = d.Address.Street,
            d.Address.PostalCode,
            d.DateOfBirth,
            d.Ssn,
            d.BadgeNumber,
            d.PayFrequencyId,
            d.ReHireDate,
            d.HireDate,
            d.TerminationDate,
            d.StoreNumber,
            DemographicId = d.Id,
            d.EmploymentStatusId,
            d.EmploymentStatus,
            IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
            CurrentPayProfit = d.PayProfits.Select(x =>
                new
                {
                x.ProfitYear,
                x.CurrentHoursYear,
                x.Etva,
                x.EnrollmentId,
                x.Enrollment
                }).FirstOrDefault(x => x.ProfitYear == currentYear),
            PreviousPayProfit = d.PayProfits.Select(x =>
                new
                {
                x.ProfitYear,
                x.CurrentHoursYear,
                x.Etva,
                x.EnrollmentId,
                x.Enrollment,
                x.PsCertificateIssuedDate
                }).FirstOrDefault(x => x.ProfitYear == previousYear)
            })
            .ToListAsync(cancellationToken);

        var missivesDict = await _missiveService.DetermineMissivesForSsns(members.Select(m => m.Ssn), currentYear, cancellationToken);

        var detailsList = new List<MemberDetails>();
        foreach (var memberData in members)
        {
            var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();
            detailsList.Add(new MemberDetails
            {
                IsEmployee = true,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn.MaskSsn(),
                YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
                HireDate = memberData.HireDate,
                ReHireDate = memberData.ReHireDate,
                TerminationDate = memberData.TerminationDate,
                StoreNumber = memberData.StoreNumber,
                EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
                Enrollment = memberData.CurrentPayProfit?.Enrollment?.Name,
                BadgeNumber = memberData.BadgeNumber,
                PayFrequencyId = memberData.PayFrequencyId,
                CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
                PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
                EmploymentStatus = memberData.EmploymentStatus?.Name,
                ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                Missives = missiveList,
                IsExecutive = memberData.IsExecutive
            });
        }

        return detailsList;
    }

    private async Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsns(ProfitSharingReadOnlyDbContext ctx, ISet<int> ssns, CancellationToken cancellationToken)
    {
        var members = await ctx.Beneficiaries
            .Include(b => b.Contact)
            .Where(b => b.Contact != null && ssns.Contains(b.Contact.Ssn))
            .Select(b => new
            {
            b.Id,
            b.Contact!.ContactInfo.FirstName,
            b.Contact.ContactInfo.LastName,
            b.Contact.Address.City,
            b.Contact.Address.State,
            Address = b.Contact.Address.Street,
            b.Contact.Address.PostalCode,
            b.Contact.DateOfBirth,
            b.Contact.Ssn,
            b.BadgeNumber,
            b.PsnSuffix,
            DemographicId = b.Id
            })
            .ToListAsync(cancellationToken);

        var detailsList = new List<MemberDetails>();
        foreach (var memberData in members)
        {
            detailsList.Add(new MemberDetails
            {
                Id = memberData.Id,
                IsEmployee = false,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Ssn = memberData.Ssn.MaskSsn(),
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                IsExecutive = false,
            });
        }

        return detailsList;
    }

    private static IQueryable<MemberDetails> ApplySorting(IQueryable<MemberDetails> query, SortedPaginationRequestDto req)
    {
        if (string.IsNullOrEmpty(req.SortBy))
        {
            return query;
        }

        var isDescending = req.IsSortDescending ?? false;
        return req.SortBy.ToLower() switch
        {
            "fullname" => isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
            "ssn" => isDescending ? query.OrderByDescending(x => x.Ssn) : query.OrderBy(x => x.Ssn),
            "badgenumber" => isDescending ? query.OrderByDescending(x => x.BadgeNumber) : query.OrderBy(x => x.BadgeNumber),
            "address" => isDescending ? query.OrderByDescending(x => x.Address) : query.OrderBy(x => x.Address),
            "addresscity" => isDescending ? query.OrderByDescending(x => x.AddressCity) : query.OrderBy(x => x.AddressCity),
            "addressstate" => isDescending ? query.OrderByDescending(x => x.AddressState) : query.OrderBy(x => x.AddressState),
            "addresszipCode" => isDescending ? query.OrderByDescending(x => x.AddressZipCode) : query.OrderBy(x => x.AddressZipCode),
            "age" => isDescending ? query.OrderByDescending(x => x.Age) : query.OrderBy(x => x.Age),
            "employmentStatus" => isDescending ? query.OrderByDescending(x => x.EmploymentStatus) : query.OrderBy(x => x.EmploymentStatus),
            _ => query
        };
    }
}
