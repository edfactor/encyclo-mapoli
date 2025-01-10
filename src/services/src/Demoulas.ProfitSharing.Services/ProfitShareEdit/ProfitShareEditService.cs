using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

/// <summary>
///     Invokes the Profit Share Update Service to compute and return the transactions (PROFIT_DETAIL rows) based on user input.
///     Modeled after PAY447
///
///     This class follows the name of the step in the Ready YE flow.    It could instead be named "View Transactions for YE Update"
/// </summary>
public class ProfitShareEditService : IProfitShareEditService
{
    private readonly IProfitShareUpdateService _profitShareUpdateService;

    public ProfitShareEditService(IProfitShareUpdateService profitShareUpdateService)
    {
        _profitShareUpdateService = profitShareUpdateService;
    }

    public async Task<ProfitShareEditResponse> ProfitShareEdit(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        ProfitShareUpdateResponse psur = await _profitShareUpdateService.ProfitShareUpdate(profitShareUpdateRequest, cancellationToken);

        List<ProfitShareEditMemberRecordResponse> records = new();
        foreach (var member in psur.Response.Results)
        {
            if (member.IsEmployee)
            {
                AddEmployeeRecords(records, member);
            }
            else
            {
                AddBeneficaryRecords(records, member);
            }
        }

        return new ProfitShareEditResponse
        {
            ReportName = "Profit Sharing Edit",
            ReportDate = DateTimeOffset.Now,
            BeginningBalance = 1,
            ContributionGrandTotal = 2,
            IncomingForfeitureGrandTotal = 3,
            EarningsGrandTotal = 4,
            Response = new PaginatedResponseDto<ProfitShareEditMemberRecordResponse> { Results = records }
        };
    }
    
    private static void AddEmployeeRecords(List<ProfitShareEditMemberRecordResponse> records, ProfitShareUpdateMemberResponse member)
    {
        // Under 21
        if (member.ZeroContributionReasonId == ZeroContributionReason.Constants.Under21WithOver1Khours /*1*/)
        {
            ProfitShareEditMemberRecordResponse rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
            {
                ZeroContStatus = ZeroContributionReason.Constants.Under21WithOver1Khours, // force new line formatting
                Reason = CommentType.Constants.VOnly.Name,
                ReasonSummary = "18,19,20 > 1000",
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
            ProfitShareEditMemberRecordResponse rec = new(member, /*8*/ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                EarningsAmount = member.EtvaEarnings, // force new line formatting
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
                Reason = CommentType.Constants.OneHundredPercentEarnings.Name
            };
            AddRecord(records, rec);
        }

        if (member.SecondaryEtvaEarnings /*PY_PROF_ETVA2*/ > 0)
        {
            ProfitShareEditMemberRecordResponse rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                YearExtension = 2, // force new line formatting
                EarningsAmount = member.SecondaryEtvaEarnings,
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
                Reason = CommentType.Constants.OneHundredPercentEarnings.Name
            };
            AddRecord(records, rec);
        }

        if (member.AllSecondaryEarnings /*PY-PROF-EARN2*/ > 0)
        {
            ProfitShareEditMemberRecordResponse rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
            {
                YearExtension = 2, // force new line formatting
                EarningsAmount = member.AllSecondaryEarnings - member.SecondaryEtvaEarnings
            };
            // rec.ZeroContStatus = Note, not set in PAY477.cbl
            AddRecord(records, rec);
        }

        // -- Normal Record
        HandleNormalRecord(records, member);
    }

    private static void HandleNormalRecord(List<ProfitShareEditMemberRecordResponse> records, ProfitShareUpdateMemberResponse member)
    {
        ProfitShareEditMemberRecordResponse rec = new(member, /*0*/ ProfitCode.Constants.IncomingContributions)
        {
            ContributionAmount = member.Contributions,
            IncomingForfeitures = member.IncomingForfeitures, // The Earnings includes Etva Earnings
            EarningsAmount = member.AllEarnings - member.EtvaEarnings
        };

        // --- Zerocont 2 = Vesting Only - Terminated with >= 1000 hours
        if (member.ZeroContributionReasonId == /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested)
        {
            rec.Reason = CommentType.Constants.VOnly.Name;
            rec.ReasonSummary = "TERM > 1000 HRS";
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
            rec.Reason = /*>64 & >5 Zero Records*/ CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested.Name;
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

    private static void AddBeneficaryRecords(List<ProfitShareEditMemberRecordResponse> records, ProfitShareUpdateMemberResponse member)
    {
        if (member.AllEarnings > 0)
        {
            ProfitShareEditMemberRecordResponse rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                ZeroContStatus = ZeroContributionReason.Constants.Normal, // force new line formatting
                EarningsAmount = member.AllEarnings,
                Reason = CommentType.Constants.OneHundredPercentEarnings.Name
            };
            AddRecord(records, rec);
        }

        if (member.AllSecondaryEarnings > 0)
        {
            ProfitShareEditMemberRecordResponse rec = new(member, /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings)
            {
                YearExtension = 2,
                ZeroContStatus = ZeroContributionReason.Constants.Normal,
                EarningsAmount = member.AllSecondaryEarnings,
                Reason = CommentType.Constants.OneHundredPercentEarnings.Name
            };
            AddRecord(records, rec);
        }
    }


    private static void AddRecord(List<ProfitShareEditMemberRecordResponse> records, ProfitShareEditMemberRecordResponse rec)
    {
        if (rec.ContributionAmount == 0 && rec.EarningsAmount == 0 && rec.IncomingForfeitures == 0 && rec.Reason == null)
        {
            return;
        }

        rec.ReasonSummary ??= rec.Reason;
        records.Add(rec);
    }
}
