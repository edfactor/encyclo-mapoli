using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

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
                CurrentBalance = null,
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
                        PhoneNumber = "",
                        MobileNumber = "",
                        EmailAddress = "",
                    CreatedDate = DateOnly.FromDateTime(new DateTime(2025,5,8,0,0,0,DateTimeKind.Utc)),
                Relationship = "DAUGHTER",
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
            CurrentBalance = null,
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
            PhoneNumber = "",
            MobileNumber = "",
            EmailAddress = "",
            CreatedDate = DateOnly.FromDateTime(new DateTime(2025, 5, 8, 0, 0, 0, DateTimeKind.Utc)),
            Relationship = "DAUGHTER",
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
                FullName = "DELAROSA, ZOE",
                PsnSuffix= 1000,
                Ssn ="XXX-XX-0692",
                State = "MA",
                Street = "243 SECOND COURT",
                Zip = "2318"
            }
        };
    }

    [Fact(DisplayName = "Get beneficiary by badge_number & psn_suffix", Skip = "MockQueryable cannot properly evaluate complex Include chains with Where filters - test data mismatch")]
    public async Task GetBeneficiary()
    {
        var res = await _beneficiaryService.GetBeneficiary(new BeneficiaryRequestDto() { BadgeNumber = 703244 }, CancellationToken.None);
        Assert.NotNull(res);
        Assert.NotNull(res.Beneficiaries?.Results);
        res.Beneficiaries.Results.ShouldBeEquivalentTo(_beneficiaryList, nameof(BeneficiaryDto.CurrentBalance));
    }
    [Fact(DisplayName = "Get Beneficiary Detail", Skip = "MockQueryable cannot properly evaluate complex Include chains with Where filters - test data mismatch")]
    public async Task GetBeneficiaryDetail()
    {
        var res = await _beneficiaryService.GetBeneficiaryDetail(new BeneficiaryDetailRequest() { BadgeNumber = 703244, PsnSuffix = 1000 }, CancellationToken.None);
        Assert.NotNull(res);
        res.ShouldBeEquivalentTo(_beneficiaryDetail, nameof(BeneficiaryDetailResponse.CurrentBalance));
    }

    [Fact(DisplayName = "Beneficiary Search Filter", Skip = "MockQueryable cannot properly evaluate complex Include chains with Where filters - test data mismatch")]
    public async Task BeneficiarySearchFilter()
    {
        var res = await _beneficiaryService.BeneficiarySearchFilter(new BeneficiarySearchFilterRequest() { MemberType = 2, BadgeNumber = 703244, PsnSuffix = 1000 }, CancellationToken.None);
        Assert.NotNull(res);
        res.Results.ShouldBeEquivalentTo(_beneficiarySearchResponse);
    }


}
