using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Listener;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.BeneficiaryInquiry;
public class BeneficiaryServiceTest : ApiTestBase<Program>
{
    private readonly IBeneficiaryInquiryService _beneficiaryService;
    private readonly List<BeneficiaryDto> _beneficiaryList;
    private readonly BeneficiaryDto _beneficiaryDetail;
    private readonly List<BeneficiarySearchFilterResponse> _beneficiarySearchResponse;

    public BeneficiaryServiceTest()
    {
        _beneficiaryService = ServiceProvider?.GetRequiredService<IBeneficiaryInquiryService>()!;
        _beneficiaryList = new List<BeneficiaryDto>()
        {
            new BeneficiaryDto()
            {
                Id = 145,
                PsnSuffix = 1000,
                BadgeNumber = 703244,
                DemographicId = 3173,
                CurrentBalance = 0,
                    Ssn = "XXX-XX-0692",
                    DateOfBirth = DateOnly.FromDateTime(new DateTime(1984,3,4,0,0,0,DateTimeKind.Utc)),
                        Street = "243 SECOND COURT",
                        Street2 = null,
                        City = "PEPPERELL",
                        State ="MA",
                        PostalCode = "2318",
                        CountryIso = "US",
                        FullName ="DELAROSA, ZOE",
                        LastName = "DELAROSA",
                        FirstName = "ZOE",
                        MiddleName =null,
                        PhoneNumber = null,
                        MobileNumber = null,
                        EmailAddress = null,
                    CreatedDate = DateOnly.FromDateTime(new DateTime(2025,5,8,0,0,0,DateTimeKind.Utc)),
                Relationship = "DAUGHTER",
                KindId ='P',
                Kind =new BeneficiaryKindDto()
                {
                    Id = 'P',
                    Name = "Primary"
                },
                Percent = 100,
                IsExecutive = false,
            }
        };

        _beneficiaryDetail = new BeneficiaryDto()
        {
            Id = 145,
            PsnSuffix = 1000,
            BadgeNumber = 703244,
            DemographicId = 3173,
            CurrentBalance = 0,
            Ssn = "XXX-XX-0692",
            DateOfBirth = DateOnly.FromDateTime(new DateTime(1984, 3, 4, 0, 0, 0, DateTimeKind.Utc)),
            Street = "243 SECOND COURT",
            Street2 = null,
            City = "PEPPERELL",
            State = "MA",
            PostalCode = "2318",
            CountryIso = "US",
            FullName = "DELAROSA, ZOE",
            LastName = "DELAROSA",
            FirstName = "ZOE",
            MiddleName = null,
            PhoneNumber = null,
            MobileNumber = null,
            EmailAddress = null,
            CreatedDate = DateOnly.FromDateTime(new DateTime(2025, 5, 8, 0, 0, 0, DateTimeKind.Utc)),
            Relationship = "DAUGHTER",
            KindId = 'P',
            Kind = new BeneficiaryKindDto()
            {
                Id = 'P',
                Name = "Primary"
            },
            Percent = 100,
            IsExecutive = false,
        };
        _beneficiarySearchResponse = new List<BeneficiarySearchFilterResponse>()
        {
            new BeneficiarySearchFilterResponse()
            {
                Age = 41, 
                BadgeNumber = 703244,
                City = "PEPPERELL",
                Name= "DELAROSA, ZOE",
                PsnSuffix= 1000, 
                Ssn ="XXX-XX-0692",
                State = "MA",
                Street = "243 SECOND COURT",
                Zip = "2318"
            }
        };
    }

    [Fact(DisplayName = "Get beneficiary by badge_number & psn_suffix")]
    public async Task GetBeneficiary()
    {
        var res = await _beneficiaryService.GetBeneficiary(new BeneficiaryRequestDto() { BadgeNumber = 703244 }, CancellationToken.None);
        Assert.NotNull(res);
        res.Beneficiaries?.Results.ShouldBeEquivalentTo(_beneficiaryList);
    }
    [Fact(DisplayName = "Get Beneficiary Detail")]
    public async Task GetBeneficiaryDetail()
    {
        var res = await _beneficiaryService.GetBeneficiaryDetail(new BeneficiaryDetailRequest() { BadgeNumber = 703244, PsnSuffix = 1000 }, CancellationToken.None);
        Assert.NotNull(res);
        res.ShouldBeEquivalentTo(_beneficiaryDetail);
    }

    [Fact(DisplayName = "Beneficiary Search Filter")]
    public async Task BeneficiarySearchFilter()
    {
        var res = await _beneficiaryService.BeneficiarySearchFilter(new BeneficiarySearchFilterRequest() { MemberType = 2, BadgeNumber = 703244, PsnSuffix = 1000},CancellationToken.None);
        Assert.NotNull(res);
        res.Results.ShouldBeEquivalentTo(_beneficiarySearchResponse);
    }


}
