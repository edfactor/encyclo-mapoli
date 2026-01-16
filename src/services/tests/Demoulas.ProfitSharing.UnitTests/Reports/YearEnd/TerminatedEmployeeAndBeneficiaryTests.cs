using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployees;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class TerminatedEmployeeAndBeneficiaryTests : ApiTestBase<Program>
{

    private readonly FilterableStartAndEndDateRequest _requestDto = new()
    {
        BeginningDate = new DateOnly(2023, 01, 01),
        EndingDate = new DateOnly(2023, 12, 31),
        Skip = 0,
        Take = 10,
        SortBy = "BadgeNumber",
        IsSortDescending = false
    };

    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Act
        var response =
            await ApiClient
                .POSTAsync<TerminatedEmployeesEndPoint,
                    FilterableStartAndEndDateRequest, TerminatedEmployeeAndBeneficiaryResponse>(_requestDto);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
