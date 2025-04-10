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
            .WithMessage("Beginning date must be within the fiscal year range: {FiscalBegin} through {FiscalEnd}.");


        RuleFor(x => x.EndingDate)
            .NotEmpty().WithMessage("Ending date is required.")
            .MustAsync(BeWithinFiscalYear)
            .WithMessage("Ending date must be within the fiscal year range: {FiscalBegin} through {FiscalEnd}.")
            .GreaterThanOrEqualTo(x => x.BeginningDate)
            .WithMessage("Ending date must be greater than or equal to beginning date.");
    }

    private async Task<bool> BeWithinFiscalYear(RehireForfeituresRequest request, DateTime date, ValidationContext<RehireForfeituresRequest> context, CancellationToken cancellationToken)
    {
        try
        {
            var bracket = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var fiscalBegin = bracket.FiscalBeginDate.ToDateTime(TimeOnly.MinValue);
            var fiscalEnd = bracket.FiscalEndDate.ToDateTime(TimeOnly.MinValue);
            if (date < fiscalBegin || date > fiscalEnd)
            {
                // Add the fiscal year range to the message
                context.MessageFormatter.AppendArgument("FiscalBegin", fiscalBegin.ToString("MMM-dd"));
                context.MessageFormatter.AppendArgument("FiscalEnd", fiscalEnd.ToString("MMM-dd"));
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
