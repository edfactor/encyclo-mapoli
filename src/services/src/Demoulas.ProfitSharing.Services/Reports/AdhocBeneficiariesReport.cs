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
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx);
            var employeeSsns = demographicQuery.Select(d => d.Ssn);

            IQueryable<Beneficiary> baseQuery = ctx.Beneficiaries.Include(b => b.Contact)
                .ThenInclude(beneficiaryContact => beneficiaryContact!.ContactInfo);

            baseQuery = req.IsAlsoEmployee ? baseQuery.Where(b => employeeSsns.Contains(b.Contact!.Ssn)) :
                baseQuery.Where(b => !employeeSsns.Contains(b.Contact!.Ssn));

            // Apply pagination
            var pagedBeneficiaries = await baseQuery
                .ToPaginationResultsAsync(req, cancellationToken);

            var beneficiarySsns = pagedBeneficiaries.Results.Select(b => b.Contact!.Ssn).ToHashSet();

            var totalBalanceResult = await _totalService.GetTotalBalanceSet(ctx, req.ProfitYear)
                .Where(x=> beneficiarySsns.Contains(x.Ssn))
                .ToDictionaryAsync(x=> x.Ssn, cancellationToken);

            // Fetch all relevant profit details for paged beneficiaries
            var allProfitDetails = await ctx.ProfitDetails
                .Where(pd => beneficiarySsns.Contains(pd.Ssn) && pd.ProfitYear == req.ProfitYear)
                .Include(profitDetail => profitDetail.ProfitCode)
                .ToListAsync(cancellationToken);

            var filteredList = pagedBeneficiaries.Results.Select(b =>
            {
                var profitDetailsForBeneficiary = allProfitDetails
                    .Where(pd => pd.Ssn == b.Contact!.Ssn)
                    .Select(pd => new ProfitDetailDto(
                        pd.ProfitYear,
                        pd.ProfitCode.Name,
                        pd.Contribution,
                        pd.Earnings,
                        pd.Forfeiture,
                        DateOnly.FromDateTime(pd.CreatedAtUtc.DateTime),
                        pd.Remark)).ToList();

                totalBalanceResult.TryGetValue(b.Contact!.Ssn, out var totalBalance);


                return new BeneficiaryReportDto(
                    b.PsnSuffix,
                    b.Contact?.ContactInfo?.FullName ?? string.Empty,
                    b.Contact != null ? b.Contact.Ssn.MaskSsn() : string.Empty,
                    b.Relationship,
                    totalBalance?.TotalAmount ?? 0,
                    b.BadgeNumber,
                    b.PsnSuffix,
                    profitDetailsForBeneficiary
                )
                { IsExecutive = b.Demographic != null ? b.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly : false };
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
                TotalEndingBalance = totalBalanceResult.Values.Sum(x => x.TotalAmount ?? 0)
            };
        });
    }
}
