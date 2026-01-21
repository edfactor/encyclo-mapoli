using System.Reflection;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

/// <summary>
/// <para>
/// Provides the list of records which could be inserted into PROFIT_DETAIL.   These are provided for a user to inspect.
/// To actually insert the records, the ProfitMasterUpdate is used.
/// </para>
/// <para>This service is modeled after PAY447</para>
/// <para>This service invokes the "ProfitShareUpdateService" to compute and return the transactions (PROFIT_DETAIL rows) based on user input.</para>
/// <para>This class is named after the step in the Year End flow.    It could instead be named "View Transactions for Year End Update"</para>
/// </summary>
public class ProfitShareEditService : IInternalProfitShareEditService
{
    private readonly IInternalProfitShareUpdateService _profitShareUpdateService;
    private readonly ICalendarService _calendarService;

    public ProfitShareEditService(IInternalProfitShareUpdateService profitShareUpdateService,
        ICalendarService calendarService)
    {
        _profitShareUpdateService = profitShareUpdateService;
        _calendarService = calendarService;
    }

    public async Task<ProfitShareEditResponse> ProfitShareEditAsync(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        var (records, beginningBalanceTotal, contributionGrandTotal, incomingForfeitureGrandTotal, earningsGrandTotal) =
            await ProfitShareEditRecords(profitShareUpdateRequest, cancellationToken);
        var responseRecords = records.Select(m => new ProfitShareEditMemberRecordResponse
        {
            IsEmployee = m.IsEmployee,
            BadgeNumber = m.BadgeNumber,
            Psn = m.Psn,
            FullName = m.Name,
            Code = m.ProfitCode,
            ContributionAmount = m.ContributionAmount,
            EarningsAmount = m.EarningAmount,
            ForfeitureAmount = m.ForfeitureAmount,
            Remark = m.Remark,
            CommentTypeId = m.CommentTypeId,
            RecordChangeSummary = m.RecordChangeSummary,
            DisplayedZeroContStatus = m.DisplayedZeroContStatus,
            YearExtension = m.YearExtension,
            IsExecutive = m.IsExecutive,
        }).ToList();



        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitShareUpdateRequest.ProfitYear, cancellationToken);

