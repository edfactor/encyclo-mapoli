using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.BeneficiaryInquiry;
public class BeneficiaryService : IBeneficiaryService
{

    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public BeneficiaryService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    public async Task<List<BeneficiaryDto>> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken)
    {
        var beneficiary =  await _dataContextFactory.UseReadOnlyContext(context =>
            context.Beneficiaries.Include(x => x.Contact)
            .Where(x => x.BadgeNumber == request.BadgeNumber && x.PsnSuffix == request.PsnSuffix)
            .Select(x => new BeneficiaryDto()
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                BeneficiaryContactId = x.BeneficiaryContactId,
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
            }).ToListAsync(cancellationToken)
        );

        return beneficiary;
    }
}
