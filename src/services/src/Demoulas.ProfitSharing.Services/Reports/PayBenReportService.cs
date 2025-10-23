using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;
public class PayBenReportService : IPayBenReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    public PayBenReportService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    public async Task<PaginatedResponseDto<PayBenReportResponse>> GetPayBenReport(PayBenReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = context.Beneficiaries
            .Include(x => x.Demographic)
            .Include(x => x.Contact)
            .ThenInclude(x => x!.ContactInfo)
            .Include(x => x.Demographic)
            .ThenInclude(x => x!.ContactInfo).Where(x => request.Id == null || x.Id == request.Id);


            var res = query.Select(x => new PayBenReportResponse()
            {
                Ssn = x.Contact != null ? x.Contact.Ssn.MaskSsn() : string.Empty,
                BeneficiaryFullName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FullName ?? string.Empty : string.Empty,
                DemographicFullName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.FullName ?? string.Empty : string.Empty,
                Psn = $"{x.BadgeNumber}{x.PsnSuffix:D4}",
                BadgeNumber = x.BadgeNumber,
                Percentage = x.Percent
            });
            PaginatedResponseDto<PayBenReportResponse> final = await res.ToPaginationResultsAsync(request, cancellationToken);
            return final;
        }, cancellationToken);
       
        return result;
    }
}
