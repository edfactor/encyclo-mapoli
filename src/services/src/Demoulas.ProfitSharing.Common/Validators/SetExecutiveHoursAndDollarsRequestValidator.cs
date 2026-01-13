using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class SetExecutiveHoursAndDollarsRequestValidator : Validator<SetExecutiveHoursAndDollarsRequest>
{

    public SetExecutiveHoursAndDollarsRequestValidator()
    {
        RuleFor(req => req.ExecutiveHoursAndDollars.Count)
            .GreaterThan(0)
            .WithMessage("At least one employee must be provided");
        RuleFor(req => req.ExecutiveHoursAndDollars.Select(d => d.BadgeNumber).Distinct().Count() ==
                       req.ExecutiveHoursAndDollars.Count)
            .Equal(true)
            .WithMessage("Badge Numbers must be unique.");
    }
}

