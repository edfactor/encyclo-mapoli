namespace Demoulas.ProfitSharing.UnitTests;

/// <summary>
/// Collection definitions for parallel test execution.
/// Tests in the same collection run sequentially.
/// Tests in different collections run in parallel (up to maxParallelThreads).
/// </summary>
/// 
// Collections that CAN run in parallel (independent endpoints with separate fixtures)
[CollectionDefinition("Beneficiary Tests", DisableParallelization = false)]
public class BeneficiaryTestCollection { }

[CollectionDefinition("Distribution Tests", DisableParallelization = false)]
public class DistributionTestCollection { }

[CollectionDefinition("Military Tests", DisableParallelization = false)]
public class MilitaryTestCollection { }

[CollectionDefinition("IT Operations Tests", DisableParallelization = false)]
public class ItOperationsTestCollection { }

[CollectionDefinition("Navigation Tests", DisableParallelization = false)]
public class NavigationTestCollection { }

[CollectionDefinition("Lookup Tests", DisableParallelization = false)]
public class LookupTestCollection { }

[CollectionDefinition("Profit Details Tests", DisableParallelization = false)]
public class ProfitDetailsTestCollection { }

[CollectionDefinition("Adjustments Tests", DisableParallelization = false)]
public class AdjustmentsTestCollection { }

[CollectionDefinition("Validation Tests", DisableParallelization = false)]
public class ValidationTestCollection { }

// Collections that must run sequentially (shared global state, year-end operations)
[CollectionDefinition("Fiscal Close Tests", DisableParallelization = true)]
public class FiscalCloseTestCollection { }

[CollectionDefinition("Year-End Tests", DisableParallelization = true)]
public class YearEndTestCollection { }

[CollectionDefinition("Master Inquiry Tests", DisableParallelization = true)]
public class MasterInquiryTestCollection { }

[CollectionDefinition("Role Inheritance Tests", DisableParallelization = true)]
public class RoleInheritanceTestCollection { }

[CollectionDefinition("Shared Global State", DisableParallelization = true)]
public class SharedGlobalStateTestCollection { }
