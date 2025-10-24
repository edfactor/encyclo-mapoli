using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class MasterInquiryRequestValidator : AbstractValidator<MasterInquiryRequest>
{
    public MasterInquiryRequestValidator()
    {
        // Minimum allowed profit year from reference data
        short minYear = (short)ReferenceData.DsmMinValue.Year;
        short currentYear = (short)DateTime.UtcNow.Year;

        // Require at least one search discriminator: a profit year, an end profit year, SSN or BadgeNumber
        RuleFor(x => x)
            .Must(x => (x.ProfitYear > minYear) || (x.EndProfitYear > minYear) || x.Ssn != 0 || (x.BadgeNumber != 0))
            .WithMessage($"Either ProfitYear or EndProfitYear (greater than {minYear}), or Ssn/BadgeNumber must be provided.");

        // Require at least two filterable values (MemberType/PaymentType with value 0 do not count)
        RuleFor(x => x).Custom((x, ctx) =>
        {
            var count = 0;

            if (x.BadgeNumber is > 0)
            {
                count++;
            }

            if (x.PsnSuffix > 0)
            {
                count++;
            }

            if (x.ProfitYear > 0)
            {
                count++;
            }

            if (x.EndProfitYear > 0)
            {
                count++;
            }



         

            if (x.ProfitCode.HasValue)
            {
                count++;
            }

            if (x.ContributionAmount.HasValue)
            {
                count++;
            }

            if (x.EarningsAmount.HasValue)
            {
                count++;
            }

            if (x.Ssn != 0)
            {
                count++;
            }

            if (x.ForfeitureAmount.HasValue)
            {
                count++;
            }

            if (x.PaymentAmount.HasValue)
            {
                count++;
            }

            if (!string.IsNullOrWhiteSpace(x.Name))
            {
                count++;
            }

            // PaymentType and MemberType count only if present and non-zero
            if (x.PaymentType.HasValue && x.PaymentType.Value != 0)
            {
                count++;
            }

            if (x.MemberType != 0)
            {
                count++;
            }

            if (count < 2)
            {
                var providedList = new List<string>();

                if (x.ProfitYear > 0 || x.EndProfitYear > 0)
                {
                    providedList.Add("ProfitYear");
                }

                if (x.BadgeNumber is > 0)
                {
                    providedList.Add("BadgeNumber");
                }

                if (x.PsnSuffix > 0)
                {
                    providedList.Add("PsnSuffix");
                }

                if (x.ProfitCode.HasValue)
                {
                    providedList.Add("ProfitCode");
                }

                if (x.ContributionAmount.HasValue)
                {
                    providedList.Add("ContributionAmount");
                }

                if (x.EarningsAmount.HasValue)
                {
                    providedList.Add("EarningsAmount");
                }

                if (x.Ssn != 0)
                {
                    providedList.Add("Ssn");
                }

                if (x.ForfeitureAmount.HasValue)
                {
                    providedList.Add("ForfeitureAmount");
                }

                if (x.PaymentAmount.HasValue)
                {
                    providedList.Add("PaymentAmount");
                }

                if (!string.IsNullOrWhiteSpace(x.Name))
                {
                    providedList.Add("Name");
                }

                if (x.PaymentType.HasValue && x.PaymentType.Value != 0)
                {
                    providedList.Add("PaymentType");
                }

                if (x.MemberType != 0)
                {
                    providedList.Add("MemberType");
                }

                var providedText = providedList.Count > 0 ? string.Join(", ", providedList) : "(none)";
                var missing = 2 - providedList.Count;
                var allCandidates = new[] { "BadgeNumber", "PsnSuffix", "StartProfitMonth", "ProfitYear", "EndProfitMonth", "ProfitCode", "ContributionAmount", "EarningsAmount", "Ssn", "ForfeitureAmount", "PaymentAmount", "Name", "PaymentType (non-zero)", "MemberType (non-zero)" };
                var suggestion = string.Join(", ", allCandidates.Where(a => !providedList.Contains(a.Replace(" (non-zero)", ""))));

                ctx.AddFailure($"At least two filterable values must be provided; provided ({providedList.Count}): {providedText}. Provide {missing} more from: {suggestion}.");
            }
        });

        // ProfitYear bounds (if provided / non-zero)
        When(x => x.ProfitYear > 0, () =>
        {
            RuleFor(x => x.ProfitYear)
                .GreaterThan(minYear).WithMessage($"ProfitYear must be greater than {minYear}.")
                .LessThanOrEqualTo(currentYear).WithMessage("ProfitYear cannot be in the future.");
        });

        // EndProfitYear bounds (if provided / non-zero)
        When(x => x.EndProfitYear > 0, () =>
        {
            RuleFor(x => x.EndProfitYear)
                .GreaterThan(minYear).WithMessage($"EndProfitYear must be greater than {minYear}.")
                .LessThanOrEqualTo(currentYear).WithMessage("EndProfitYear cannot be in the future.");
        });

        // If both years provided, ensure ordering ProfitYear <= EndProfitYear
        RuleFor(x => x)
            .Custom((x, ctx) =>
            {
                if (x.ProfitYear > 0 && x.EndProfitYear > 0 && x.ProfitYear > x.EndProfitYear)
                {
                    ctx.AddFailure("EndProfitYear", "EndProfitYear must be the same as or greater than ProfitYear.");
                }
            });

        // Miscellaneous checks (months, amounts, types, SSN/badge) done in a single Custom validator
        RuleFor(x => x).Custom((x, ctx) =>
        {
            // Amounts non-negative
            if (x.ContributionAmount is < 0)
            {
                ctx.AddFailure("ContributionAmount", "ContributionAmount cannot be negative.");
            }

            if (x.EarningsAmount is < 0)
            {
                ctx.AddFailure("EarningsAmount", "EarningsAmount cannot be negative.");
            }

            if (x.ForfeitureAmount is < 0)
            {
                ctx.AddFailure("ForfeitureAmount", "ForfeitureAmount cannot be negative.");
            }

            if (x.PaymentAmount is < 0)
            {
                ctx.AddFailure("PaymentAmount", "PaymentAmount cannot be negative.");
            }

            // MemberType domain check (if provided)
            if (x.MemberType != 0 && (x.MemberType < 0 || x.MemberType > 3))
            {
                ctx.AddFailure("MemberType", "MemberType is invalid.");
            }

            // PaymentType domain check (if provided)
            if (x.PaymentType.HasValue && (x.PaymentType < 0 || x.PaymentType.Value > 3))
            {
                ctx.AddFailure("PaymentType", "PaymentType is invalid.");
            }

            // SSN / Badge basic sanity
            // SSN must be positive and 9 digits or less (frontend enforces 9-digit max)
            if (x.Ssn < 0)
            {
                ctx.AddFailure("Ssn", "Ssn must be a positive integer.");
            }

            if (x.Ssn > 999_999_999)
            {
                ctx.AddFailure("Ssn", "Ssn must be 9 digits or less.");
            }

            // Badge number must be positive and up to 11 digits (frontend cap)
            if (x.BadgeNumber < 0)
            {
                ctx.AddFailure("BadgeNumber", "BadgeNumber must be a positive integer.");
            }

            if (x.BadgeNumber > 999_999_999)
            {
                ctx.AddFailure("BadgeNumber", "Badge number must be 9 digits or less.");
            }

            // EndProfitYear frontend limits (2020..2100)
            if (x.EndProfitYear.HasValue)
            {
                if (x.EndProfitYear.Value < 2020)
                {
                    ctx.AddFailure("EndProfitYear", "Year must be 2020 or later.");
                }
                if (x.EndProfitYear.Value > 2100)
                {
                    ctx.AddFailure("EndProfitYear", "Year must be 2100 or earlier.");
                }
            }
        });
    }
}
