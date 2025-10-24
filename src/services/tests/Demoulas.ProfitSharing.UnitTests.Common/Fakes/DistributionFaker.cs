using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class DistributionFaker : Faker<Distribution>
{
    private readonly IList<DistributionFrequency>? _distributionFrequencies;
    private readonly IList<DistributionStatus>? _distributionStatuses;
    private readonly IList<TaxCode>? _taxCodes;
    private readonly IList<DistributionPayee>? _distributionPayees;

    internal DistributionFaker(
        IList<DistributionFrequency>? distributionFrequencies = null,
        IList<DistributionStatus>? distributionStatuses = null,
        IList<TaxCode>? taxCodes = null,
        IList<DistributionPayee>? distributionPayees = null)
    {
        _distributionFrequencies = distributionFrequencies;
        _distributionStatuses = distributionStatuses;
        _taxCodes = taxCodes;
        _distributionPayees = distributionPayees;

        RuleFor(d => d.Id, f => f.IndexFaker + 1)
            .RuleFor(d => d.Ssn, f => f.Person.Ssn().ConvertSsnToInt())
            .RuleFor(d => d.PaymentSequence, f => (byte)(f.IndexFaker % 10 + 1))
            .RuleFor(d => d.EmployeeName, f => f.Person.FullName)
            .RuleFor(d => d.FrequencyId, f => f.PickRandom(
                DistributionFrequency.Constants.Monthly,
                DistributionFrequency.Constants.Quarterly,
                DistributionFrequency.Constants.Annually,
                DistributionFrequency.Constants.Hardship,
                DistributionFrequency.Constants.PayDirect,
                DistributionFrequency.Constants.RolloverDirect))
            .RuleFor(d => d.StatusId, f => f.PickRandom('P', 'C', 'H', 'X', 'D', 'Y', 'O'))
            .RuleFor(d => d.GrossAmount, f => Math.Round(f.Finance.Amount(100, 10_000), 2))
            .RuleFor(d => d.FederalTaxPercentage, f => Math.Round(f.Random.Decimal(0, 30), 2))
            .RuleFor(d => d.StateTaxPercentage, f => Math.Round(f.Random.Decimal(0, 10), 2))
            .RuleFor(d => d.FederalTaxAmount, (f, d) => Math.Round(d.GrossAmount * d.FederalTaxPercentage / 100, 2))
            .RuleFor(d => d.StateTaxAmount, (f, d) => Math.Round(d.GrossAmount * d.StateTaxPercentage / 100, 2))
            .RuleFor(d => d.TaxCodeId, f => f.PickRandom('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'P', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'))
            .RuleFor(d => d.Tax1099ForEmployee, f => f.Random.Bool(0.95f))
            .RuleFor(d => d.Tax1099ForBeneficiary, f => f.Random.Bool(0.05f))
            .RuleFor(d => d.IsDeceased, f => f.Random.Bool(0.02f))
            .RuleFor(d => d.GenderId, f => f.PickRandom<char?>('M', 'F', 'X', 'U', null))
            .RuleFor(d => d.QualifiedDomesticRelationsOrder, f => f.Random.Bool(0.01f))
            .RuleFor(d => d.RothIra, f => f.Random.Bool(0.1f))
            .RuleFor(d => d.CreatedAtUtc, f => f.Date.Past(2, DateTime.UtcNow))
            .RuleFor(d => d.ModifiedAtUtc, (f, d) => d.CreatedAtUtc.AddMinutes(f.Random.Int(0, 10080)))
            .RuleFor(d => d.UserName, f => f.Internet.UserName())
            .FinishWith((_, d) => SetupNavigationProperties(d));
    }

    private void SetupNavigationProperties(Distribution distribution)
    {
        // Set Frequency navigation property
        if (_distributionFrequencies != null)
        {
            distribution.Frequency = _distributionFrequencies.FirstOrDefault(freq => freq.Id == distribution.FrequencyId);
        }

        // Set Status navigation property  
        if (_distributionStatuses != null)
        {
            distribution.Status = _distributionStatuses.FirstOrDefault(status => status.Id == distribution.StatusId);
        }

        // Set TaxCode navigation property
        if (_taxCodes != null)
        {
            distribution.TaxCode = _taxCodes.FirstOrDefault(tc => tc.Id == distribution.TaxCodeId);
        }

        // Set Payee navigation property (alternate between payees)
        if (_distributionPayees != null && _distributionPayees.Count > 0)
        {
            distribution.PayeeId = (int?)(distribution.Id % _distributionPayees.Count) + 1;
            distribution.Payee = _distributionPayees.FirstOrDefault(p => p.Id == distribution.PayeeId);
        }

        // Set some distributions to specific statuses for testing
        if (_distributionStatuses != null)
        {
            if (distribution.Id <= 50)
            {
                distribution.StatusId = DistributionStatus.Constants.RequestOnHold;
                distribution.Status = _distributionStatuses.FirstOrDefault(s => s.Id == DistributionStatus.Constants.RequestOnHold);
            }
            else if (distribution.Id <= 100)
            {
                distribution.StatusId = DistributionStatus.Constants.ManualCheck;
                distribution.Status = _distributionStatuses.FirstOrDefault(s => s.Id == DistributionStatus.Constants.ManualCheck);
                distribution.ManualCheckNumber = $"MC-{1000 + distribution.Id}";
            }
        }
    }
}
