using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation;

/// <summary>
/// Validator for IdsRequest to enforce batch size limits and ensure valid IDs.
/// Guards against degenerate queries with zero IDs or excessive batch sizes.
/// </summary>
public sealed class IdsRequestValidator : AbstractValidator<IdsRequest>
{
    public IdsRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotNull()
            .WithMessage("Ids cannot be null.");

        RuleFor(x => x.Ids)
            .Must(ids => ids != null && ids.Length > 0)
            .WithMessage("At least one ID must be provided.");

        RuleFor(x => x.Ids)
            .Must(ids => ids == null || ids.Length <= 1000)
            .WithMessage("Cannot process more than 1000 IDs in a single request.");

        RuleFor(x => x.Ids)
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("All IDs must be positive integers.");

        RuleFor(x => x.Ids)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Length)
            .WithMessage("Duplicate IDs are not allowed.");
    }
}