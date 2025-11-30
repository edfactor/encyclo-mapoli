using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class ProfitCodeFaker : Faker<ProfitCode>
{
    internal ProfitCodeFaker()
    {
        RuleFor(pc => pc.Id, f => f.PickRandom(ProfitCode.Constants.IncomingContributions.Id,
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                ProfitCode.Constants.OutgoingForfeitures.Id,
                ProfitCode.Constants.OutgoingDirectPayments.Id,
                ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                ProfitCode.Constants.IncomingQdroBeneficiary.Id,
                ProfitCode.Constants.Incoming100PercentVestedEarnings.Id,
                ProfitCode.Constants.Outgoing100PercentVestedPayment.Id))
            .RuleFor(pc => pc.Name, f => f.Name.JobTitle())
            .RuleFor(pc => pc.Frequency, f => f.Lorem.Word())
            .UseSeed(100);
    }
}
