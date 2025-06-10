using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.BeneficiaryInquiry;
public class BeneficiaryInquiryService : IBeneficiaryInquiryService
{

    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public BeneficiaryInquiryService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    public async Task<PaginatedResponseDto<BeneficiaryDto>> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken)
    {
        var beneficiary = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var result = context.Beneficiaries.Include(x => x.Contact)
            .Where(x => x.BadgeNumber == request.BadgeNumber && x.PsnSuffix == request.PsnSuffix)
            .Select(x => new BeneficiaryDto()
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                KindId = x.KindId,
                Contact = new BeneficiaryContactDto()
                {
                    Id = x.Contact != null ? x.Contact.Id : 0,
                    CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                    DateOfBirth = x.Contact != null ? x.Contact.DateOfBirth : DateOnly.MaxValue,
                    Ssn = x.Contact != null ? x.Contact.Ssn.ToString() : null,
                    Address = new Common.Contracts.Response.AddressResponseDto()
                    {
                        City = x.Contact != null ? x.Contact.Address.City : null,
                        CountryIso = x.Contact != null ? x.Contact.Address.CountryIso ?? "" : "",
                        PostalCode = x.Contact != null ? x.Contact.Address.PostalCode : null,
                        State = x.Contact != null ? x.Contact.Address.State : null,
                        Street = x.Contact != null ? x.Contact.Address.Street : "",
                        Street2 = x.Contact != null ? x.Contact.Address.Street2 : null,
                    },
                    ContactInfo = new Common.Contracts.Response.ContactInfoResponseDto()
                    {
                        FirstName = x.Contact != null ? x.Contact.ContactInfo.FirstName : "",
                        LastName = x.Contact != null ? x.Contact.ContactInfo.LastName : "",
                        EmailAddress = x.Contact != null ? x.Contact.ContactInfo.EmailAddress : "",
                        FullName = x.Contact != null ? x.Contact.ContactInfo.FullName : "",
                        MiddleName = x.Contact != null ? x.Contact.ContactInfo.MiddleName : null,
                        MobileNumber = x.Contact != null ? x.Contact.ContactInfo.MobileNumber : "",
                        PhoneNumber = x.Contact != null ? x.Contact.ContactInfo.PhoneNumber : ""
                    }
                },
                Kind = new BeneficiaryKindDto()
                {
                    Id = x.Kind != null ? x.Kind.Id : '0',
                    Name = x.Kind != null ? x.Kind.Name : null
                },
                Relationship = x.Relationship
            });
            PaginatedResponseDto<BeneficiaryDto> final = await result.ToPaginationResultsAsync(request,cancellationToken);
            return final;
        }
        );

        return beneficiary;
    }

    public async Task<BeneficiaryTypesResponseDto> GetBeneficiaryTypes(BeneficiaryTypesRequestDto beneficiaryTypesRequestDto, CancellationToken cancellation)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            return context.BeneficiaryTypes.Select(x => new BeneficiaryTypeDto { Id = x.Id, Name = x.Name }).ToListAsync();
        });

        return new BeneficiaryTypesResponseDto() { BeneficiaryTypeList = result.Result };
    }
}
