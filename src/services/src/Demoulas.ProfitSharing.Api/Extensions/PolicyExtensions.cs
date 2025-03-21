using System.Numerics;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigureSecurityPolicies(this WebApplicationBuilder builder)
    {

        _ = builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanViewYearEndReports, x => x.RequireRole(Role.ITOPERATIONS, Role.FINANCEMANAGER, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanGetPayProfitRecords, x => x.RequireRole(Role.ITOPERATIONS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanViewPayClassificationTypes, x => x.RequireRole(Role.ITOPERATIONS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanAddDemographics, x => x.RequireRole(Role.ITOPERATIONS, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanViewBalances, x => x.RequireRole(Role.ITOPERATIONS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanRunYearEndProcesses, x => x.RequireRole(Role.ITOPERATIONS, Role.ADMINISTRATOR, Role.FINANCEMANAGER));
            options.AddPolicy(Policy.CanRunMasterInquiry, x => x.RequireRole(Role.ITOPERATIONS, Role.ADMINISTRATOR, Role.FINANCEMANAGER));
            options.AddPolicy(Policy.CanFreezeDemographics, x => x.RequireRole(Role.ITOPERATIONS));
        });

        return builder;
    }
}
