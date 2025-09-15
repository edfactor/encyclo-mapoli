using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using FluentValidation.Results;
using Moq;
using System;

namespace Demoulas.ProfitSharing.UnitTests.Common.Validators;

public class MilitaryContributionRequestValidatorTests
{
    private static (MilitaryContributionRequestValidator Validator, Mock<IEmployeeLookupService> EmployeeLookupMock) CreateValidator()
    {
        var employeeLookupMock = new Mock<IEmployeeLookupService>(MockBehavior.Strict);
        employeeLookupMock
            .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new MilitaryContributionRequestValidator(employeeLookupMock.Object);
        return (validator, employeeLookupMock);
    }

    private static CreateMilitaryContributionRequest ValidRequest(short? profitYear = null, DateTime? contributionDate = null)
    {
        short year = profitYear ?? (short)DateTime.Today.Year;
        var date = contributionDate ?? new DateTime(year, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        return new CreateMilitaryContributionRequest
        {
            BadgeNumber = 12345,
            ContributionAmount = 100m,
            ProfitYear = year,
            IsSupplementalContribution = true,
            ContributionDate = date
        };
    }

    [Fact]
    public async Task Valid_request_passes_validation()
    {
        var (validator, _) = CreateValidator();
        // Use previous year to avoid boundary with current-year rule and midnight rollovers
        var today = DateTime.Today;
        short prevYear = (short)(today.Year - 1);
        CreateMilitaryContributionRequest req = ValidRequest(prevYear, new DateTime(prevYear, 1, 15, 0, 0, 0, DateTimeKind.Utc));
        ValidationResult result = await validator.ValidateAsync(req);
        Assert.True(result.IsValid, string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
    }

    [Fact]
    public async Task ContributionAmount_must_be_greater_than_zero()
    {
        var (validator, _) = CreateValidator();
        var req = ValidRequest() with { ContributionAmount = 0m };
        var result = await validator.ValidateAsync(req);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            (e.PropertyName?.Contains(nameof(CreateMilitaryContributionRequest.ContributionAmount), StringComparison.OrdinalIgnoreCase) ?? false)
            || e.ErrorMessage.Contains("greater than zero", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ProfitYear_must_be_within_bounds()
    {
        var (validator, _) = CreateValidator();
        // Less than 2020
        var tooLow = ValidRequest(2019, new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var lowResult = await validator.ValidateAsync(tooLow);
        Assert.False(lowResult.IsValid);

        // Greater than current year
        var today = DateTime.Today;
        short nextYear = (short)(today.Year + 1);
        var tooHigh = ValidRequest(nextYear, new DateTime(nextYear, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var highResult = await validator.ValidateAsync(tooHigh);
        Assert.False(highResult.IsValid);
    }

    [Fact]
    public async Task BadgeNumber_must_exist_and_be_positive()
    {
        var (validator, employeeLookupMock) = CreateValidator();
        // Negative/zero badge
        var invalidBadge = ValidRequest() with { BadgeNumber = 0 };
        var invalidBadgeResult = await validator.ValidateAsync(invalidBadge);
        Assert.False(invalidBadgeResult.IsValid);

        // Non-existent badge
        employeeLookupMock.Reset();
        employeeLookupMock
            .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var missingBadge = ValidRequest() with { BadgeNumber = 999999 };
        var missingBadgeResult = await validator.ValidateAsync(missingBadge);
        Assert.False(missingBadgeResult.IsValid);
        Assert.Contains(missingBadgeResult.Errors, e =>
            (e.PropertyName?.Contains(nameof(CreateMilitaryContributionRequest.BadgeNumber), StringComparison.OrdinalIgnoreCase) ?? false)
            || e.ErrorMessage.Contains("badge number was not found", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ContributionDate_cannot_be_in_future()
    {
        var (validator, _) = CreateValidator();
        var today = DateTime.Today;
        var req = ValidRequest((short)today.Year, today.AddDays(1));
        var result = await validator.ValidateAsync(req);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            (e.PropertyName?.Contains(nameof(CreateMilitaryContributionRequest.ContributionDate), StringComparison.OrdinalIgnoreCase) ?? false)
            || e.ErrorMessage.Contains("cannot be in the future", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Supplemental_contribution_bypasses_duplicate_check()
    {
        // Even if there are existing records, supplemental should pass
        var (validator, _) = CreateValidator();
        var today = DateTime.Today;
        short prevYear = (short)(today.Year - 1);
        var req = ValidRequest(prevYear, new DateTime(prevYear, 1, 15, 0, 0, 0, DateTimeKind.Utc)) with { IsSupplementalContribution = true };
        var result = await validator.ValidateAsync(req);
        Assert.True(result.IsValid, string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
    }
}
