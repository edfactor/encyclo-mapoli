using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

/// <summary>
/// Service handling employee/demographic master inquiry operations.
/// Provides optimized queries for employee lookups with profit details.
/// </summary>
public sealed class EmployeeMasterInquiryService : IEmployeeMasterInquiryService
{
    private readonly ILogger<EmployeeMasterInquiryService> _logger;
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly IMissiveService _missiveService;
    private readonly IDemographicReaderService _demographicReaderService;

    public EmployeeMasterInquiryService(
        ILoggerFactory loggerFactory,
        IProfitSharingDataContextFactory factory,
        IMissiveService missiveService,
        IDemographicReaderService demographicReaderService)
    {
        _logger = loggerFactory.CreateLogger<EmployeeMasterInquiryService>();
        _factory = factory;
        _missiveService = missiveService;
        _demographicReaderService = demographicReaderService;
    }

    private static byte ResolveEnrollmentId(int? vestingScheduleId, bool hasForfeited)
    {
        if (!vestingScheduleId.HasValue || vestingScheduleId.Value == 0)
        {
            return EnrollmentConstants.NotEnrolled;
        }

        if (hasForfeited)
        {
            return vestingScheduleId.Value == VestingSchedule.Constants.OldPlan
                ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                : EnrollmentConstants.NewVestingPlanHasForfeitureRecords;
        }

        return vestingScheduleId.Value == VestingSchedule.Constants.OldPlan
            ? EnrollmentConstants.OldVestingPlanHasContributions
            : EnrollmentConstants.NewVestingPlanHasContributions;
    }

    public Task<IQueryable<MasterInquiryItem>> GetEmployeeInquiryQueryAsync(
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Building employee inquiry query. EndProfitYear: {EndProfitYear}, PaymentType: {PaymentType}, BadgeNumber: {BadgeNumber}",
            req?.EndProfitYear, req?.PaymentType, req?.BadgeNumber);

        return _factory.UseReadOnlyContext(async ctx =>
        {
            return await GetEmployeeInquiryQueryAsync(ctx, req, cancellationToken);
        }, cancellationToken);
    }

    public async Task<IQueryable<MasterInquiryItem>> GetEmployeeInquiryQueryAsync(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Building employee inquiry query (with context). EndProfitYear: {EndProfitYear}, PaymentType: {PaymentType}, BadgeNumber: {BadgeNumber}",
            req?.EndProfitYear, req?.PaymentType, req?.BadgeNumber);

        _logger.LogInformation("TRACE: About to call BuildDemographicQuery");
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx).ConfigureAwait(false);
        _logger.LogInformation("TRACE: BuildDemographicQuery completed");

        // ReadOnlyDbContext automatically handles AsSplitQuery and AsNoTracking
        // TagWith helps identify this query in profiling/logging
        IQueryable<ProfitDetail> profitDetailsQuery = ctx.ProfitDetails;
        _logger.LogInformation("TRACE: Got ProfitDetails query");

        // OPTIMIZATION: Pre-filter ProfitDetails before expensive join if we have selective criteria
        if (req?.EndProfitYear.HasValue == true)
        {
            profitDetailsQuery = profitDetailsQuery.Where(pd => pd.ProfitYear <= req.EndProfitYear.Value);
            _logger.LogDebug("Applied EndProfitYear filter: {EndProfitYear}", req.EndProfitYear.Value);
        }

        if (req?.PaymentType.HasValue == true)
        {
            // Apply payment type filter early for massive performance gain
            var commentTypeIds = req.PaymentType switch
            {
                1 => new byte?[] { CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id },
                2 => new byte?[] { CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id },
                3 => new byte?[] { CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id },
                _ => Array.Empty<byte?>()
            };

            if (commentTypeIds.Length > 0)
            {
                profitDetailsQuery = profitDetailsQuery.Where(pd => commentTypeIds.Contains(pd.CommentTypeId));
                _logger.LogDebug("Applied PaymentType filter: {PaymentType}, CommentTypeIds: {CommentTypeIds}",
                    req.PaymentType.Value, string.Join(", ", commentTypeIds.Where(id => id.HasValue).Select(id => id!.Value)));
            }
        }

        _logger.LogInformation("TRACE: About to build query with Include");
