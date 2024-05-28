using Demoulas.Common.Data.Contexts.DTOs.Request;
using Demoulas.Common.Data.Contexts.DTOs.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public Task<PaginatedResponseDto<DemographicsResponseDto>> GetAllDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async c =>
        {
            return await c.Demographics.Select(d => new DemographicsResponseDto
            {
                BadgeNumber = d.BadgeNumber,
                Address = new AddressResponseDto
                {
                    Street = d.Address.Street,
                    Street2 = d.Address.Street2,
                    City = d.Address.City,
                    State = d.Address.State,
                    PostalCode = d.Address.PostalCode,
                    CountryISO = d.Address.CountryISO,
                }
            }).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
        });

    }
}