        return new ProfitShareEditResponse
        {
            ReportName = "Profit Sharing Edit",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            BeginningBalanceTotal = beginningBalanceTotal,
            ContributionGrandTotal = contributionGrandTotal,
            IncomingForfeitureGrandTotal = incomingForfeitureGrandTotal,
            EarningsGrandTotal = earningsGrandTotal,
            Response = new PaginatedResponseDto<ProfitShareEditMemberRecordResponse>(profitShareUpdateRequest)
            {
                Total = records.Count,
                Results = HandleInMemorySortAndPaging(profitShareUpdateRequest, responseRecords)
            }
        };
    }

    public static List<T> HandleInMemorySortAndPaging<T>(SortedPaginationRequestDto sortedPaginationRequest, List<T> rows)
    {
        string sortBy = sortedPaginationRequest.SortBy ?? "FullName";
        bool isDescending = sortedPaginationRequest.IsSortDescending ?? false;

        var property = typeof(T).GetProperty(sortBy, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property is null && string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
        {
            property = typeof(T).GetProperty("FullName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        if (property is null)
        {
            throw new ArgumentException($"Property '{sortBy}' not found on type {typeof(T).Name}");
        }

        rows = isDescending
            ? rows.OrderByDescending(m => property.GetValue(m, null)).ToList()
            : rows.OrderBy(m => property.GetValue(m, null)).ToList();

        return rows.Skip(sortedPaginationRequest.Skip ?? 0).Take(sortedPaginationRequest.Take ?? 25).ToList();
    }


    public async
        Task<(List<ProfitShareEditMemberRecord>, decimal BeginningBalanceTotal, decimal ContributionGrandTotal, decimal IncomingForfeitureGrandTotal, decimal EarningsGrandTotal)>
        ProfitShareEditRecords(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        ProfitShareUpdateResult psur = await _profitShareUpdateService.ProfitShareUpdateInternalAsync(profitShareUpdateRequest, cancellationToken);

        var beginningBalanceTotal = 0m;
        var contributionGrandTotal = 0m;
        var incomingForfeitureGrandTotal = 0m;
        var earningsGrandTotal = 0m;

        List<ProfitShareEditMemberRecord> records = new();
        foreach (var member in psur.Members)
        {
            if (member.IsEmployee)
            {
                AddEmployeeRecords(records, member);
            }
            else
            {
                AddBeneficiaryRecords(records, member);
            }

            beginningBalanceTotal += member.BeginningAmount;
            contributionGrandTotal += member.Contributions;
            incomingForfeitureGrandTotal += member.IncomingForfeitures;
            earningsGrandTotal += member.AllEarnings;
        }

        return (records, beginningBalanceTotal, contributionGrandTotal, incomingForfeitureGrandTotal, earningsGrandTotal);
    }

    private static void AddEmployeeRecords(List<ProfitShareEditMemberRecord> records, ProfitShareUpdateMember member)
    {
        // Under 21
        if (member.ZeroContributionReasonId == /*1*/ ZeroContributionReason.Constants.Under21WithOver1Khours)
        {
            ProfitShareEditMemberRecord rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
            {
                ZeroContStatus = ZeroContributionReason.Constants.Under21WithOver1Khours,
                Remark = /*V-ONLY*/ CommentType.Constants.VOnly.Name,
                CommentTypeId = CommentType.Constants.VOnly.Id,
                RecordChangeSummary = "18,19,20 > 1000"
            };
            if (member.AllEarnings <= 0)
            {
                AddRecord(records, rec);
                return;
            }
            // is both a Bene and "18,19,20 > 1000", so we treat them as both. - they earn interest - and get a year of service
            rec.EarningAmount = member.AllEarnings;
            AddRecord(records, rec);
            return;
        }

        if (member.SecondaryEtvaEarnings /*PY_PROF_ETVA2*/ > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                YearExtension = 2, // force new line formatting
                EarningAmount = member.SecondaryEtvaEarnings,
                Remark = /*"100% Earnings"*/ CommentType.Constants.OneHundredPercentEarnings.Name,
                CommentTypeId = CommentType.Constants.OneHundredPercentEarnings.Id
            };
            AddRecord(records, rec);
        }

        if (member.AllSecondaryEarnings /*PY-PROF-EARN2*/ > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
            {
                YearExtension = 2, // force new line formatting
                EarningAmount = member.AllSecondaryEarnings - member.SecondaryEtvaEarnings
            };
            // rec.ZeroContStatus = Note, not set in PAY477.cbl
            AddRecord(records, rec);
        }

        // -- Normal Record

        ProfitShareEditMemberRecord mrec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
        {
            ContributionAmount = member.Contributions,
            ForfeitureAmount = member.IncomingForfeitures, // The Earnings includes Etva Earnings
            EarningAmount = member.AllEarnings - member.EtvaEarnings
        };

        // --- Zerocont 2 = Vesting Only - Terminated with >= 1000 hours
        if (member.ZeroContributionReasonId == /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested)
        {
            mrec.Remark = /*"V-Only"*/ CommentType.Constants.VOnly.Name;
            mrec.CommentTypeId = CommentType.Constants.VOnly.Id;
            mrec.RecordChangeSummary = "TERM > 1000 HRS";
            mrec.ZeroContStatus = /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested;
            AddRecord(records, mrec);
        }
        else if (member.ZeroContributionReasonId == /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            // * --- Main-2069 Do Not Process 65 & >5 Zero records
            if (member.Contributions == 0 && mrec.EarningAmount /* We want non ETVA Earnings here */ == 0 && member.IncomingForfeitures == 0)
            {
                return;
            }

            mrec.ZeroContStatus = /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested;
            mrec.Remark = /*">64 & >5 100%"*/ CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested.Name;
            mrec.CommentTypeId = CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested.Id;
            AddRecord(records, mrec);
        }
        else if (member.ZeroContributionReasonId == /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay)
        {
            mrec.DisplayedZeroContStatus = 0; // See <code>IF P-ZEROCONT = "7"</code> in PAY477.cbl
            AddRecord(records, mrec);
        }
        else if (member.Contributions != 0 || member.AllEarnings != 0 || member.IncomingForfeitures != 0)
        {
            AddRecord(records, mrec);
        }
    }

    private static void AddBeneficiaryRecords(List<ProfitShareEditMemberRecord> records, ProfitShareUpdateMember member)
    {
        if (member.AllEarnings > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
                EarningAmount = member.AllEarnings,
                Remark = /*"100% Earnings"*/ CommentType.Constants.OneHundredPercentEarnings.Name,
                CommentTypeId = CommentType.Constants.OneHundredPercentEarnings.Id
            };
            AddRecord(records, rec);
        }

        if (member.AllSecondaryEarnings > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                YearExtension = 2,
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
                EarningAmount = member.AllSecondaryEarnings,
                Remark = /*"100% Earnings"*/ CommentType.Constants.OneHundredPercentEarnings.Name,
                CommentTypeId = CommentType.Constants.OneHundredPercentEarnings.Id
            };
            AddRecord(records, rec);
        }
    }


    private static void AddRecord(List<ProfitShareEditMemberRecord> records, ProfitShareEditMemberRecord rec)
    {
        if (rec is { ContributionAmount: 0, EarningAmount: 0, ForfeitureAmount: 0, Remark: null })
        {
            return;
        }

        rec.RecordChangeSummary ??= rec.Remark;
        records.Add(rec);
    }
}
