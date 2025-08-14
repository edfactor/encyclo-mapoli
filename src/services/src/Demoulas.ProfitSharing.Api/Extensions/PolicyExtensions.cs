using System.Numerics;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigureSecurityPolicies(this WebApplicationBuilder builder)
    {

        _ = builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanViewYearEndReports, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanGetPayProfitRecords, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanViewPayClassificationTypes, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanAddDemographics, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanViewBalances, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanRunYearEndProcesses, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR, Role.FINANCEMANAGER));
            options.AddPolicy(Policy.CanRunMasterInquiry, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.HARDSHIPADMINISTRATOR, Role.DISTRIBUTIONSCLERK));
            options.AddPolicy(Policy.CanMaintainBeneficiaries, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK));
            options.AddPolicy(Policy.CanFreezeDemographics, x => x.RequireRole(Role.ITDEVOPS));
        });

        return builder;
    }
}
