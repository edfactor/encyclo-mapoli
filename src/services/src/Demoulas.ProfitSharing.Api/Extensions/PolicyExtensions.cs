using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigureSecurityPolicies(this WebApplicationBuilder builder)
    {

        _ = builder.Services.AddAuthorization(options =>
        {
            // Reporting: finance reports are read-only. Allow Finance, Admin, and Auditor.
            options.AddPolicy(Policy.CanViewYearEndReports,
                x => x.RequireRole(Role.FINANCEMANAGER, Role.ADMINISTRATOR, Role.AUDITOR));

            // Read-only finance data views for pay/profit records.
            options.AddPolicy(Policy.CanGetPayProfitRecords,
                x => x.RequireRole(Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.AUDITOR));

            // Reference data views.
            options.AddPolicy(Policy.CanViewPayClassificationTypes,
                x => x.RequireRole(Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.AUDITOR));

            // Balance views can be needed by Finance, Clerks, Admin, and HR (for participant context). Auditors are read-only.
            options.AddPolicy(Policy.CanViewBalances,
                x => x.RequireRole(Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR, Role.AUDITOR));

            // Year-end processes are highly sensitive; restrict to Finance and Admin only.
            options.AddPolicy(Policy.CanRunYearEndProcesses,
                x => x.RequireRole(Role.FINANCEMANAGER, Role.ADMINISTRATOR));

            // Master inquiry is broad read; allow Finance, Clerks, Admin; exclude IT and HR here to mirror legacy separation.
            options.AddPolicy(Policy.CanRunMasterInquiry,
                x => x.RequireRole(Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.AUDITOR));

            // Beneficiary maintenance belongs to HR and Clerks; Admin as break-glass.
            options.AddPolicy(Policy.CanMaintainBeneficiaries,
                x => x.RequireRole(Role.HARDSHIPADMINISTRATOR, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR));

            /*
             * IT roles can freeze and add demographics to support data integrity and operational needs.
             */
            options.AddPolicy(Policy.CanFreezeDemographics, x => x.RequireRole(Role.ITDEVOPS, Role.ITOPERATIONS, Role.HARDSHIPADMINISTRATOR, Role.ADMINISTRATOR));
        });

        return builder;
    }
}
