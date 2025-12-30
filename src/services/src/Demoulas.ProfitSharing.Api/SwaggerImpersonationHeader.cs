using Demoulas.ProfitSharing.Security;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Demoulas.ProfitSharing.Api;

public class SwaggerImpersonationHeader : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var hdrParameter = new OpenApiParameter
        {
            Name = Role.IMPERSONATION,
            Kind = OpenApiParameterKind.Header,
            IsRequired = false,
            Description = "A list of roles to impersonate",
            Schema = new JsonSchema
            {
                Type = JsonObjectType.String,
                Default = "",
                Enumeration = { Role.BENEFICIARY_ADMINISTRATOR, Role.FINANCEMANAGER, Role.DISTRIBUTIONSCLERK, Role.HARDSHIPADMINISTRATOR, Role.ADMINISTRATOR, Role.ITDEVOPS, Role.EXECUTIVEADMIN, Role.ITOPERATIONS, Role.AUDITOR, Role.HR_READONLY, Role.SSN_UNMASKING }
            },
        };

        context.OperationDescription.Operation.Parameters.Add(hdrParameter);

        return true;
    }
}
