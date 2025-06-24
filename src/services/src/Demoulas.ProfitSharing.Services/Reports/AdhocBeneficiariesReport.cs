using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
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

    public AdhocBeneficiariesReport(IProfitSharingDataContextFactory dataContextFactory, IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
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

            var beneficiarySsns =await baseQuery.Select(b => b.Contact!.Ssn).ToHashSetAsync(cancellationToken);

            // Fetch all relevant profit details in one query
            var allProfitDetails = ctx.ProfitDetails
                .Where(pd => beneficiarySsns.Contains(pd.Ssn) && pd.ProfitYear >= req.MinProfitYear)
                .Include(profitDetail => profitDetail.ProfitCode);
               

           

            var filteredList = new List<BeneficiaryReportDto>();
            decimal totalBalance = 0;
            foreach (var b in baseQuery)
            {
                var profitDetailsForBeneficiary = allProfitDetails
                    .Where(pd => pd.Ssn == b.Contact!.Ssn)
                    .ToList();
                
                if (!profitDetailsForBeneficiary.Any()) { continue; }
                
                var profitDetailsList = req.DetailSwitch
                    ? profitDetailsForBeneficiary.Select(pd => new ProfitDetailDto(
                        pd.ProfitYear,
                        pd.ProfitCode.Name,
                        pd.Contribution,
                        pd.Earnings,
                        pd.Forfeiture,
                        DateOnly.FromDateTime(pd.TransactionDate.DateTime),
                        pd.Remark)).ToList()
                    : null;
                
                var dto = new BeneficiaryReportDto(
                    b.Id,
                    b.Contact?.ContactInfo?.FullName ?? string.Empty,
                    b.Contact != null ? b.Contact.Ssn.ToString() : string.Empty,
                    b.Relationship,
                    b.CurrentBalance,
                    b.BadgeNumber,
                    profitDetailsList
                );
                filteredList.Add(dto);
                totalBalance += b.CurrentBalance;
            }
            
            var resultList = filteredList;
            var totalCount = resultList.Count;
            return new AdhocBeneficiariesReportResponse
            {
                ReportName = "adhoc-beneficiaries-report",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue,
                Response = new PaginatedResponseDto<BeneficiaryReportDto>(new())
                {
                    Results = resultList!,
                    Total = totalCount
                },
                TotalEndingBalance = totalBalance,
                TotalCount = totalCount
            };
        });
    }
}
