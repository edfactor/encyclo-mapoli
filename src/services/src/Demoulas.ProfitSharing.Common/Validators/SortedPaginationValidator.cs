using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Validators;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class SortedPaginationValidator<TPagination, TResponse> : PaginationValidatorBase<TPagination> where TPagination : SortedPaginationRequestDto
{
    public SortedPaginationValidator()
    {
        _ = RuleFor(x => x.SortBy)
            .Must(IsValidFieldName)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                       .WithMessage($"SortBy must be a property of {typeof(TResponse)}.");
    }

    private static bool IsValidFieldName(string? fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return true;
        }
        var properties = typeof(TResponse).GetProperties();
        return properties.Any(p => p.Name.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
    }
}
