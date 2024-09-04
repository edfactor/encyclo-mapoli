using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Okta.AspNetCore;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigurePolicies(this WebApplicationBuilder builder)
    {

        _ = builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanViewYearEndReports, x => x.RequireRole(Role.FINANCEMANAGER, Role.ADMINISTRATOR));
            options.AddPolicy(Policy.CanGetPayProfitRecords, x => x.RequireRole(Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanViewPayClassificationTypes, x => x.RequireRole(Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.ADMINISTRATOR, Role.HARDSHIPADMINISTRATOR));
            options.AddPolicy(Policy.CanAddDemographics, x => x.RequireRole(Role.ADMINISTRATOR));

        });

        return builder;
    }
}
