using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class CreateBeneficiaryContactEndpoint : ProfitSharingEndpoint<CreateBeneficiaryContactRequest, Results<Ok<CreateBeneficiaryContactResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;
    private readonly ILogger<CreateBeneficiaryContactEndpoint> _logger;

    public CreateBeneficiaryContactEndpoint(IBeneficiaryService beneficiaryService, ILogger<CreateBeneficiaryContactEndpoint> logger)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }
    public override void Configure()
    {
        Post("/contact");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary contact";
            s.ExampleRequest = CreateBeneficiaryContactRequest.SampleRequest();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryContactResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();

        // Add validation as required by project conventions
        Validator<CreateBeneficiaryContactRequestValidator>();
    }

    protected override async Task<Results<Ok<CreateBeneficiaryContactResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(CreateBeneficiaryContactRequest req, CancellationToken ct)
    {
        // Start activity for detailed tracing
        using var activity = EndpointTelemetry.ActivitySource.StartActivity("create_beneficiary_contact");
        activity?.SetTag("operation", "create_beneficiary_contact");
        activity?.SetTag("contact_ssn", req.ContactSsn.ToString()[..3] + "***"); // Mask SSN for telemetry

        try
        {
            _logger.LogInformation("Creating beneficiary contact with SSN ending {SsnLastDigits}",
                req.ContactSsn.ToString()[^4..]);

            var result = await _beneficiaryService.CreateBeneficiaryContact(req, ct);

            _logger.LogInformation("Successfully created beneficiary contact with ID {ContactId}", result.Id);
            activity?.SetTag("contact_id", result.Id.ToString());
            activity?.SetTag("success", true);

            return TypedResults.Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Contact Ssn already exists"))
        {
            _logger.LogWarning(ex, "Attempted to create beneficiary contact with duplicate SSN ending {SsnLastDigits}",
                req.ContactSsn.ToString()[^4..]);

            activity?.SetTag("success", false);
            activity?.SetTag("error_type", "duplicate_ssn");

            return TypedResults.Problem("A contact with this SSN already exists");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Contact Ssn must be 9 digits"))
        {
            _logger.LogWarning(ex, "Invalid SSN format provided for beneficiary contact creation");

            activity?.SetTag("success", false);
            activity?.SetTag("error_type", "invalid_ssn_format");

            return TypedResults.Problem("SSN must be exactly 9 digits");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating beneficiary contact");

            activity?.SetTag("success", false);
            activity?.SetTag("exception_type", ex.GetType().Name);

            return TypedResults.Problem($"An unexpected error occurred: {ex.Message}");
        }
    }
}

// Validation class following project conventions
public class CreateBeneficiaryContactRequestValidator : AbstractValidator<CreateBeneficiaryContactRequest>
{
    public CreateBeneficiaryContactRequestValidator()
    {
        RuleFor(x => x.ContactSsn)
            .InclusiveBetween(100000000, 999999999)
            .WithMessage("Contact SSN must be a 9-digit number.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth must be in the past.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-150)))
            .WithMessage("Date of birth must be within reasonable range.");

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Street address is required and must be at most 100 characters.");

        RuleFor(x => x.Street2)
            .MaximumLength(100)
            .WithMessage("Street address line 2 must be at most 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Street2));

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City is required and must be at most 50 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .Length(2)
            .WithMessage("State is required and must be 2 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Postal code is required and must be in format 12345 or 12345-6789.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("First name is required and must be at most 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Last name is required and must be at most 50 characters.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(50)
            .WithMessage("Middle name must be at most 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{3}-\d{3}-\d{4}$")
            .WithMessage("Phone number must be in format 123-456-7890.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.MobileNumber)
            .Matches(@"^\d{3}-\d{3}-\d{4}$")
            .WithMessage("Mobile number must be in format 123-456-7890.")
            .When(x => !string.IsNullOrEmpty(x.MobileNumber));

        RuleFor(x => x.EmailAddress)
            .EmailAddress()
            .MaximumLength(100)
            .WithMessage("Email address must be valid and at most 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.EmailAddress));
    }
}
