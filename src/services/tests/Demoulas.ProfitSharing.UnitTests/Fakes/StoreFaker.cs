using Bogus;
using Demoulas.StoreInfo.DTOs;

namespace Demoulas.ProfitSharing.IntegrationTests.Fakes;

/// <summary>
///   Faker for <c>PayrollCheckStatus</c>
/// </summary>
internal sealed class StoreFaker : Faker<StoreDetailDTO>
{
  /// <summary>
  ///   Initializes a default instance of <c>PayrollCheckStatusFaker</c>
  /// </summary>
  internal StoreFaker()
  {
    RuleFor(p => p.StoreId, f => f.IndexFaker);
    RuleFor(p => p.DisplayName, f => f.Random.AlphaNumeric(100));
  }
}