#pragma warning disable AsyncFixer02 // Long-running or blocking operations inside an async method
        var query = profitDetailsQuery
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .TagWith("EmployeeMasterInquiry: Get demographics with profit details")
            .Join(demographics,
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
                        IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                        EmploymentStatusId = d.EmploymentStatusId,
                        // Use correlated subqueries for PayProfits data
                        // These will be optimized by Oracle when IX_PayProfits_Demographic_Year index exists
                        CurrentIncomeYear = ctx.PayProfits
                            .Where(pp => pp.DemographicId == d.Id && pp.ProfitYear == pd.ProfitYear)
                            .Select(pp => pp.CurrentIncomeYear)
                            .FirstOrDefault(),
                        CurrentHoursYear = ctx.PayProfits
                            .Where(pp => pp.DemographicId == d.Id && pp.ProfitYear == pd.ProfitYear)
                            .Select(pp => pp.CurrentHoursYear)
                            .FirstOrDefault()
                    }
                });
#pragma warning restore AsyncFixer02 // Long-running or blocking operations inside an async method

        _logger.LogInformation("TRACE: Query built, returning");
        return query;
    }

    public async Task<(int ssn, MemberDetails? memberDetails)> GetEmployeeDetailsAsync(
        int id,
        short currentYear,
        short previousYear,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting employee details for ID: {EmployeeId}, CurrentYear: {CurrentYear}, PreviousYear: {PreviousYear}",
            id, currentYear, previousYear);

        return await _factory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var memberData = await demographics
                .Include(d => d.PayProfits)
                .Include(d => d.Department)
                .Include(d => d.TerminationCode)
                .Include(d => d.PayClassification)
                .Include(d => d.Gender)
                .Where(d => d.Id == id)
                .Select(d => new
                {
                    d.Id,
                    d.ContactInfo.FirstName,
                    d.ContactInfo.MiddleName,
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
                        .Select(x =>
                            new
                            {
                                x.ProfitYear,
                                x.CurrentHoursYear,
                                x.VestingScheduleId,
                                x.HasForfeited,
                                x.Etva,
                            })
                        .FirstOrDefault(x => x.ProfitYear == currentYear),
                    PreviousPayProfit = d.PayProfits
                        .Select(x =>
                            new
                            {
                                x.ProfitYear,
                                x.CurrentHoursYear,
                                x.Etva,
                                x.PsCertificateIssuedDate
                            })
                        .FirstOrDefault(x => x.ProfitYear == previousYear)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (memberData is null)
            {
                return (0, CreateEmptyMemberDetails());
            }

            var missivesDict = await _missiveService.DetermineMissivesForSsns([memberData.Ssn], currentYear, cancellationToken);
            var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var missives)
                ? missives
                : new List<int>();

            _logger.LogDebug("Found {MissiveCount} missives for SSN: {MaskedSsn}", missiveList.Count, memberData.Ssn.MaskSsn());

            var duplicateSsns = await demographics
                    .GroupBy(d => d.Ssn)
                    .Where(g => g.Count() > 1 && g.Key == memberData.Ssn)
                    .Select(g => g.Key)
                    .ToHashSetAsync(cancellationToken);

            List<int> badgeNumbersOfDuplicates = [];
            if (duplicateSsns.Any())
            {
                badgeNumbersOfDuplicates = await demographics
                    .Where(d => d.Ssn == memberData.Ssn && d.Id != memberData.DemographicId)
                    .Select(d => d.BadgeNumber)
                    .ToListAsync(cancellationToken);

                _logger.LogWarning("Duplicate SSN detected for Badge: {BadgeNumber}, SSN: {MaskedSsn}, Duplicate badges: {DuplicateBadges}",
                    memberData.BadgeNumber, memberData.Ssn.MaskSsn(), string.Join(", ", badgeNumbersOfDuplicates));
            }

            var enrollmentId = ResolveEnrollmentId(memberData.CurrentPayProfit?.VestingScheduleId,
                memberData.CurrentPayProfit?.HasForfeited ?? false);
            var enrollmentName = EnrollmentConstants.GetDescription(enrollmentId);

            return (memberData.Ssn, new MemberDetails
            {
                IsEmployee = true,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                MiddleName = memberData.MiddleName,
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
                EnrollmentId = enrollmentId,
                Enrollment = enrollmentName,
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
                WorkLocation = memberData.StoreNumber > 0 ? $"Store {memberData.StoreNumber}" : null,

                ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                Missives = missiveList,
                BadgesOfDuplicateSsns = badgeNumbersOfDuplicates
            });
        }, cancellationToken);
    }

    public Task<PaginatedResponseDto<MemberDetails>> GetEmployeeDetailsForSsnsAsync(
        MasterInquiryRequest req,
        ISet<int> ssns,
        short currentYear,
        short previousYear,
        ISet<int> duplicateSsns,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting paginated employee details for {SsnCount} SSNs. BadgeNumber: {BadgeNumber}, CurrentYear: {CurrentYear}",
            ssns.Count, req.BadgeNumber, currentYear);

        return _factory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

            // ReadOnlyDbContext automatically handles AsSplitQuery and AsNoTracking
            var query = demographics
                .Where(d => ssns.Contains(d.Ssn))
                .TagWith("EmployeeMasterInquiry: Get demographic details for SSNs");

            if (req.BadgeNumber.HasValue && req.BadgeNumber != 0)
            {
                query = query.Where(d => d.BadgeNumber == req.BadgeNumber);
            }

            // Optimize projection: only select needed fields, avoid loading unnecessary navigation properties
            var members = await query
                .Select(d => new
                {
                    d.Id,
                    d.ContactInfo.FullName,
                    d.ContactInfo.FirstName,
                    d.ContactInfo.MiddleName,
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
                    d.FullTimeDate,
                    d.TerminationDate,
                    d.StoreNumber,
                    DemographicId = d.Id,
                    d.EmploymentStatusId,
                    d.EmploymentStatus,
                    IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                    Department = d.Department != null ? d.Department.Name : "N/A",
                    TerminationReason = d.TerminationCode != null ? d.TerminationCode.Name : "N/A",
                    Gender = d.Gender != null ? d.Gender.Name : "N/A",
                    PayClassification = d.PayClassification != null ? d.PayClassification.Name : "N/A",
                    // Optimize PayProfit queries - only fetch what we need for current/previous years
                    CurrentPayProfit = d.PayProfits
                        .Where(x => x.ProfitYear == currentYear)
                        .Select(x => new
                        {
                            x.ProfitYear,
                            x.CurrentHoursYear,
                            x.VestingScheduleId,
                            x.HasForfeited,
                            x.Etva
                        }).FirstOrDefault(),
                    PreviousPayProfit = d.PayProfits
                        .Where(x => x.ProfitYear == previousYear)
                        .Select(x => new
                        {
                            x.ProfitYear,
                            x.CurrentHoursYear,
                            x.Etva,
                            x.PsCertificateIssuedDate
                        }).FirstOrDefault()
                })
                .ToPaginationResultsAsync(req, cancellationToken);

            // Fetch missives for all SSNs in one query (exclude duplicates)
            var nonDuplicateSsns = members.Results.Select(m => m.Ssn).Except(duplicateSsns).ToList();
            var missivesDict = await _missiveService.DetermineMissivesForSsns(nonDuplicateSsns, currentYear, cancellationToken);

            // Fetch all duplicate badge numbers in one query instead of N queries
            // EF Core 9: Optimized batch query
            var duplicateBadgeMap = new Dictionary<int, List<int>>();
            if (duplicateSsns.Any())
            {
                var duplicateData = await demographics
                    .Where(d => duplicateSsns.Contains(d.Ssn))
                    .Select(d => new { d.Ssn, d.BadgeNumber, d.Id })
                    .TagWith("EmployeeMasterInquiry: Fetch duplicate badges")
                    .ToListAsync(cancellationToken);

                foreach (var dup in duplicateSsns)
                {
                    duplicateBadgeMap[dup] = duplicateData
                        .Where(d => d.Ssn == dup)
                        .Select(d => d.BadgeNumber)
                        .Distinct()
                        .ToList();
                }
            }

            var detailsList = new List<MemberDetails>();
            foreach (var memberData in members.Results)
            {
                var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();

                // Get duplicate badges from pre-fetched map
                var duplicateBadges = duplicateSsns.Contains(memberData.Ssn) && duplicateBadgeMap.TryGetValue(memberData.Ssn, out var badges)
                    ? badges.Where(b => b != memberData.BadgeNumber).ToList()
                    : new List<int>();

                var enrollmentId = ResolveEnrollmentId(memberData.CurrentPayProfit?.VestingScheduleId,
                    memberData.CurrentPayProfit?.HasForfeited ?? false);
                var enrollmentName = EnrollmentConstants.GetDescription(enrollmentId);

                detailsList.Add(new MemberDetails
                {
                    IsEmployee = true,
                    Id = memberData.Id,
                    FirstName = memberData.FirstName,
                    MiddleName = memberData.MiddleName,
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
                    FullTimeDate = memberData.FullTimeDate,
                    TerminationDate = memberData.TerminationDate,
                    StoreNumber = memberData.StoreNumber,
                    EnrollmentId = enrollmentId,
                    Enrollment = enrollmentName,
                    BadgeNumber = memberData.BadgeNumber,
                    PayFrequencyId = memberData.PayFrequencyId,
                    CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
                    PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
                    EmploymentStatus = memberData.EmploymentStatus?.Name,
                    Department = memberData.Department,
                    TerminationReason = memberData.TerminationReason,
                    Gender = memberData.Gender,
                    PayClassification = memberData.PayClassification,
                    PhoneNumber = memberData.PhoneNumber,
                    WorkLocation = memberData.StoreNumber > 0 ? $"Store {memberData.StoreNumber}" : null,
                    ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                    Missives = missiveList,
                    BadgesOfDuplicateSsns = duplicateBadges
                });
            }

            _logger.LogDebug("Retrieved {ResultCount} employee details from {TotalCount} total matching records",
                detailsList.Count, members.Total);

            return new PaginatedResponseDto<MemberDetails>(req) { Results = detailsList, Total = members.Total };
        }, cancellationToken);
    }

    public Task<List<MemberDetails>> GetAllEmployeeDetailsForSsnsAsync(
        ISet<int> ssns,
        short currentYear,
        short previousYear,
        ISet<int> duplicateSsns,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all employee details for {SsnCount} SSNs (non-paginated). CurrentYear: {CurrentYear}",
            ssns.Count, currentYear);

        return _factory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

            // ReadOnlyDbContext automatically handles AsSplitQuery and AsNoTracking
            var query = demographics
                .Where(d => ssns.Contains(d.Ssn))
                .TagWith("EmployeeMasterInquiry: Get all demographic details for SSNs");

            // Optimize projection: only select needed fields
            var members = await query
                .Select(d => new
                {
                    d.Id,
                    d.ContactInfo.FullName,
                    d.ContactInfo.FirstName,
                    d.ContactInfo.MiddleName,
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
                    d.FullTimeDate,
                    d.TerminationDate,
                    d.StoreNumber,
                    DemographicId = d.Id,
                    d.EmploymentStatus,
                    IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                    Department = d.Department != null ? d.Department.Name : "N/A",
                    TerminationReason = d.TerminationCode != null ? d.TerminationCode.Name : "N/A",
                    Gender = d.Gender != null ? d.Gender.Name : "N/A",
                    PayClassification = d.PayClassification != null ? d.PayClassification.Name : "N/A",
                    // Optimize PayProfit queries
                    CurrentPayProfit = d.PayProfits
                        .Where(x => x.ProfitYear == currentYear)
                        .Select(x => new
                        {
                            x.CurrentHoursYear,
                            x.VestingScheduleId,
                            x.HasForfeited,
                            x.Etva
                        }).FirstOrDefault(),
                    PreviousPayProfit = d.PayProfits
                        .Where(x => x.ProfitYear == previousYear)
                        .Select(x => new
                        {
                            x.Etva,
                            x.PsCertificateIssuedDate
                        }).FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            // Fetch missives for all non-duplicate SSNs in one query
            var nonDuplicateSsns = members.Select(m => m.Ssn).Except(duplicateSsns).ToList();
            var missivesDict = await _missiveService.DetermineMissivesForSsns(nonDuplicateSsns, currentYear, cancellationToken);

            // Fetch all duplicate badge numbers in one optimized query
            var duplicateBadgeMap = new Dictionary<int, List<int>>();
            if (duplicateSsns.Any())
            {
                var duplicateData = await demographics
                    .Where(d => duplicateSsns.Contains(d.Ssn))
                    .Select(d => new { d.Ssn, d.BadgeNumber, d.Id })
                    .TagWith("EmployeeMasterInquiry: Fetch all duplicate badges")
                    .ToListAsync(cancellationToken);

                foreach (var dup in duplicateSsns)
                {
                    duplicateBadgeMap[dup] = duplicateData
                        .Where(d => d.Ssn == dup)
                        .Select(d => d.BadgeNumber)
                        .Distinct()
                        .ToList();
                }
            }

            var detailsList = new List<MemberDetails>();
            foreach (var memberData in members)
            {
                var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();

                // Get duplicate badges from pre-fetched map
                var duplicateBadges = duplicateSsns.Contains(memberData.Ssn) && duplicateBadgeMap.TryGetValue(memberData.Ssn, out var badges)
                    ? badges.Where(b => b != memberData.BadgeNumber).ToList()
                    : new List<int>();

                var enrollmentId = ResolveEnrollmentId(memberData.CurrentPayProfit?.VestingScheduleId,
                    memberData.CurrentPayProfit?.HasForfeited ?? false);
                var enrollmentName = EnrollmentConstants.GetDescription(enrollmentId);

                detailsList.Add(new MemberDetails
                {
                    IsEmployee = true,
                    Id = memberData.Id,
                    FirstName = memberData.FirstName,
                    MiddleName = memberData.MiddleName,
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
                    FullTimeDate = memberData.FullTimeDate,
                    TerminationDate = memberData.TerminationDate,
                    StoreNumber = memberData.StoreNumber,
                    EnrollmentId = enrollmentId,
                    Enrollment = enrollmentName,
                    BadgeNumber = memberData.BadgeNumber,
                    PayFrequencyId = memberData.PayFrequencyId,
                    CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
                    PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
                    EmploymentStatus = memberData.EmploymentStatus?.Name,
                    Department = memberData.Department,
                    TerminationReason = memberData.TerminationReason,
                    Gender = memberData.Gender,
                    PayClassification = memberData.PayClassification,
                    PhoneNumber = memberData.PhoneNumber,
                    WorkLocation = memberData.StoreNumber > 0 ? $"Store {memberData.StoreNumber}" : null,
                    ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                    Missives = missiveList,
                    BadgesOfDuplicateSsns = duplicateBadges
                });
            }

            return detailsList;
        }, cancellationToken);
    }

    public Task<int> FindEmployeeSsnByBadgeAsync(
        int badgeNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding employee SSN by badge number: {BadgeNumber}", badgeNumber);

        return _factory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            // ReadOnlyDbContext automatically handles AsNoTracking
            int ssnEmpl = await demographics
                .Where(d => d.BadgeNumber == badgeNumber)
                .Select(d => d.Ssn)
                .FirstOrDefaultAsync(cancellationToken);

            if (ssnEmpl == 0)
            {
                _logger.LogInformation("No employee found for badge number: {BadgeNumber}", badgeNumber);
            }
            else
            {
                _logger.LogDebug("Found SSN for badge number: {BadgeNumber}, SSN: {MaskedSsn}", badgeNumber, ssnEmpl.MaskSsn());
            }

            return ssnEmpl;
        }, cancellationToken);
    }

    private static MemberDetails CreateEmptyMemberDetails()
    {
        return new MemberDetails
        {
            Id = 0,
            IsEmployee = true,
            BadgeNumber = 0,
            PsnSuffix = 0,
            PayFrequencyId = 0,
            IsExecutive = false,
            Ssn = 0.MaskSsn(),
            FirstName = string.Empty,
            MiddleName = null,
            LastName = string.Empty,
            Address = string.Empty,
            AddressCity = string.Empty,
            AddressState = string.Empty,
            AddressZipCode = string.Empty,
            Age = 0,
            DateOfBirth = default,
            YearToDateProfitSharingHours = 0,
            EnrollmentId = EnrollmentConstants.NotEnrolled,
            Enrollment = EnrollmentConstants.GetDescription(EnrollmentConstants.NotEnrolled),
            StoreNumber = 0,
            CurrentEtva = 0,
            PreviousEtva = 0,
            Missives = [],
            BadgesOfDuplicateSsns = []
        };
    }
}
