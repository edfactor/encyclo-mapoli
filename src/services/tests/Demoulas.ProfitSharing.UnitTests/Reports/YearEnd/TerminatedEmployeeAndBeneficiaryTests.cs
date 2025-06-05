using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using FastEndpoints;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class TerminatedEmployeeAndBeneficiaryTests : ApiTestBase<Program>
{
    
    private readonly ProfitYearRequest _requestDto = new ProfitYearRequest()
    {
        ProfitYear = 2023
    };

    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Act
        var response =
            await ApiClient
                .POSTAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                    ProfitYearRequest, TerminatedEmployeeAndBeneficiaryResponse>(_requestDto);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
