using Demoulas.Common.Contracts.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public abstract class PaginationValidatorBase<TPagination> : Validator<TPagination> where TPagination : SortedPaginationRequestDto 
{
    protected PaginationValidatorBase()
    {
        _ = RuleFor(x => x.Skip)
            .Must(i => i.GetValueOrDefault(0) >= 0)
            .WithMessage("Must contain a positive value");

        _ = RuleFor(x => x.Take)
            .Must(i => i.GetValueOrDefault(0) >= 0)
            .WithMessage("Must contain a positive value");
    }
}
