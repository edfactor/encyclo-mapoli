using System.Data;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services.ProfitMaster;

#pragma warning disable AsyncFixer01

/// <summary>
///     Profit Master Update - inserts or deletes PROFIT_DETAIL rows based on values provided by the user.
///     Invokes the Profit Share Edit Service to compute and return the transactions (PROFIT_DETAIL rows) based on user
///     input.
///     The resulting records are inserted or deleted in the PROFIT_DETAILS database.
///     The employees ETVA field will be adjusted when inserting and records with an "100% Earnings" (profit_code=8)
///     records.
///     The ETVA field will be decremented when performing deletes.
///     The class name follows the name of the step in the Ready YE flow.
/// </summary>
public class ProfitMasterService : IProfitMasterService
{
    private readonly IAppUser _appUser;
    private readonly IProfitSharingDataContextFactory _dbFactory;
    private readonly IInternalProfitShareEditService _profitShareEditService;
    private readonly IFrozenService _frozenService;

    public ProfitMasterService(IInternalProfitShareEditService profitShareEditService, IProfitSharingDataContextFactory dbFactory, IAppUser appUser, IFrozenService frozenService)
    {
        _profitShareEditService = profitShareEditService;
        _dbFactory = dbFactory;
        _appUser = appUser;
        _frozenService = frozenService;
    }

    public async Task<ProfitMasterUpdateResponse?> Status(ProfitYearRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        return await _dbFactory.UseReadOnlyContext(async ctx =>
        {
            YearEndUpdateStatus? yearEndUpdateStatus = await ctx.YearEndUpdateStatuses.Where(status => status.ProfitYear == profitShareUpdateRequest.ProfitYear)
                .FirstOrDefaultAsync(cancellationToken);
            if (yearEndUpdateStatus == null)
            {
                return null;
            }

            return new ProfitMasterUpdateResponse
            {
                BeneficiariesEffected = yearEndUpdateStatus.BeneficiariesEffected,
                EmployeesEffected = yearEndUpdateStatus.EmployeesEffected,
                EtvasEffected = yearEndUpdateStatus.EtvasEffected,
                UpdatedTime = yearEndUpdateStatus.CreatedAtUtc,
                UpdatedBy = yearEndUpdateStatus.UserName ?? "UnKnown",
                ContributionPercent = yearEndUpdateStatus.ContributionPercent,
                IncomingForfeitPercent = yearEndUpdateStatus.IncomingForfeitPercent,
                EarningsPercent = yearEndUpdateStatus.EarningsPercent,
                SecondaryEarningsPercent = yearEndUpdateStatus.SecondaryEarningsPercent,
                MaxAllowedContributions = yearEndUpdateStatus.MaxAllowedContributions,
                BadgeAdjusted = yearEndUpdateStatus.BadgeAdjusted,
                BadgeAdjusted2 = yearEndUpdateStatus.BadgeAdjusted2,
                AdjustContributionAmount = yearEndUpdateStatus.AdjustContributionAmount,
                AdjustEarningsAmount = yearEndUpdateStatus.AdjustEarningsAmount,
                AdjustIncomingForfeitAmount = yearEndUpdateStatus.AdjustIncomingForfeitAmount,
                AdjustEarningsSecondaryAmount = yearEndUpdateStatus.AdjustEarningsSecondaryAmount
            };
        }, cancellationToken);
    }

