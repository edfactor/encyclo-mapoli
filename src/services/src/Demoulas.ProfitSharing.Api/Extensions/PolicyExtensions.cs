using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigureSecurityPolicies(this WebApplicationBuilder builder)
    {

        _ = builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanViewYearEndReports, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanGetPayProfitRecords, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanViewPayClassificationTypes, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanViewBalances, x => x.RequireRole(Role.ITDEVOPS, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanRunYearEndProcesses, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR, Role.FINANCEMANAGER));
            options.AddPolicy(Policy.CanRunMasterInquiry, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanMaintainBeneficiaries, x => x.RequireRole(Role.ITDEVOPS, Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.HARDSHIPADMINISTRATOR));

            /*
             * IT roles can freeze and add demographics to support data integrity and operational needs.
             */
            options.AddPolicy(Policy.CanFreezeDemographics, x => x.RequireRole(Role.ITDEVOPS, Role.ITOPERATIONS));
            options.AddPolicy(Policy.CanAddDemographics, x => x.RequireRole(Role.ITDEVOPS, Role.ITOPERATIONS));
        });

        return builder;
    }
}
