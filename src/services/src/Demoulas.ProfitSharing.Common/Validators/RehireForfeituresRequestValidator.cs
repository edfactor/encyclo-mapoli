using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Validators;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.Validators;

public class RehireForfeituresRequestValidator : PaginationValidatorBase<RehireForfeituresRequest>
{
    private readonly ICalendarService _calendarService;
    private readonly ILogger<RehireForfeituresRequestValidator> _logger;

    public RehireForfeituresRequestValidator(
        ICalendarService calendarService,
        ILoggerFactory factory)
    {
        _calendarService = calendarService;
        _logger = factory.CreateLogger<RehireForfeituresRequestValidator>();

        RuleFor(x => x.ProfitYear).GreaterThan((short)0).WithMessage("Profit year must be greater than zero.");

        RuleFor(x => x.BeginningDate)
            .NotEmpty().WithMessage("Beginning date is required.")
            .MustAsync(BeWithinFiscalYear)
            .WithMessage("Beginning date must be within the fiscal year range.");

        RuleFor(x => x.EndingDate)
            .NotEmpty().WithMessage("Ending date is required.")
            .MustAsync(BeWithinFiscalYear)
            .WithMessage("Ending date must be within the fiscal year range.")
            .GreaterThanOrEqualTo(x => x.BeginningDate)
            .WithMessage("Ending date must be greater than or equal to beginning date.");
    }

    private async Task<bool> BeWithinFiscalYear(RehireForfeituresRequest request, DateTime date, ValidationContext<RehireForfeituresRequest> context, CancellationToken cancellationToken)
    {
        try
        {
            var bracket = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);

            if (date < bracket.FiscalBeginDate.ToDateTime(TimeOnly.MinValue) || date > bracket.FiscalEndDate.ToDateTime(TimeOnly.MinValue))
            {
                _logger.LogWarning("Date {Date} is outside fiscal year {ProfitYear} boundaries ({Start} - {End})",
                    date, request.ProfitYear, bracket.FiscalBeginDate, bracket.FiscalEndDate);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating date against fiscal year boundaries");
            return false;
        }
    }
}
