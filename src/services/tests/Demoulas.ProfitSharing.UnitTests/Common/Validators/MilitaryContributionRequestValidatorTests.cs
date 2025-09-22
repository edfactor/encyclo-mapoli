using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Contracts;
using System.Collections.Generic;
using FluentValidation.Results;
using Moq;
using System;

namespace Demoulas.ProfitSharing.UnitTests.Common.Validators;

public class MilitaryContributionRequestValidatorTests
{
    private static (MilitaryContributionRequestValidator Validator, Mock<IEmployeeLookupService> EmployeeLookupMock, Mock<IMilitaryService> MilitaryServiceMock) CreateValidator()
    {
        var employeeLookupMock = new Mock<IEmployeeLookupService>(MockBehavior.Strict);
        employeeLookupMock
            .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        employeeLookupMock
            .Setup(x => x.GetEarliestHireDateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateOnly((short)DateTime.Today.Year - 1, 1, 1));
        employeeLookupMock
            .Setup(x => x.GetDateOfBirthAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateOnly((short)DateTime.Today.Year - 30, 1, 1));

        var militaryServiceMock = new Mock<IMilitaryService>(MockBehavior.Strict);
        // Default: return success with empty results for GetMilitaryServiceRecordAsync
        militaryServiceMock
            .Setup(m => m.GetMilitaryServiceRecordAsync(It.IsAny<Demoulas.ProfitSharing.Common.Contracts.Request.Military.MilitaryContributionRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse>>.Success(new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse> { Results = new List<Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse>() }));

        var validator = new MilitaryContributionRequestValidator(employeeLookupMock.Object, militaryServiceMock.Object);
        return (validator, employeeLookupMock, militaryServiceMock);
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
        var (validator, _, _) = CreateValidator();
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
        var (validator, _, _) = CreateValidator();
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
        var (validator, _, _) = CreateValidator();
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
        var (validator, employeeLookupMock, _) = CreateValidator();
        // Negative/zero badge
        var invalidBadge = ValidRequest() with { BadgeNumber = 0 };
        var invalidBadgeResult = await validator.ValidateAsync(invalidBadge);
        Assert.False(invalidBadgeResult.IsValid);

        // Non-existent badge
        employeeLookupMock.Reset();
        employeeLookupMock
            .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        employeeLookupMock
            .Setup(x => x.GetEarliestHireDateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateOnly?)null);
        employeeLookupMock
            .Setup(x => x.GetDateOfBirthAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateOnly?)null);

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
        var (validator, _, _) = CreateValidator();
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
        var (validator, _, _) = CreateValidator();
        var today = DateTime.Today;
        short prevYear = (short)(today.Year - 1);
        var req = ValidRequest(prevYear, new DateTime(prevYear, 1, 15, 0, 0, 0, DateTimeKind.Utc)) with { IsSupplementalContribution = true };
        var result = await validator.ValidateAsync(req);
        Assert.True(result.IsValid, string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
    }

    [Fact]
    public async Task Contribution_before_hire_date_is_rejected()
    {
        var (validator, employeeLookupMock, _) = CreateValidator();
        // Employee earliest hire year is set to previous year by default in CreateValidator.
        var today = DateTime.Today;
        short twoYearsAgo = (short)(today.Year - 2);
        // contribution in a year before hire
        var req = ValidRequest(twoYearsAgo, new DateTime(twoYearsAgo, 1, 15, 0, 0, 0, DateTimeKind.Utc)) with { IsSupplementalContribution = false };
        var result = await validator.ValidateAsync(req);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("before the employee's earliest known hire year", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Contribution_by_employee_under_21_is_rejected()
    {
        var (validator, employeeLookupMock, _) = CreateValidator();
        // Set DOB so employee is 20 at contribution date
        var today = DateTime.Today;
        short year = (short)(today.Year - 20);
        employeeLookupMock.Reset();
        employeeLookupMock
            .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        employeeLookupMock
            .Setup(x => x.GetEarliestHireDateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateOnly((short)today.Year - 1, 1, 1));
        employeeLookupMock
            .Setup(x => x.GetDateOfBirthAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateOnly(year, today.Month, Math.Min(1, today.Day)));

        var req = ValidRequest((short)today.Year, new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Utc)) with { IsSupplementalContribution = false };
        var result = await validator.ValidateAsync(req);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("at least 21 years old", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Duplicate_detection_by_contribution_year_rejects_regular_contribution_when_v_only_record_exists()
    {
        var (validator, _, militaryServiceMock) = CreateValidator();

        // Arrange: the user selects ProfitYear = current (e.g., 2025) but ContributionDate = 2020.
        // The service should look up existing records by the contribution year (2020) and reject
        // a non-supplemental contribution when a V-only (YearsOfServiceCredit = 1) record exists for 2020.
        var contributionYear = 2020;
        var req = ValidRequest((short)DateTime.Today.Year, new DateTime(contributionYear, 1, 15, 0, 0, 0, DateTimeKind.Utc)) with { IsSupplementalContribution = false };

        // Configure the military service mock to return an existing V-only record for contributionYear
        militaryServiceMock.Reset();
        militaryServiceMock
            .Setup(m => m.GetMilitaryServiceRecordAsync(It.Is<Demoulas.ProfitSharing.Common.Contracts.Request.Military.MilitaryContributionRequest>(r => r.ProfitYear == contributionYear && r.BadgeNumber == req.BadgeNumber), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse>>.Success(new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse>
            {
                Results = new List<Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse>
                {
                    new Demoulas.ProfitSharing.Common.Contracts.Response.Military.MilitaryContributionResponse
                    {
                        BadgeNumber = req.BadgeNumber,
                        ProfitYear = (short)contributionYear,
                        Amount = 100,
                        IsSupplementalContribution = false
                    }
                }
            }));

        var result = await validator.ValidateAsync(req);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Regular Contribution already recorded for year", StringComparison.OrdinalIgnoreCase));
    }
}