    public async Task<ProfitMasterUpdateResponse> Update(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken ct)
    {
        return await _dbFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            // The ETVA value is always hot in the wall clock year.
            short etvaHotProfitYear = (short)DateTime.Now.Year;

            var latestYearEndUpdateStatus = await ctx.YearEndUpdateStatuses
                .OrderByDescending(st => st.ProfitYear)
                .FirstOrDefaultAsync(ct);

            // Get active frozen year.
            var frozenDemographicYear = (await _frozenService.GetActiveFrozenDemographic(ct)).ProfitYear;

            // lastestYearEndUpdateStatus == null only for first year ever, 2025.
            if (latestYearEndUpdateStatus != null && latestYearEndUpdateStatus.ProfitYear != frozenDemographicYear - 1)
            {
                throw new BadHttpRequestException(
                    $"Expected no year_end_update_status row to be present when starting the {frozenDemographicYear} (frozen demographics year) update");
            }

            var openProfitYear = frozenDemographicYear;

            (List<ProfitShareEditMemberRecord> records, _, _, _, _) = await _profitShareEditService.ProfitShareEditRecords(profitShareUpdateRequest, ct);

            Dictionary<byte, ProfitCode> code2ProfitCode = await ctx.ProfitCodes.ToDictionaryAsync(pc => pc.Id, pc => pc, ct);

            // Get the records to be created
            List<ProfitDetail> profitDetailRecords = CreateProfitDetailRecords(code2ProfitCode, openProfitYear, records);

            EnsureNoValuesOutOfBounds(profitDetailRecords);

            // Insert them in bulk.
            // ctx.ProfitDetails.AddRange(profitDetailRecords); <--- 7 minutes for obfuscated database
            BulkInsertProfitDetails(ctx, transaction, profitDetailRecords);

            // This code only runs when the system is "FROZEN" which means in the beginning of a new year, and processing
            // last year's profit sharing.   We currently grab most profit sharing data from PayProfit using the prior year (aka profit year), but
            // the hot ETVA is located in the PayProfit for the wall-clock year (aka profitYear+1) 

            // Vocabulary;  "Now Year" or "wall clock year" or "profit year + 1" they are the same.  
            //              "Profit Year" is simply the profit year. 

            // This selection of which columns are from "profit year" vs "now year" is could change. 

            // This gist of why we care about two PAY_PROFIT rows, is that we need information from both when the wall clock year is not the profit year.
            // EnrolledId               -> (either row?) which type of plan are they in 1,2 or are the out 3,4   (do we want now or at Fiscal Freeze?) 
            // PointsEarned             -> (profit year) set in PAY443/YearEndService.cs
            // ZeroContributionReasonId -> (profit year) set in PAY426/PayProfitUpdateService.cs
            // ETVA                     -> (profit year - write only!) - it becomes "last years ETVA" aka the ETVA when YE (this program) is run.
            // ETVA                     -> (Now Year aka "profit year + 1", tho "hot" ETVA for an employee

            // When the Profit Share Update is run, both the profitYear and profitYear+1 ETVAs need to be updated.
            // profitYear + 1 <--- because it is the hot "ETVA" which gets adjusted on demand.
            // profitYear     <--- because it is about to become the new "Last Years" ETVA amount - the amount used in this profit share run.
            // I believe they should both hold identical values when this is completed.   Note that doing a revert will restore the profitYear+1 ETVA to the value it had before the update.
            // and will zero out the ETVA for the profitYear.

            // Bump the ETVA by the amount of the earnings for the 100% vested earnings records.
            // if you got $100 in interest on your "fully owned" part of profit sharing, then your ETVA goes up by 100.
            FormattableString sqlAdjustEtva = @$"
                MERGE INTO pay_profit pp
                USING (
                  -- find all the employees who got an altered ETVA, via the new 8 records.
                  SELECT d.id AS demographic_id, pd.earnings
                  FROM profit_detail pd
                  JOIN demographic d ON pd.ssn = d.ssn
                  WHERE pd.profit_code_id = /*8*/ {ProfitCode.Constants.Incoming100PercentVestedEarnings.Id} 
                    AND pd.profit_year = {openProfitYear}
                    AND pd.comment_type_id = /* 23 '100% Earnings' */{CommentType.Constants.OneHundredPercentEarnings.Id} 
                ) oq    
                ON (pp.demographic_id = oq.demographic_id AND pp.profit_year = {etvaHotProfitYear})
                WHEN MATCHED THEN
                  -- bump UP the ETVA by the amount of 100% earnings. 
                  UPDATE SET pp.etva = pp.etva + oq.earnings";
            int etvasEffected = await ctx.Database.ExecuteSqlInterpolatedAsync(sqlAdjustEtva, ct);

            // now copy New ETVA to prior ETVA year, so we have a record of its value at YE
            short etvaPriorProfitYear = (short)(etvaHotProfitYear - 1);
            FormattableString sqlAdjustProfitYearEtva = @$"
                MERGE INTO pay_profit pp
                USING (
                  -- find all the employees who got an altered ETVA, via the new 8 records.
                  SELECT d.id AS demographic_id, ppNow.Etva
                  FROM profit_detail pd
                  JOIN demographic d ON pd.ssn = d.ssn
                  JOIN pay_profit ppNow ON d.id = ppNow.demographic_id
                  WHERE pd.profit_code_id = /*8*/ {ProfitCode.Constants.Incoming100PercentVestedEarnings.Id} 
                    AND pd.profit_year = {openProfitYear}
                    AND pd.comment_type_id = /* 23 '100% Earnings' */{CommentType.Constants.OneHundredPercentEarnings.Id} 
                    AND ppNow.profit_year = {etvaHotProfitYear}
                ) oq    
                ON (pp.demographic_id = oq.demographic_id AND pp.profit_year = {etvaPriorProfitYear})
                WHEN MATCHED THEN
                  -- copy now to last year 
                  UPDATE SET pp.etva = oq.etva";
            etvasEffected += await ctx.Database.ExecuteSqlInterpolatedAsync(sqlAdjustProfitYearEtva, ct);

            int employeesEffected = records.Where(m => m.IsEmployee).Select(m => m.Ssn).ToHashSet().Count;
            int beneficiariesEffected = records.Where(m => !m.IsEmployee).Select(m => m.Ssn).ToHashSet().Count;

            ctx.YearEndUpdateStatuses.Add(new YearEndUpdateStatus
            {
                ProfitYear = openProfitYear,
                ModifiedAtUtc = DateTimeOffset.UtcNow,
                UserName = _appUser.UserName ?? "Unknown",
                BeneficiariesEffected = beneficiariesEffected,
                EmployeesEffected = employeesEffected,
                EtvasEffected = etvasEffected,
                ContributionPercent = profitShareUpdateRequest.ContributionPercent,
                IncomingForfeitPercent = profitShareUpdateRequest.IncomingForfeitPercent,
                EarningsPercent = profitShareUpdateRequest.EarningsPercent,
                SecondaryEarningsPercent = profitShareUpdateRequest.SecondaryEarningsPercent,
                MaxAllowedContributions = profitShareUpdateRequest.MaxAllowedContributions,
                BadgeAdjusted = profitShareUpdateRequest.BadgeToAdjust,
                BadgeAdjusted2 = profitShareUpdateRequest.BadgeToAdjust2,
                AdjustContributionAmount = profitShareUpdateRequest.AdjustContributionAmount,
                AdjustEarningsAmount = profitShareUpdateRequest.AdjustEarningsAmount,
                AdjustIncomingForfeitAmount = profitShareUpdateRequest.AdjustIncomingForfeitAmount,
                AdjustEarningsSecondaryAmount = profitShareUpdateRequest.AdjustEarningsSecondaryAmount,
                IsYearEndCompleted = true, // Not used atm
            });

            await ctx.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return new ProfitMasterUpdateResponse
            {
                BeneficiariesEffected = beneficiariesEffected,
                EmployeesEffected = employeesEffected,
                EtvasEffected = etvasEffected,
                TransactionsCreated = profitDetailRecords.Count,
                UpdatedTime = DateTime.Now,
                UpdatedBy = _appUser.UserName ?? "Unknown",
                ContributionPercent = profitShareUpdateRequest.ContributionPercent,
                IncomingForfeitPercent = profitShareUpdateRequest.IncomingForfeitPercent,
                EarningsPercent = profitShareUpdateRequest.EarningsPercent,
                SecondaryEarningsPercent = profitShareUpdateRequest.SecondaryEarningsPercent,
                MaxAllowedContributions = profitShareUpdateRequest.MaxAllowedContributions,
                BadgeAdjusted = profitShareUpdateRequest.BadgeToAdjust,
                BadgeAdjusted2 = profitShareUpdateRequest.BadgeToAdjust2,
                AdjustContributionAmount = profitShareUpdateRequest.AdjustContributionAmount,
                AdjustEarningsAmount = profitShareUpdateRequest.AdjustEarningsAmount,
                AdjustIncomingForfeitAmount = profitShareUpdateRequest.AdjustIncomingForfeitAmount,
                AdjustEarningsSecondaryAmount = profitShareUpdateRequest.AdjustEarningsSecondaryAmount
            };
        }, ct);
    }

    private static List<ProfitDetail> CreateProfitDetailRecords(Dictionary<byte, ProfitCode> id2ProfitCode, short profitYear,
        IEnumerable<ProfitShareEditMemberRecord> rec)
    {
        return rec.Select(r => new ProfitDetail
        {
            ProfitCode = id2ProfitCode[r.ProfitCode],
            Ssn = r.Ssn,
            ProfitYear = profitYear,
            ProfitYearIteration = r.YearExtension,
            ProfitCodeId = r.ProfitCode,
            Contribution = r.ContributionAmount,
            Earnings = r.EarningAmount,
            Forfeiture = r.ForfeitureAmount,
            Remark = r.Remark,
            ZeroContributionReasonId = r.ZeroContStatus,
            CommentTypeId = r.CommentTypeId,
            YearsOfServiceCredit = (r.ProfitCode == 0 && (r.ContributionAmount != 0 || r.ZeroContStatus is 1 or 2)) ? (sbyte)1 : (sbyte)0
        }).ToList();
    }

    public async Task<ProfitMasterRevertResponse> Revert(ProfitYearRequest profitYearRequest, CancellationToken cancellationToken)
    {
        var frozenDemographicYear = (await _frozenService.GetActiveFrozenDemographic(cancellationToken)).ProfitYear;
        // The ETVA value is always hot in the wall clock year.
        short etvaHotProfitYear = (short)DateTime.Now.Year;

        return await _dbFactory.UseWritableContextAsync(async (ctx, transaction) =>
        {
            var latestYearEndUpdateStatus = await ctx.YearEndUpdateStatuses
                .OrderByDescending(st => st.ProfitYear)
                .FirstOrDefaultAsync(cancellationToken);
            if (latestYearEndUpdateStatus == null || frozenDemographicYear != latestYearEndUpdateStatus.ProfitYear)
            {
                throw new BadHttpRequestException("Expected to be reverting a frozen year with an existing yearEndUpdateStatus row.");
            }

            var openProfitYear = latestYearEndUpdateStatus.ProfitYear; // Undoing the just completed YE.

            // Consider adding additional checking to ensure that:
            // - the ETVA selection (how many EVTA rows will be altered)
            // - the employee count
            // - the bene count
            // if any of these counts do not match, the revert should halt - and return the expected and actual numbers.
            // or perhaps the counts should be "number of profit count 0 records" and "number of profit count 8 records"

            // --- Adjust ETVA

            // Rollback the effect of the transactions on the "NOW" ETVA.
            FormattableString sqlAdjustEtva = $@"
                MERGE INTO pay_profit pp
                USING (
                  -- find the employees and the amount of earnings
                  SELECT d.id AS demographic_id, pd.earnings
                  FROM profit_detail pd
                  JOIN demographic d ON pd.ssn = d.ssn
                  JOIN pay_profit pp2 ON d.id = pp2.demographic_id
                  WHERE pd.profit_code_id = {ProfitCode.Constants.Incoming100PercentVestedEarnings.Id}
                    AND pd.profit_year = {openProfitYear}
                    AND pd.comment_type_id = {CommentType.Constants.OneHundredPercentEarnings.Id}
                    AND pp2.profit_year = {etvaHotProfitYear}
                ) oq
                ON (pp.demographic_id = oq.demographic_id AND pp.profit_year = {etvaHotProfitYear})
                WHEN MATCHED THEN
                  -- bump DOWN the ETVA by the amount of 100% earnings.
                  UPDATE SET pp.etva = pp.etva - oq.earnings";
            int etvasEffected = await ctx.Database.ExecuteSqlInterpolatedAsync(sqlAdjustEtva, cancellationToken);

            // Now get rid of the transactions.
            FormattableString deleteTransactionsSql = $@"
                DELETE FROM profit_detail WHERE
                PROFIT_YEAR = {openProfitYear} AND (
                   PROFIT_CODE_id = {ProfitCode.Constants.IncomingContributions.Id} AND (
                    comment_type_id is null OR
                    comment_type_id = {CommentType.Constants.VOnly.Id} OR
                    comment_type_id = {CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested.Id}
                    )
                OR
               (PROFIT_CODE_id = {ProfitCode.Constants.Incoming100PercentVestedEarnings.Id} AND comment_type_id = {CommentType.Constants.OneHundredPercentEarnings.Id}))";
            var transactionsDeleted = await ctx.Database.ExecuteSqlInterpolatedAsync(deleteTransactionsSql, cancellationToken);

            ctx.YearEndUpdateStatuses.Remove(latestYearEndUpdateStatus);

            await ctx.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Would be better if we figured out which of the profit rows are employees vs bene and then
            // validated that the number deleted matches the numbers inserted.

            return new ProfitMasterRevertResponse
            {
                BeneficiariesEffected = latestYearEndUpdateStatus?.BeneficiariesEffected ?? 0,
                EmployeesEffected = latestYearEndUpdateStatus?.EmployeesEffected ?? 0,
                TransactionsRemoved = transactionsDeleted,
                EtvasEffected = etvasEffected,
                UpdatedTime = DateTime.Now,
                UpdatedBy = _appUser.UserName ?? "Unknown",
            };
        }, cancellationToken);
    }

    private static void /*async Task*/ BulkInsertProfitDetails(ProfitSharingDbContext ctx, IDbContextTransaction transaction, IEnumerable<ProfitDetail> details)
    {
        //
        // Consider looking into; Demoulas.Common.Data.Contexts.Extensions.DbContextExtensions
        //
        // WARNING: OracleBulkCopy does NOT have a public constructor that accepts a transaction.
        // According to Oracle documentation, when a transaction exists on the connection,
        // OracleBulkCopy will use it automatically IF the connection is opened within that transaction context.
        //
        // Since EF Core manages the connection and transaction lifecycle, the connection is already
        // open and enrolled in the transaction when we get here. The bulk copy SHOULD participate
        // in the same transaction as long as we use the same connection object.
        //
        // The 'transaction' parameter is kept in the signature to make it explicit that this method
        // expects to be called within a transaction context, even though we cannot directly pass it
        // to OracleBulkCopy due to API limitations.
        //
        // Reference: https://docs.oracle.com/en/database/oracle/oracle-database/21/odpnt/OracleBulkCopyClass.html

        _ = transaction; // Suppress unused parameter warning - see comment above

        OracleConnection connection = (OracleConnection)ctx.Database.GetDbConnection();

        using var bulkCopy = new OracleBulkCopy(connection)
        {
            DestinationTableName = "PROFIT_DETAIL"
        };

        // Define column mappings (exclude ID since it's auto-generated)
        var columns = new[]
        {
            "SSN", "PROFIT_YEAR", "PROFIT_YEAR_ITERATION", "DISTRIBUTION_SEQUENCE", "PROFIT_CODE_ID", "CONTRIBUTION", "EARNINGS", "FORFEITURE", "MONTH_TO_DATE", "YEAR_TO_DATE",
            "REMARK", "ZERO_CONTRIBUTION_REASON_ID", "FEDERAL_TAXES", "STATE_TAXES", "TAX_CODE_ID", "COMMENT_TYPE_ID", "COMMENT_RELATED_CHECK_NUMBER", "COMMENT_RELATED_STATE",
            "COMMENT_RELATED_ORACLE_HCM_ID", "COMMENT_RELATED_PSN_SUFFIX", "COMMENT_IS_PARTIAL_TRANSACTION", "CREATED_AT_UTC", "YEARS_OF_SERVICE_CREDIT"
        };

        // This seems nutty, to simply map the column names 1-1, but take this out and Oracle tries to add the parameters to the
        // SQL in it's own secret order - not the order of the column definitions in the table.
        foreach (var col in columns)
        {
            bulkCopy.ColumnMappings.Add(col, col);
        }

        using var table = new DataTable(bulkCopy.DestinationTableName);
        table.Columns.Add("SSN", typeof(long));
        table.Columns.Add("PROFIT_YEAR", typeof(short));
        table.Columns.Add("PROFIT_YEAR_ITERATION", typeof(byte));
        table.Columns.Add("DISTRIBUTION_SEQUENCE", typeof(int));
        table.Columns.Add("PROFIT_CODE_ID", typeof(byte));
        table.Columns.Add("CONTRIBUTION", typeof(decimal));
        table.Columns.Add("EARNINGS", typeof(decimal));
        table.Columns.Add("FORFEITURE", typeof(decimal));
        table.Columns.Add("MONTH_TO_DATE", typeof(byte));
        table.Columns.Add("YEAR_TO_DATE", typeof(short));
        table.Columns.Add("REMARK", typeof(string));
        table.Columns.Add("ZERO_CONTRIBUTION_REASON_ID", typeof(int));
        table.Columns.Add("FEDERAL_TAXES", typeof(decimal));
        table.Columns.Add("STATE_TAXES", typeof(decimal));
        table.Columns.Add("TAX_CODE_ID", typeof(string));
        table.Columns.Add("COMMENT_TYPE_ID", typeof(byte));
        table.Columns.Add("COMMENT_RELATED_CHECK_NUMBER", typeof(string));
        table.Columns.Add("COMMENT_RELATED_STATE", typeof(string));
        table.Columns.Add("COMMENT_RELATED_ORACLE_HCM_ID", typeof(long));
        table.Columns.Add("COMMENT_RELATED_PSN_SUFFIX", typeof(int));
        table.Columns.Add("COMMENT_IS_PARTIAL_TRANSACTION", typeof(int));
        table.Columns.Add("CREATED_AT_UTC", typeof(DateTimeOffset)); // Explicitly define type here
        table.Columns.Add("YEARS_OF_SERVICE_CREDIT", typeof(int));

        foreach (var pd in details)
        {
            table.Rows.Add(
                pd.Ssn,
                pd.ProfitYear,
                pd.ProfitYearIteration,
                pd.DistributionSequence,
                pd.ProfitCodeId,
                pd.Contribution,
                pd.Earnings,
                pd.Forfeiture,
                pd.MonthToDate,
                pd.YearToDate,
                pd.Remark,
                pd.ZeroContributionReasonId,
                pd.FederalTaxes,
                pd.StateTaxes,
                pd.TaxCodeId,
                pd.CommentTypeId,
                pd.CommentRelatedCheckNumber,
                pd.CommentRelatedState,
                pd.CommentRelatedOracleHcmId,
                pd.CommentRelatedPsnSuffix,
                pd.CommentIsPartialTransaction,
                pd.CreatedAtUtc,
                pd.YearsOfServiceCredit
            );
        }

        bulkCopy.WriteToServer(table);
    }

    private static void EnsureNoValuesOutOfBounds(List<ProfitDetail> profitDetailRecords)
    {
        const decimal maxAmount = 9999999.99m;

        var outOfBounds = profitDetailRecords.Where(pd =>
            // These fields must be non-negative
            pd.Contribution is < 0 or > maxAmount ||
            pd.Forfeiture is < 0 or > maxAmount ||
            pd.FederalTaxes is < 0 or > maxAmount ||
            pd.StateTaxes is < 0 or > maxAmount ||
            // Earnings can be negative but still has bounds
            pd.Earnings is < -maxAmount or > maxAmount
        ).ToList();

        if (outOfBounds.Count != 0)
        {
            throw new InvalidOperationException($"Found {outOfBounds.Count} records with values out of bounds with precision or negative");
        }
    }
}
