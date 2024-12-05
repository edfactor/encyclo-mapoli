using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.Formatters;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.ReportFormatters;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAY444
{
    public long EffectiveYear { get; set; } // PIC 9(4)
    public DateTime TodaysDateTime { get; set; } = DateTime.Now;


    // We are currently hooked up to the PROFITSHARE database for employees because we dont yet have a way to correctly calculate ETVA
    private readonly EmployeeDataHelper _employeeDataHelper;

    private readonly IProfitSharingDataContextFactory dbContextFactory;

    // new structures
    public List<string> ReportLines = new();

    // Need to ensure this is surfaced correctly
    private bool _rerunNeeded;

    public PAY444(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        _employeeDataHelper = new EmployeeDataHelper(connection, dbContextFactory, profitYear);
        EffectiveYear = profitYear;
        this.dbContextFactory = dbContextFactory;
    }

    // It is annoying that forfeit proceeds earnings, but that is the way the Cobol prompts for them.  
    public void ApplyAdjustments(decimal contributionPercent,
        decimal incomingForfeitPercent, decimal earningsPercent, decimal secondaryEarningsPercent,
        long badgeToAdjust, decimal adjustContributionAmount, decimal adjustIncomingForfeitAmount, decimal adjustEarningsAmount,
        long badgeToAdjust2, decimal adjustEarningsSecondary, long maxAllowedContributions)
    {
        // Should AdjustmentAmounts be a request DTO?
        var adjustmentAmounts = new AdjustmentAmounts(
            ContributionPercent: contributionPercent,
            IncomingForfeitPercent: incomingForfeitPercent,
            EarningsPercent: earningsPercent,
            SecondaryEarningsPercent: secondaryEarningsPercent,
            BadgeToAdjust: badgeToAdjust,
            AdjustContributionAmount: adjustContributionAmount,
            AdjustIncomingForfeitAmount: adjustIncomingForfeitAmount,
            AdjustEarningsAmount: adjustEarningsAmount,
            BadgeToAdjust2: badgeToAdjust2,
            AdjustEarningsSecondaryAmount: adjustEarningsSecondary,
            MaxAllowedContributions: maxAllowedContributions
        );

        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentsApplied adjustmentsApplied = new();

        List<MemberFinancials> members = new();
        ProcessEmployees(members, adjustmentAmounts, adjustmentsApplied);
        ProcessBeneficiaries(members, adjustmentAmounts);

        if (_rerunNeeded)
        {
            // This needs to get to the invoker - so they can make adjustments for MAX_CONTRIBUTIONS?
            DISPLAY("Rerun of PAY444 is required.  See output for list of issues.");
        }

        m805PrintSequence(members, maxAllowedContributions);

        m1000AdjustmentReport(adjustmentAmounts, adjustmentsApplied);
    }

    public void ProcessEmployees(List<MemberFinancials> members, AdjustmentAmounts adjustmentAmounts,
        AdjustmentsApplied adjustmentsApplied)
    {
        foreach (EmployeeFinancials empl in _employeeDataHelper.rows)
        {
            var memb = ProcessEmployee(empl, adjustmentAmounts, adjustmentsApplied);
            if (memb != null)
            {
                members.Add(memb);
            }
        }
    }

    private void ProcessBeneficiaries(List<MemberFinancials> members, AdjustmentAmounts adjustmentAmounts)
    {
        var benes = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries.OrderBy(b => b.Contact.ContactInfo.FullName).ThenByDescending(b => b.EmployeeId * 10000 + b.PsnSuffix).Select(b => new BeneficiaryFinancials
            {
                Psn = Convert.ToInt64(b.Psn),
                Ssn = b.Contact.Ssn,
                Name = b.Contact.ContactInfo.FullName,
                CurrentAmount = b.Amount,
                Earnings = b.Earnings,
                SecondaryEarnings = b.SecondaryEarnings
            }).ToListAsync()
        ).GetAwaiter().GetResult();

        foreach (BeneficiaryFinancials bene in benes)
        {
            // is already handled as an employee?
            if (_employeeDataHelper.HasRecordBySsn(bene.Ssn))
            {
                continue;
            }
            var memb = ProcessBeneficiary(bene, adjustmentAmounts);
            if (memb != null)
            {
                members.Add(memb);
            }
        }
    }

    public MemberFinancials? ProcessEmployee(EmployeeFinancials empl, AdjustmentAmounts adjustmentAmounts, AdjustmentsApplied adjustmentsApplied)
    {
        WS_PAYPROFIT ws_payprofit = new WS_PAYPROFIT();
        ws_payprofit.WS_PS_AMT = empl.CurrentAmount;
        ws_payprofit.WS_PROF_POINTS = empl.PointsEarned;

        //* If an employee has an ETVA amount and no years on the plan, employee is a
        //* beneficiary and should get earnings on the etva amt(8 record)
        if (empl.EmployeeTypeId == 0) // 0 = not new, 1 == new in plan
        {
            if (empl.EtvaAfterVestingRules > 0 && empl.CurrentAmount == 0)
            {
                empl.EmployeeTypeId = 2; // empl is bene and gets earning on ETVA
            }
        }

        if (empl.EnrolledId <= 0 && empl.EmployeeTypeId <= 0 && ws_payprofit.WS_PS_AMT <= 0 && empl.YearsInPlan <= 0)
        {
            return null;
        }

        var detailTotals = GetDetailTotals(empl.Ssn);


        MemberTotals membTot = new MemberTotals();

        ComputeContribution(ws_payprofit, membTot, empl.EmployeeId, adjustmentAmounts, adjustmentsApplied);
        ComputeForfeitures(ws_payprofit, membTot, empl.EmployeeId, adjustmentAmounts, adjustmentsApplied);


        membTot.EarningsBalance = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
            (ws_payprofit.WS_PS_AMT - detailTotals.ForfeitsTotal - detailTotals.PaidAllocationsTotal) - detailTotals.DistributionsTotal;

        membTot.EarningsBalance -= detailTotals.ClassActionFundTotal;

        if (membTot.EarningsBalance <= 0)
        {
            membTot.EarnPoints = 0;
            membTot.PointsDollars = 0;
        }
        else
        {
            membTot.PointsDollars =
                detailTotals.AllocationsTotal + (ws_payprofit.WS_PS_AMT - detailTotals.ForfeitsTotal - detailTotals.PaidAllocationsTotal) - detailTotals.DistributionsTotal;

            membTot.EarnPoints = (long)Math.Round(membTot.PointsDollars / 100,
                MidpointRounding.AwayFromZero);
        }

        MemberFinancials memb = new();
        memb.EmployeeId = empl.EmployeeId;
        memb.Psn = empl.EmployeeId;

        memb.Name = empl.Name;
        memb.Ssn = empl.Ssn;

        ComputeEarnings(membTot, null, empl, adjustmentAmounts, adjustmentsApplied, detailTotals.ClassActionFundTotal);

        memb.Xfer = detailTotals.AllocationsTotal;
        memb.Pxfer = detailTotals.PaidAllocationsTotal;
        memb.CurrentAmount = ws_payprofit.WS_PS_AMT;
        memb.Distributions = detailTotals.DistributionsTotal;
        memb.Military = detailTotals.MilitaryTotal;
        memb.Caf = detailTotals.ClassActionFundTotal;
        memb.EmployeeTypeId = empl.EmployeeTypeId;
        memb.ContributionPoints = ws_payprofit.WS_PROF_POINTS;
        memb.EarningPoints = membTot.EarnPoints;
        memb.Contributions = ws_payprofit.WS_PROF_CONT;
        memb.IncomingForfeitures = ws_payprofit.WS_PROF_FORF;
        memb.IncomingForfeitures = memb.IncomingForfeitures - detailTotals.ForfeitsTotal;
        memb.Earnings = empl.Earnings;
        memb.Earnings += empl.EarningsOnEtva;
        memb.SecondaryEarnings = empl.SecondaryEarnings;
        memb.SecondaryEarnings += empl.SecondaryEtvaEarnings;

        decimal memberTotalContribution = ws_payprofit.WS_PROF_CONT + detailTotals.MilitaryTotal + ws_payprofit.WS_PROF_FORF;

        if (memberTotalContribution > adjustmentAmounts.MaxAllowedContributions)
        {
            m260Maxcont(memberTotalContribution, ws_payprofit, memb, empl.EmployeeId, adjustmentAmounts.MaxAllowedContributions);
        }
        else
        {
            memb.MaxOver = 0;
        }

        empl.Contributions = ws_payprofit.WS_PROF_CONT;
        empl.IncomeForfeiture = ws_payprofit.WS_PROF_FORF;

        if (false /*rewrites are off ... the destination columns no longer exist in payprofit*/)
        {
            m420RewritePayprofit(empl);
        }

        return memb;
    }


    public MemberFinancials ProcessBeneficiary(BeneficiaryFinancials bene, AdjustmentAmounts adjustmentAmounts)
    {
        WS_PAYPROFIT ws_payprofit = new WS_PAYPROFIT();

        EmployeeFinancials payprof_rec = new(); // <--- uh what! BOBH UHWHAT

        payprof_rec.Ssn = bene.Ssn;
        payprof_rec.EmployeeId = 0;

        ws_payprofit.WS_PS_AMT = bene.CurrentAmount;

        MemberTotals membTot = new();
        membTot.PointsDollars = 0m;
        membTot.EarningsBalance = 0m;
        membTot.EarnPoints = 0;

        var detailTotals = GetDetailTotals(bene.Ssn);

        membTot.EarningsBalance = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
            (ws_payprofit.WS_PS_AMT - detailTotals.ForfeitsTotal - detailTotals.PaidAllocationsTotal) - detailTotals.DistributionsTotal;

        membTot.EarningsBalance -= detailTotals.ClassActionFundTotal;

        if (membTot.EarningsBalance <= 0)
        {
            membTot.EarnPoints = 0;
            membTot.PointsDollars = 0;
        }
        else
        {
            membTot.PointsDollars =
                Math.Round(detailTotals.AllocationsTotal + (ws_payprofit.WS_PS_AMT - detailTotals.ForfeitsTotal - detailTotals.PaidAllocationsTotal) - detailTotals.DistributionsTotal,
                    2, MidpointRounding.AwayFromZero);

            membTot.EarnPoints = (long)Math.Round(membTot.PointsDollars / 100,
                MidpointRounding.AwayFromZero);
        }

        //* Payben people are py-prof-newemp = 2 and need ETVA and an Allocation this year
        if (detailTotals.AllocationsTotal != 0)
        {
            payprof_rec.EmployeeTypeId = 2;
            if (payprof_rec.EarningsOnEtva == 0)
            {
                payprof_rec.EarningsOnEtva = 0.01m;
            }
        }

        payprof_rec.EmployeeId = 0;
        ComputeEarnings(membTot, bene, null, adjustmentAmounts, null, detailTotals.ClassActionFundTotal);

        MemberFinancials memb = new();
        memb.Name = bene.Name;
        memb.Ssn = bene.Ssn;
        memb.Psn = bene.Psn;
        memb.Distributions = detailTotals.DistributionsTotal;

        if (detailTotals.ClassActionFundTotal > 0)
        {
            memb.Caf = detailTotals.ClassActionFundTotal;
        }
        else
        {
            memb.Caf = 0;
        }

        memb.Xfer = detailTotals.AllocationsTotal;
        memb.Pxfer = detailTotals.PaidAllocationsTotal;

        memb.CurrentAmount = ws_payprofit.WS_PS_AMT;
        memb.EarningPoints = membTot.EarnPoints;
        memb.IncomingForfeitures = memb.IncomingForfeitures - detailTotals.ForfeitsTotal;

        memb.Earnings = bene.Earnings;
        memb.SecondaryEarnings = bene.SecondaryEarnings;

        // BOBH Does this make sense, do bene's hit the max here?
        decimal memberContributionTotal = ws_payprofit.WS_PROF_CONT + detailTotals.MilitaryTotal + ws_payprofit.WS_PROF_FORF;

        if (memberContributionTotal > adjustmentAmounts.MaxAllowedContributions)
        {
            m260Maxcont(memberContributionTotal, ws_payprofit, memb, 0, adjustmentAmounts.MaxAllowedContributions);
        }

        if (false /*rewrites are off ... the destination columns no longer exist in payprofit*/)
        {
            m430RewritePayben(bene);
        }

        return memb;
    }


    public void ComputeContribution(WS_PAYPROFIT ws_payprofit, MemberTotals memberTotals, long badge, AdjustmentAmounts adjustmentAmounts, AdjustmentsApplied adjustmentsApplied)
    {
        memberTotals.ContributionAmount = Math.Round(adjustmentAmounts.ContributionPercent * ws_payprofit.WS_PROF_POINTS, 2,
            MidpointRounding.AwayFromZero);

        if (adjustmentAmounts.BadgeToAdjust > 0 && adjustmentAmounts.BadgeToAdjust == badge)
        {
            adjustmentsApplied.ContributionAmountUnadjusted = memberTotals.ContributionAmount;
            memberTotals.ContributionAmount += adjustmentAmounts.AdjustContributionAmount;
            adjustmentsApplied.ContributionAmountAdjusted = memberTotals.ContributionAmount;
        }

        ws_payprofit.WS_PROF_CONT = memberTotals.ContributionAmount;
    }


    public void ComputeForfeitures(WS_PAYPROFIT ws_payprofit, MemberTotals memberTotals, long badge, AdjustmentAmounts adjustmentAmounts, AdjustmentsApplied adjustmentsApplied)
    {
        memberTotals.IncomingForfeitureAmount = Math.Round(adjustmentAmounts.IncomingForfeitPercent * ws_payprofit.WS_PROF_POINTS, 2,
            MidpointRounding.AwayFromZero);
        if (adjustmentAmounts.BadgeToAdjust > 0 && adjustmentAmounts.BadgeToAdjust == badge)
        {
            adjustmentsApplied.IncomingForfeitureAmountUnadjusted = memberTotals.IncomingForfeitureAmount;
            memberTotals.IncomingForfeitureAmount += adjustmentAmounts.AdjustIncomingForfeitAmount;
            adjustmentsApplied.IncomingForfeitureAmountAdjusted = memberTotals.IncomingForfeitureAmount;
        }

        ws_payprofit.WS_PROF_FORF = memberTotals.IncomingForfeitureAmount;
    }

    public void ComputeEarnings(MemberTotals memberTotals, BeneficiaryFinancials? bene, EmployeeFinancials? empl, AdjustmentAmounts adjustmentAmounts, AdjustmentsApplied? adjustmentsApplied, decimal ClassActionFundTotal)
    {
        if (memberTotals.EarnPoints <= 0 && empl != null)
        {
            memberTotals.EarnPoints = 0;
            memberTotals.EarningsAmount = 0;
            memberTotals.SecondaryEarningsAmount = 0;
            empl.Earnings = 0;
            empl.SecondaryEarnings = 0;
        }


        memberTotals.EarningsAmount = Math.Round(adjustmentAmounts.EarningsPercent * memberTotals.EarnPoints,
            2, MidpointRounding.AwayFromZero);
        if (adjustmentAmounts.BadgeToAdjust > 0 && adjustmentAmounts.BadgeToAdjust == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied.SV_EARN_AMT = memberTotals.EarningsAmount;
            memberTotals.EarningsAmount += adjustmentAmounts.AdjustEarningsAmount;
            adjustmentsApplied.SV_EARN_ADJUSTED = memberTotals.EarningsAmount;
        }

        memberTotals.SecondaryEarningsAmount =
            Math.Round(adjustmentAmounts.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);
        if (adjustmentAmounts.BadgeToAdjust2 > 0 &&
            adjustmentAmounts.BadgeToAdjust2 == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied.SecondaryEarningsAmountUnadjusted = memberTotals.SecondaryEarningsAmount;
            memberTotals.SecondaryEarningsAmount += adjustmentAmounts.AdjustEarningsSecondaryAmount;
            adjustmentsApplied.SecondaryEarningsAmountAdjusted = memberTotals.SecondaryEarningsAmount;
        }

        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA
        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* need to subtract CAF out of PY-PS-ETVA for people not fully vested
        //* because  we can't give earnings for 2021 on class action funds -
        //* they were added in 2021.CAF was added to PY - PS - ETVA for
        //* PY - PS - YEARS < 6.

        decimal EtvaAfterVestingRulesAdjustedByCAF = 0;
        if (empl != null && empl.EtvaAfterVestingRules > 0)
        {
            if (empl.YearsInPlan < 6)
            {
                EtvaAfterVestingRulesAdjustedByCAF = empl.EtvaAfterVestingRules - ClassActionFundTotal;
            }
            else
            {
                empl.EtvaAfterVestingRules = EtvaAfterVestingRulesAdjustedByCAF;
            }
        }

        if (EtvaAfterVestingRulesAdjustedByCAF <= 0 && empl != null)
        {
            empl.Earnings = memberTotals.EarningsAmount;
            empl.SecondaryEarnings = 0m;
            empl.EarningsOnEtva = 0m;
            empl.SecondaryEtvaEarnings = 0m;
            return;
        }


        if (empl != null && memberTotals.PointsDollars > 0)
        {
            // Computes the ETVA amount
            decimal WS_ETVA_PERCENT = EtvaAfterVestingRulesAdjustedByCAF / memberTotals.PointsDollars;
            decimal WS_ETVA_AMT = Math.Round(memberTotals.EarningsAmount * WS_ETVA_PERCENT, 2, MidpointRounding.AwayFromZero);

            // subtracts that amount from the members total earnings
            memberTotals.EarningsAmount = memberTotals.EarningsAmount - WS_ETVA_AMT;

            // Sets Earn and ETVA amounts
            empl!.Earnings = memberTotals.EarningsAmount;
            empl.EarningsOnEtva = WS_ETVA_AMT;
        }

        if (bene != null)
        {
            bene.Earnings = 0m;
            bene.Earnings = memberTotals.EarningsAmount;
        }

        if (adjustmentAmounts.SecondaryEarningsPercent != 0m) // Secondary Earnings
        {
            decimal WS_ETVA_PERCENT = EtvaAfterVestingRulesAdjustedByCAF / memberTotals.PointsDollars;
            decimal WS_ETVA2_AMT = Math.Round(memberTotals.SecondaryEarningsAmount * WS_ETVA_PERCENT, 2,
                MidpointRounding.AwayFromZero);
            memberTotals.SecondaryEarningsAmount -= WS_ETVA2_AMT;
            if (empl != null)
            {
                empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
                empl.SecondaryEtvaEarnings = WS_ETVA2_AMT;
            }

            if (bene != null)
            {
                bene.SecondaryEarnings = WS_ETVA2_AMT;
            }
        }
    }


    public void m260Maxcont(decimal memberTotalContribution, WS_PAYPROFIT ws_payprofit, MemberFinancials memberFinancials, long badge, long maxAllowedContribution)
    {
        decimal overContribution = memberTotalContribution - maxAllowedContribution;

        if (overContribution < ws_payprofit.WS_PROF_FORF)
        {
            ws_payprofit.WS_PROF_FORF -= overContribution;
        }
        else
        {
            DISPLAY($"FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE #{badge}");
            ws_payprofit.WS_PROF_FORF = 0;
        }

        memberFinancials.MaxOver = overContribution;
        memberFinancials.MaxPoints = ws_payprofit.WS_PROF_POINTS;
        _rerunNeeded = true;
    }

    private void DISPLAY(string v)
    {
        Console.WriteLine("DISPLAY: " + v);
    }


    public void m420RewritePayprofit(EmployeeFinancials emp)
    {
        if (emp.EmployeeTypeId == 2)
        {
            emp.EmployeeTypeId = 0;
        }

        // BOBH. should update employee. Alas, SMART does not have the columns.
    }

    public void m430RewritePayben(BeneficiaryFinancials bene)
    {
        if (bene.Earnings == 0 && bene.SecondaryEarnings == 0)
        {
        }

        // BOBH should update bene.   Alas, SMART does not have the columns.
    }


    public DetailTotals GetDetailTotals(int ssn)
    {
        decimal distributionsTotal = 0;
        decimal forfeitsTotal = 0;
        decimal allocationsTotal = 0;
        decimal paidAllocationsTotal = 0;
        decimal militaryTotal = 0;
        decimal classActionFundTotal = 0;

        List<ProfitDetail> pds = dbContextFactory.UseReadOnlyContext(ctx =>
                ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == EffectiveYear)
                    .OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy(pd => pd.MonthToDate)
                    .ThenBy(pd => pd.FederalTaxes)
                    .ToListAsync()
            ).GetAwaiter().GetResult();

        foreach (var pd in pds)
        {
            var (profitCode, remark, forfeiture, contribution, earnings, profitYearIteration) = (pd.ProfitCodeId,
                pd.Remark, pd.Forfeiture, pd.Contribution, pd.Earnings, pd.ProfitYearIteration);

            if (profitCode == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal /*1*/ ||
                profitCode == ProfitCode.Constants.OutgoingDirectPayments /*3*/)
            {
                distributionsTotal += forfeiture;
            }

            if (profitCode == ProfitCode.Constants.Outgoing100PercentVestedPayment /*9*/)
            {
                if (remark![..6] == "XFER >" ||
                    remark[..6] == "QDRO >" ||
                    remark[..5] == "XFER>" ||
                    remark[..5] == "QDRO>")
                {
                    paidAllocationsTotal += forfeiture;
                }
                else
                {
                    distributionsTotal += forfeiture;
                }
            }

            if (profitCode == ProfitCode.Constants.OutgoingForfeitures /*2*/)
            {
                forfeitsTotal += forfeiture;
            }

            if (profitCode == ProfitCode.Constants.OutgoingXferBeneficiary /*5*/)
            {
                paidAllocationsTotal += forfeiture;
            }

            if (profitCode == ProfitCode.Constants.IncomingQdroBeneficiary /*6*/)
            {
                allocationsTotal += contribution;
            }

            if (profitYearIteration == 1 /*Military*/)
            {
                militaryTotal += contribution;
            }

            if (profitYearIteration == 2 /*Class Action Fund*/)
            {
                classActionFundTotal += earnings;
            }
        }

        return new DetailTotals(
             DistributionsTotal: distributionsTotal,
             ForfeitsTotal: forfeitsTotal,
             AllocationsTotal: allocationsTotal,
             PaidAllocationsTotal: paidAllocationsTotal,
             MilitaryTotal: militaryTotal,
             ClassActionFundTotal: classActionFundTotal);

    }

    public void m805PrintSequence(List<MemberFinancials> members, long maxAllowedContribution)
    {
        WRITE("\fDJDE JDE=PAY426,JDL=PAYROL,END,;");
        HEADER_1 header_1 = new();
        header_1.HDR1_YY = TodaysDateTime.Year - 2000;
        header_1.HDR1_MM = TodaysDateTime.Month;
        header_1.HDR1_DD = TodaysDateTime.Day;
        header_1.HDR1_YEAR1 = EffectiveYear;
        header_1.HDR1_HR = TodaysDateTime.Hour;
        header_1.HDR1_MN = TodaysDateTime.Minute;


        members.Sort((a, b) =>
        {
            int nameComparison = StringComparer.Ordinal.Compare(a.Name, b.Name);
            if (nameComparison != 0)
            {
                return nameComparison;
            }

            // This is so we converge on a consistant sort.  This effectively matches Ready's order.
            long aBadge = Convert.ToInt64(a.EmployeeId);
            long bBadge = Convert.ToInt64(b.EmployeeId);
            aBadge = aBadge == 0 ? a.Psn : aBadge;
            bBadge = bBadge == 0 ? b.Psn : bBadge;
            return aBadge < bBadge ? -1 : 1;
        });


        ReportCounters reportCounters = new();
        CollectTotals collectTotals = new();

        foreach (MemberFinancials memberFinancials in members)
        {
            m810WriteReport(reportCounters, header_1, memberFinancials, collectTotals);
        }

        m850PrintTotals(reportCounters, collectTotals, maxAllowedContribution);
    }

    private void WRITE(object obj)
    {
        ReportLines.Add(obj.ToString().TrimEnd());
    }


    public void m810WriteReport(ReportCounters reportCounters, HEADER_1 header_1, MemberFinancials memberFinancials, CollectTotals collectTotals)
    {
        if (reportCounters.LineCounter > 60)
        {
            m830PrintHeader(reportCounters, header_1);
        }

        ReportLine report_line = new();
        ReportLine2 report_line_2 = new();
        if (memberFinancials.EmployeeId > 0)
        {
            report_line.BADGE_NBR = memberFinancials.EmployeeId;
            report_line.EMP_NAME = memberFinancials.Name?.Length > 24
                ? memberFinancials.Name.Substring(0, 24)
                : memberFinancials.Name;

            report_line.BEG_BAL = memberFinancials.CurrentAmount;
            report_line.PR_DIST1 = memberFinancials.Distributions;

            if (memberFinancials.EmployeeTypeId == 1)
            {
                report_line.PR_NEWEMP = "NEW";
            }
            else if (memberFinancials.EmployeeTypeId == 2)
            {
                // This is checking to see if an employee also a bene, and they are a new employee, so not yet vested but they 
                throw new IOException("BOBH: this never happens.");
                // see if member is also a bene.
                // payben_rec.Ssn = memberFinancials.Ssn;
                // PAYBEN_FILE_STATUS = READ_ALT_KEY_PAYBEN(payben_rec);
                // if (PAYBEN_FILE_STATUS == "00")
                // {
                //    report_line.PR_NEWEMP = "BEN";
                //}
            }
            else
            {
                report_line.PR_NEWEMP = " ";
            }

            if (memberFinancials.Xfer != 0)
            {
                memberFinancials.Contributions = memberFinancials.Contributions + memberFinancials.Xfer;
            }

            if (memberFinancials.Pxfer != 0)
            {
                memberFinancials.Military = memberFinancials.Military - memberFinancials.Pxfer;
            }

            report_line.PR_CONT = memberFinancials.Contributions;
            report_line.PR_MIL = memberFinancials.Military;
            report_line.PR_FORF = memberFinancials.IncomingForfeitures;
            report_line.PR_EARN = memberFinancials.Earnings;

            if (memberFinancials.SecondaryEarnings != 0)
            {
                DISPLAY($"badge {memberFinancials.EmployeeId} earnings2 ${memberFinancials.SecondaryEarnings}");
            }

            report_line.PR_EARN2 = memberFinancials.SecondaryEarnings;
            report_line.PR_EARN2 = memberFinancials.Caf;
        }


        decimal endingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                                memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                                memberFinancials.IncomingForfeitures + memberFinancials.Military +
                                memberFinancials.Caf -
                                memberFinancials.Distributions;
        report_line.END_BAL = endingBalance;

        if (memberFinancials.EmployeeId == 0)
        {
            report_line_2.PR2_EMP_NAME =
                memberFinancials.Name?.Length > 24 ? memberFinancials.Name.Substring(0, 24) : memberFinancials.Name;
            report_line_2.PR2_PSN = memberFinancials.Psn;
            report_line_2.PR2_BEG_BAL = memberFinancials.CurrentAmount;
            report_line_2.PR2_DIST1 = memberFinancials.Distributions;
            report_line_2.PR2_NEWEMP = "BEN";
            memberFinancials.Contributions += memberFinancials.Xfer;
            report_line_2.PR2_CONT = memberFinancials.Contributions;
            report_line_2.PR2_MIL = memberFinancials.Military;
            report_line_2.PR2_FORF = memberFinancials.IncomingForfeitures;
            report_line_2.PR2_EARN = memberFinancials.Earnings;
            report_line_2.PR2_EARN2 = memberFinancials.SecondaryEarnings;
            report_line_2.PR2_EARN2 = memberFinancials.Caf;

            endingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                            memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                            memberFinancials.IncomingForfeitures + memberFinancials.Military + memberFinancials.Caf -
                            memberFinancials.Distributions;
            report_line_2.PR2_END_BAL = endingBalance;
        }

        collectTotals.WS_TOT_BEGBAL += memberFinancials.CurrentAmount;
        if (memberFinancials.Xfer != 0)
        {
            memberFinancials.Contributions -= memberFinancials.Xfer;
        }

        if (memberFinancials.Pxfer != 0)
        {
            memberFinancials.Military += memberFinancials.Pxfer;
        }

        collectTotals.WS_TOT_DIST1 += memberFinancials.Distributions;
        collectTotals.WS_TOT_CONT += memberFinancials.Contributions;
        collectTotals.WS_TOT_MIL += memberFinancials.Military;
        collectTotals.WS_TOT_FORF += memberFinancials.IncomingForfeitures;
        collectTotals.WS_TOT_EARN += memberFinancials.Earnings;
        collectTotals.WS_TOT_EARN2 += memberFinancials.SecondaryEarnings;
        collectTotals.WS_TOT_ENDBAL += endingBalance;
        collectTotals.WS_TOT_XFER += memberFinancials.Xfer;
        collectTotals.WS_TOT_PXFER -= memberFinancials.Pxfer;
        collectTotals.WS_EARN_PTS_TOTAL += memberFinancials.EarningPoints;
        collectTotals.WS_PROF_PTS_TOTAL += memberFinancials.ContributionPoints;
        collectTotals.WS_TOT_CAF += memberFinancials.Caf;
        collectTotals.MaxOverTotal += memberFinancials.MaxOver;
        collectTotals.MaxPointsTotal += memberFinancials.MaxPoints;

        if (memberFinancials.CurrentAmount != 0m
            || memberFinancials.Distributions != 0m
            || memberFinancials.Contributions != 0m
            || memberFinancials.Xfer != 0m
            || memberFinancials.Pxfer != 0m
            || memberFinancials.Military != 0m
            || memberFinancials.IncomingForfeitures != 0m
            || memberFinancials.Earnings != 0m
            || memberFinancials.SecondaryEarnings != 0m)
        {
            if (memberFinancials.EmployeeId > 0)
            {
                reportCounters.EmployeeCounter += 1;
                WRITE(report_line);
            }

            if (memberFinancials.EmployeeId == 0)
            {
                reportCounters.BeneficiaryCounter += 1;
                WRITE(report_line_2);
            }

            reportCounters.LineCounter += 1;
        }
    }

    public void m830PrintHeader(ReportCounters reportCounters, HEADER_1 header_1)
    {
        reportCounters.PageCounter += 1;
        header_1.HDR1_PAGE = reportCounters.PageCounter;
        WRITE("\f" + header_1);
        WRITE("");
        WRITE(new HEADER_2());
        WRITE(new HEADER_3());
        reportCounters.LineCounter = 4;
    }


    public void m850PrintTotals(ReportCounters reportCounters, CollectTotals ws_client_totals, long maxAllowedContribution)
    {
        ClientTot client_tot = new();
        client_tot.BEG_BAL_TOT = ws_client_totals.WS_TOT_BEGBAL;
        client_tot.DIST1_TOT = ws_client_totals.WS_TOT_DIST1;
        client_tot.MIL_TOT = ws_client_totals.WS_TOT_MIL;
        client_tot.CONT_TOT = ws_client_totals.WS_TOT_CONT;
        client_tot.FORF_TOT = ws_client_totals.WS_TOT_FORF;
        client_tot.EARN_TOT = ws_client_totals.WS_TOT_EARN;
        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_EARN2;
        if (ws_client_totals.WS_TOT_EARN2 != 0)
        {
            DISPLAY("WS_TOT_EARN2 NOT 0 " + ws_client_totals.WS_TOT_EARN2);
        }

        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_CAF;
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_ENDBAL;


        TOTAL_HEADER_1 total_header_1 = new();
        total_header_1.TOT_HDR1_YEAR1 = EffectiveYear;
        total_header_1.TOT_HDR1_DD = TodaysDateTime.Day;
        total_header_1.TOT_HDR1_MM = TodaysDateTime.Month;
        total_header_1.TOT_HDR1_YY = TodaysDateTime.Year - 2000;
        total_header_1.TOT_HDR1_HR = TodaysDateTime.Hour;
        total_header_1.TOT_HDR1_MN = TodaysDateTime.Minute;

        WRITE("\f" + total_header_1);
        WRITE("");
        WRITE(new TOTAL_HEADER_2());
        WRITE(new TOTAL_HEADER_3());
        WRITE("");
        WRITE(client_tot);

        client_tot.BEG_BAL_TOT = 0m;
        client_tot.DIST1_TOT = 0m;
        client_tot.CONT_TOT = 0m;
        client_tot.MIL_TOT = 0m;
        client_tot.FORF_TOT = 0m;
        client_tot.EARN_TOT = 0m;
        client_tot.END_BAL_TOT = 0m;
        client_tot.EARN2_TOT = 0m;

        client_tot.CONT_TOT = ws_client_totals.WS_TOT_XFER;
        client_tot.MIL_TOT = ws_client_totals.WS_TOT_PXFER;
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_PXFER + ws_client_totals.WS_TOT_XFER;
        client_tot.TOT_FILLER = "ALLOC   ";
        WRITE(client_tot);

        client_tot.BEG_BAL_TOT = 0m;
        client_tot.DIST1_TOT = 0m;
        client_tot.CONT_TOT = 0m;
        client_tot.MIL_TOT = 0m;
        client_tot.FORF_TOT = 0m;
        client_tot.EARN_TOT = 0m;
        client_tot.END_BAL_TOT = 0m;
        client_tot.EARN2_TOT = 0m;

        client_tot.CONT_TOT = ws_client_totals.WS_PROF_PTS_TOTAL;
        client_tot.EARN_TOT = ws_client_totals.WS_EARN_PTS_TOTAL;
        client_tot.useRedefineFormatting = true;
        client_tot.TOT_FILLER = "POINT";
        WRITE("");
        WRITE(client_tot);

        EMPLOYEE_COUNT_TOT employee_count_tot = new();
        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = reportCounters.EmployeeCounter;
        WRITE("");
        WRITE(employee_count_tot);
        EMPLOYEE_COUNT_TOT_PAYBEN employee_count_tot_payben = new();
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = reportCounters.BeneficiaryCounter;
        WRITE("");
        WRITE(employee_count_tot_payben);

        RERUN_TOT rerun_tot = new();
        rerun_tot.RERUN_OVER = ws_client_totals.MaxOverTotal;
        rerun_tot.RERUN_POINTS = ws_client_totals.MaxPointsTotal;
        rerun_tot.RERUN_MAX = maxAllowedContribution;

        ReportLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerun_tot);
    }


    public void m1000AdjustmentReport(AdjustmentAmounts adjustmentAmounts, AdjustmentsApplied adjustmentsApplied)
    {
        if (adjustmentAmounts.BadgeToAdjust == 0)
        {
            return;
        }

        HEADER_1 header_1 = new();
        HEADER_4 header_4 = new();
        HEADER_5 header_5 = new();


        header_1.HDR1_PAGE = 1;
        header_1.HDR1_RPT = "PAY444A";
        WRITE2_afterPage(header_1);
        WRITE2_advance2(header_4);
        WRITE2_advance2(header_5);

        PRINT_ADJ_LINE1 print_adj_line1 = new();
        print_adj_line1.PL_ADJUST_BADGE = adjustmentAmounts.BadgeToAdjust;
        print_adj_line1.PL_ADJ_DESC = "INITIAL";
        print_adj_line1.PL_CONT_AMT = adjustmentsApplied.ContributionAmountUnadjusted;
        print_adj_line1.PL_FORF_AMT = adjustmentsApplied.IncomingForfeitureAmountUnadjusted;
        print_adj_line1.PL_EARN_AMT = adjustmentsApplied.SV_EARN_AMT;
        print_adj_line1.PL_EARN2_AMT = adjustmentsApplied.SecondaryEarningsAmountUnadjusted;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJUST_BADGE = 0;
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        print_adj_line1.PL_CONT_AMT = adjustmentAmounts.AdjustContributionAmount;
        print_adj_line1.PL_EARN_AMT = adjustmentAmounts.AdjustEarningsAmount;
        print_adj_line1.PL_EARN2_AMT = adjustmentAmounts.AdjustEarningsSecondaryAmount;
        print_adj_line1.PL_FORF_AMT = adjustmentAmounts.AdjustIncomingForfeitAmount;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJ_DESC = "FINAL";
        print_adj_line1.PL_CONT_AMT = adjustmentsApplied.ContributionAmountAdjusted;
        print_adj_line1.PL_FORF_AMT = adjustmentsApplied.IncomingForfeitureAmountAdjusted;
        print_adj_line1.PL_EARN_AMT = adjustmentsApplied.SV_EARN_ADJUSTED;
        print_adj_line1.PL_EARN2_AMT = adjustmentsApplied.SecondaryEarningsAmountAdjusted;

        WRITE2_advance2(print_adj_line1);

        if (adjustmentsApplied.IncomingForfeitureAmountUnadjusted == 0 && adjustmentsApplied.SV_EARN_AMT == 0)
        {
            WRITE2_advance2("No adjustment - employee not found.");
        }
    }

    private void WRITE2_advance2(object header4)
    {
        // We dont currently support this second report.    We may have to.
        //throw new NotImplementedException();
    }

    private void WRITE2_afterPage(HEADER_1 header1)
    {
        // We dont currently support this second report.    We may have to.
        // throw new NotImplementedException();
    }
}
