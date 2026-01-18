using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public class AdhocBeneficiariesReport : IAdhocBeneficiariesReport
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public AdhocBeneficiariesReport(IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public Task<AdhocBeneficiariesReportResponse> GetAdhocBeneficiariesReportAsync(
        AdhocBeneficiariesReportRequest req,
        CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(ctx);
            var employeeSsns = demographicQuery.Select(d => d.Ssn);
            var baseQuery = ctx.Beneficiaries
                .Where(b => !b.IsDeleted)
                .Select(b => new
                {
                    b.Id,
                    b.PsnSuffix,
                    b.Relationship,
                    b.BadgeNumber,
                    ContactSsn = b.Contact!.Ssn,
                    ContactFullName = b.Contact.ContactInfo!.FullName,
                    DemographicPayFrequencyId = b.Demographic != null ? b.Demographic.PayFrequencyId : (int?)null
                });

            baseQuery = req.IsAlsoEmployee
                ? baseQuery.Where(b => employeeSsns.Contains(b.ContactSsn))
                : baseQuery.Where(b => !employeeSsns.Contains(b.ContactSsn));

            // Apply pagination
            var pagedBeneficiaries = await baseQuery
                .ToPaginationResultsAsync(req, cancellationToken);

            var beneficiarySsns = pagedBeneficiaries.Results.Select(b => b.ContactSsn).ToHashSet();

            var totalBalances = await _totalService.GetTotalBalanceSet(ctx, req.ProfitYear)
                .Where(x => beneficiarySsns.Contains(x.Ssn))
                .ToListAsync(cancellationToken);
            var totalBalanceBySsn = totalBalances.ToLookup(x => x.Ssn);

            // Fetch all relevant profit details for paged beneficiaries
            var allProfitDetails = await ctx.ProfitDetails
                .Where(pd => beneficiarySsns.Contains(pd.Ssn) && pd.ProfitYear == req.ProfitYear)
                .Include(profitDetail => profitDetail.ProfitCode)
                .ToListAsync(cancellationToken);

            var filteredList = pagedBeneficiaries.Results.Select(b =>
            {
                var profitDetailsForBeneficiary = allProfitDetails
                    .Where(pd => pd.Ssn == b.ContactSsn)
                    .Select(pd => new ProfitDetailDto(
                        pd.ProfitYear,
                        pd.ProfitCode.Name,
                        pd.Contribution,
                        pd.Earnings,
                        pd.Forfeiture,
                        DateOnly.FromDateTime(pd.CreatedAtUtc.DateTime),
                        pd.Remark)).ToList();

                var totalBalance = totalBalanceBySsn[b.ContactSsn].FirstOrDefault();

                return new BeneficiaryReportDto(
                    b.PsnSuffix,
                    b.ContactFullName ?? string.Empty,
                    b.ContactSsn.MaskSsn(),
                    b.Relationship,
                    totalBalance?.TotalAmount ?? 0,
                    b.BadgeNumber,
                    b.PsnSuffix,
                    profitDetailsForBeneficiary
                )
                { IsExecutive = b.DemographicPayFrequencyId == PayFrequency.Constants.Monthly };
            }).ToList();

            return new AdhocBeneficiariesReportResponse
            {
                ReportName = "adhoc-beneficiaries-report",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue,
                Response = new PaginatedResponseDto<BeneficiaryReportDto>(req)
                {
                    Results = filteredList!,
                    Total = pagedBeneficiaries.Total
                },
                TotalEndingBalance = totalBalances.Sum(x => x.TotalAmount ?? 0)
            };
        }, cancellationToken);
    }
}
