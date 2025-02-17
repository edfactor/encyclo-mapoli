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

    public ProfitShareEditService(IInternalProfitShareUpdateService profitShareUpdateService)
    {
        _profitShareUpdateService = profitShareUpdateService;
    }

    public async Task<ProfitShareEditResponse> ProfitShareEdit(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        var records = await getRecords(profitShareUpdateRequest, cancellationToken);
        var responseRecords = records.Select(m => new ProfitShareEditMemberRecordResponse
        {
            IsEmployee = false,
            BadgeNumber = m.BadgeNumber,
            Psn = m.Psn,
            Name = m.Name,
            Code = m.Code,
            ContributionAmount = m.ContributionAmount,
            EarningsAmount = m.EarningAmount,
            ForfeitureAmount = m.ForfeitureAmount,
            Remark = m.Remark,
            CommentTypeId = m.CommentTypeId,
            RecordChangeSummary = m.RecordChangeSummary,
            ZeroContStatus = m.ZeroContStatus,
            YearExtension = m.YearExtension
        }).ToList();

        return new ProfitShareEditResponse
        {
            ReportName = "Profit Sharing Edit",
            ReportDate = DateTimeOffset.Now,
            BeginningBalance = 1,
            ContributionGrandTotal = 2,
            IncomingForfeitureGrandTotal = 3,
            EarningsGrandTotal = 4,
            Response = new PaginatedResponseDto<ProfitShareEditMemberRecordResponse> { Results = responseRecords }
        };
    }

    public Task<IEnumerable<ProfitShareEditMemberRecord>> ProfitShareEditRecords(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        return getRecords(profitShareUpdateRequest, cancellationToken);
    }

    private async Task<IEnumerable<ProfitShareEditMemberRecord>> getRecords(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        ProfitShareUpdateResult psur = await _profitShareUpdateService.ProfitShareUpdateInternal(profitShareUpdateRequest, cancellationToken);

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
        }
        return records;
    }

    private static void AddEmployeeRecords(List<ProfitShareEditMemberRecord> records, ProfitShareUpdateMember member)
    {
        // Under 21
        if (member.ZeroContributionReasonId == ZeroContributionReason.Constants.Under21WithOver1Khours /*1*/)
        {
            ProfitShareEditMemberRecord rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
            {
                ZeroContStatus = ZeroContributionReason.Constants.Under21WithOver1Khours, // force new line formatting
                Remark = /*V-ONLY*/ CommentType.Constants.VOnly.Name,
                CommentTypeId = CommentType.Constants.VOnly.Id,
                RecordChangeSummary = "18,19,20 > 1000",
                Code = 0
            };
            if (member.AllEarnings <= 0)
            {
                AddRecord(records, rec);
                return;
            }
        }

        //  ETVA Vested Earnings create 8 records 

        if (member.EtvaEarnings > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*8*/ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                EarningAmount = member.EtvaEarnings, // force new line formatting
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
                Remark = /*"100% Earnings"*/ CommentType.Constants.OneHundredPercentEarnings.Name,
                CommentTypeId = CommentType.Constants.OneHundredPercentEarnings.Id
            };
            AddRecord(records, rec);
        }

        if (member.SecondaryEtvaEarnings /*PY_PROF_ETVA2*/ > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                YearExtension = 2, // force new line formatting
                EarningAmount = member.SecondaryEtvaEarnings,
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
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
        HandleNormalRecord(records, member);
    }

    private static void HandleNormalRecord(List<ProfitShareEditMemberRecord> records, ProfitShareUpdateMember member)
    {
        ProfitShareEditMemberRecord rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
        {
            ContributionAmount = member.Contributions,
            ForfeitureAmount = member.IncomingForfeitures, // The Earnings includes Etva Earnings
            EarningAmount = member.AllEarnings - member.EtvaEarnings
        };

        // --- Zerocont 2 = Vesting Only - Terminated with >= 1000 hours
        if (member.ZeroContributionReasonId == /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested)
        {
            rec.Remark = /*"V-Only"*/ CommentType.Constants.VOnly.Name;
            rec.CommentTypeId = CommentType.Constants.VOnly.Id;
            rec.RecordChangeSummary = "TERM > 1000 HRS";
            rec.ZeroContStatus = /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested;
            AddRecord(records, rec);
        }
        else if (member.ZeroContributionReasonId == /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            // * --- Main-2069 Do Not Process 65 & >5 Zero records
            if (member.Contributions == 0 && member.AllEarnings == 0 && member.IncomingForfeitures == 0)
            {
                return;
            }

            rec.ZeroContStatus = /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested;
            rec.Remark = /*">64 & >5 100%"*/ CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested.Name;
            rec.CommentTypeId = CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested.Id;
            AddRecord(records, rec);
        }
        else if (member.ZeroContributionReasonId == /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay)
        {
            // No update of ZeroContStatus
            AddRecord(records, rec);
        }
        else if (member.Contributions != 0 || member.AllEarnings != 0 || member.IncomingForfeitures != 0)
        {
            AddRecord(records, rec);
        }
    }

    private static void AddBeneficiaryRecords(List<ProfitShareEditMemberRecord> records, ProfitShareUpdateMember member)
    {
        if (member.AllEarnings > 0)
        {
            ProfitShareEditMemberRecord rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                ZeroContStatus = ZeroContributionReason.Constants.Normal, // force new line formatting
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
        if (rec.ContributionAmount == 0 && rec.EarningAmount == 0 && rec.ForfeitureAmount == 0 && rec.Remark == null)
        {
            return;
        }

        rec.RecordChangeSummary ??= rec.Remark;
        records.Add(rec);
    }
}
