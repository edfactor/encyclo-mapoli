using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitShareEdit;

public class EmployeeTests : ApiTestBase<Program>
{
    private const decimal DefaultEarningsPercent = 5;
    private readonly Demographic _demographic;
    private readonly PayProfit _payProfit;
    private readonly List<ProfitDetail> _profitDetails;
    private readonly IProfitShareEditService _service;
    private readonly short _thisYear;

    public EmployeeTests()
    {
        // create mock database with just 1 employee with two profit detail rows in last year.
        ScenarioFactory sf = new ScenarioFactory().CreateOneEmployeeWithProfitDetails();
        MockDbContextFactory = sf.BuildMocks();
        _demographic = sf.Demographics[0];
        _payProfit = sf.PayProfits[0];
        _profitDetails = sf.ProfitDetails;
        _thisYear = sf.ThisYear;

        _service = ServiceProvider?.GetRequiredService<IProfitShareEditService>()!;
    }

    [Fact]
    public async Task ensure_happy_path_works_fine()
    {
        // Arrange
        _demographic.ContactInfo.FullName = "Joey, Doughnuts";

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Count.Should().Be(1);
        ProfitShareEditMemberRecordResponse m = records[0];
        m.Name.Should().Be("Joey, Doughnuts");
        // test's default balance is 1000. test's default requested earnings 5%
        m.EarningsAmount.Should().Be(1000 * .05m);
    }

    [Fact]
    public async Task if_not_enrolled_and_has_no_YIP_then_they_do_not_show_up()
    {
        // Arrange
        _payProfit.EnrollmentId = /*0*/ Enrollment.Constants.NotEnrolled;
        _profitDetails[0].YearsOfServiceCredit = 0;

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Should().BeEmpty();
    }

    [Fact]
    public async Task if_years_in_plan_is_zero_but_is_enrolled_is_true_then_ensure_member_shows_up()
    {
        // Arrange
        _payProfit.EnrollmentId = /*2*/ Enrollment.Constants.NewVestingPlanHasContributions;

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Should().NotBeEmpty();
    }

    [Fact]
    public async Task if_years_in_plan_is_NOT_zero_and_NOT_enrolled_then_ensure_they_show()
    {
        // Arrange
        _payProfit.EnrollmentId = /*2*/ Enrollment.Constants.NewVestingPlanHasContributions;
        _profitDetails[0].YearsOfServiceCredit = 1;

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Should().NotBeEmpty();
    }


    [Fact]
    public async Task under_21_no_without_balance()
    {
        // Arrange
        _payProfit.ZeroContributionReasonId = ZeroContributionReason.Constants.Under21WithOver1Khours /*1*/;
        _profitDetails[0].Contribution = 0; // set them to no money

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        ProfitShareEditMemberRecordResponse m = records[0];

        m.Remark.Should().Be(CommentType.Constants.VOnly.Name);
        m.RecordChangeSummary.Should().Be("18,19,20 > 1000");
    }


    [Fact]
    public async Task under_21_with_balance()
    {
        // Arrange
        _payProfit.ZeroContributionReasonId = /*1*/ ZeroContributionReason.Constants.Under21WithOver1Khours;

        // Act
        ProfitShareUpdateRequest req = DefaultRequest();
        req.ContributionPercent = 10;
        req.EarningsPercent = 2;
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        ProfitShareEditMemberRecordResponse m = records[0];

        m.ContributionAmount.Should().Be(0);
        m.ForfeitureAmount.Should().Be(0);
        m.Remark.Should().BeNull();
    }

    [Fact]
    public async Task employee_with_ETVA_expect_earnings_on_both_an_8_record_and_a_0_record()
    {
        // Arrange
        _payProfit.Etva = 1000m;

        _profitDetails[0].ProfitCodeId.Should().Be(0);
        _profitDetails[0].Contribution = 3000m;

        // Balance is 2000 in profit details
        // ETVA is 1000
        // Earnings is 5%
        // rec 0 should have 2000 * %5 = 100
        // rec 8 should have 1000 * %5 = 50

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(DefaultRequest(), CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Should().HaveCount(2);
        ProfitShareEditMemberRecordResponse r1 = records[0];
        r1.Code.Should().Be(8);
        r1.EarningsAmount.Should().Be(1000 /*ETVA Bal*/ * DefaultEarningsPercent / 100);

        ProfitShareEditMemberRecordResponse r2 = records[1];
        r2.Code.Should().Be(0);
        r2.EarningsAmount.Should().Be(2000 /*Non Etva Bal*/ * DefaultEarningsPercent / 100);
    }


    [Fact]
    public async Task secondary_earnings_no_etva_happy_path()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _thisYear, SecondaryEarningsPercent = 3 };

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(req, CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Count.Should().Be(1);
        ProfitShareEditMemberRecordResponse m = records[0];

        // test's default balance is 1000. test's default requested earnings 5%
        m.YearExtension.Should().Be(2);
        m.EarningsAmount.Should().Be(1000 * .03m);
    }

    [Fact]
    public async Task secondary_earnings_with_etva()
    {
        // Arrange
        _payProfit.Etva = 1000m;

        // 0 record has Vested amounts
        _profitDetails[0].ProfitCodeId.Should().Be(0);
        _profitDetails[0].Contribution = 3000m;

        ProfitShareUpdateRequest req = new() { ProfitYear = _thisYear, SecondaryEarningsPercent = 3 };

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(req, CancellationToken.None);

        // Assert
        List<ProfitShareEditMemberRecordResponse> records = response.Response.Results.ToList();
        records.Count.Should().Be(2);
        ProfitShareEditMemberRecordResponse r1 = records[0];
        ProfitShareEditMemberRecordResponse r2 = records[1];

        // 3% of 3000 = 90
        r1.Code.Should().Be( /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings);
        r1.YearExtension.Should().Be(2);
        r1.EarningsAmount.Should().Be(30 /*1000 * .03m*/);

        r2.Code.Should().Be( /*0*/ ProfitCode.Constants.IncomingContributions);
        r2.YearExtension.Should().Be(2);
        r2.EarningsAmount.Should().Be(60 /*2000 * .03m*/);
    }


    private ProfitShareUpdateRequest DefaultRequest()
    {
        return new ProfitShareUpdateRequest { ProfitYear = _thisYear, EarningsPercent = DefaultEarningsPercent };
    }
}
