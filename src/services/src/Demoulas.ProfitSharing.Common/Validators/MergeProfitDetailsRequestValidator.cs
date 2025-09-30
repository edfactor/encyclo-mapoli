using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.Demographics;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// FluentValidation validator for MergeProfitDetailsRequest
/// </summary>
public class MergeProfitDetailsRequestValidator : Validator<MergeProfitDetailsRequest>
{
    public MergeProfitDetailsRequestValidator()
    {
        RuleFor(x => x.SourceSsn)
            .NotEmpty()
            .WithMessage("Source SSN is required.")
            .InclusiveBetween(1, 999999999)
            .WithMessage("Source SSN must be between 1 and 999999999.");

        RuleFor(x => x.DestinationSsn)
            .NotEmpty()
            .WithMessage("Destination SSN is required.")
            .InclusiveBetween(1, 999999999)
            .WithMessage("Destination SSN must be between 1 and 999999999.");

        RuleFor(x => x)
            .Must(x => x.SourceSsn != x.DestinationSsn)
            .WithMessage("Source SSN and Destination SSN cannot be the same.")
            .WithName("SSN Validation");
    }
}
