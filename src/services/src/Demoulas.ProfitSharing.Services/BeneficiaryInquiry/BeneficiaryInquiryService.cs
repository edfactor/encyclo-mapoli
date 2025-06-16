using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Data.Interfaces;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.BeneficiaryInquiry;
public class BeneficiaryInquiryService : IBeneficiaryInquiryService
{

    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IFrozenService _frozenService;
    private readonly ITotalService _totalService;

    public BeneficiaryInquiryService(IProfitSharingDataContextFactory dataContextFactory, IFrozenService frozenService, ITotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _frozenService = frozenService;
        _totalService = totalService;   
    }


    public async Task<PaginatedResponseDto<BeneficiaryDto>> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken)
    {
        var frozenStateResponse = await _frozenService.GetActiveFrozenDemographic(cancellationToken);
        short yearEnd = frozenStateResponse.ProfitYear;
       


        var beneficiary = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var result = context.Beneficiaries.Include(x => x.Contact).Include(x => x.Contact.ContactInfo)
            .Where(
                x => 
                (request.BadgeNumber == null || request.BadgeNumber ==0 || x.BadgeNumber == request.BadgeNumber) && 
                (request.PsnSuffix == null || request.PsnSuffix == 0 || x.PsnSuffix == request.PsnSuffix) &&
                (request.Name == null || x.Contact.ContactInfo.FullName.Contains(request.Name)) &&
                (request.Ssn == null || request.Ssn == 0 || x.Contact.Ssn == request.Ssn) &&
                (request.Address == null || x.Contact.Address.Street.Contains(request.Address)) &&
                (request.City == null || x.Contact.Address.City.Contains(request.City)) &&
                (request.State == null || x.Contact.Address.State.Contains(request.State)) &&
                (request.Percentage == null || request.Percentage ==0 || x.Percent == request.Percentage) &&
                (request.KindId == null || x.KindId == request.KindId) 
                ).Skip(request.Skip??0).Take(request.Take??25)
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
            PaginatedResponseDto<BeneficiaryDto> final = await result.ToPaginationResultsAsync(request, cancellationToken);
            return final;
        }
        );
        //setting Current balance
        foreach (var item in beneficiary.Results)
        {
            int ssn = item.Contact.Ssn.ConvertSsnToInt();
            var currentBalanceRes = await _totalService.GetVestingBalanceForSingleMemberAsync(SearchBy.Ssn, ssn, yearEnd, cancellationToken);
            item.CurrentBalance = currentBalanceRes?.CurrentBalance;
            item.Contact.Ssn = ssn.MaskSsn();
        }


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


    public async Task<BeneficiaryKindResponseDto> GetBeneficiaryKind(BeneficiaryKindRequestDto beneficiaryKindRequestDto, CancellationToken cancellation)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            return context.BeneficiaryKinds.Select(x => new BeneficiaryKindDto { Id = x.Id, Name = x.Name }).ToListAsync();
        });

        return new BeneficiaryKindResponseDto() { BeneficiaryKindList = result.Result };
    }
}
