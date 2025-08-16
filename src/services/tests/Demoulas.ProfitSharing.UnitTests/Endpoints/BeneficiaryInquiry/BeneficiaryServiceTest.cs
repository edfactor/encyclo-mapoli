using Demoulas.ProfitSharing.Api;
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
                Percent = 100
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


}
