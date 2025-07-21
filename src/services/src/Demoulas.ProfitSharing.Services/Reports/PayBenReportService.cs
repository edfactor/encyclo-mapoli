using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;
public class PayBenReportService : IPayBenReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    public PayBenReportService(IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
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
            .Include(x => x.Contact.ContactInfo)
            .Include(x => x.Demographic.ContactInfo);

            var res = query.Select(x => new PayBenReportResponse()
            {
                Ssn = x.Contact.Ssn.ToString(),
                BeneficiaryFullName = x.Contact.ContactInfo.FullName,
                DemographicFullName = x.Demographic.ContactInfo.FullName,
                Psn = x.Psn,
                Badge = x.BadgeNumber,
                Percentage = x.Percent
            });
            PaginatedResponseDto<PayBenReportResponse> final = await res.ToPaginationResultsAsync(request, cancellationToken);
            return final;
        });
        foreach (var item in result.Results)
        {
            item.Ssn = Convert.ToInt32(item.Ssn).MaskSsn();
        }
        return result;
    }
}
