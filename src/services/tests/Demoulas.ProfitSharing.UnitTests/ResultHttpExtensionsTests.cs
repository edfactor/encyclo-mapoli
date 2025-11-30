using Demoulas.ProfitSharing.Common.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests;

public class ResultHttpExtensionsTests
{
    private sealed record DummyDto(string Value);

    [Fact]
    public void SuccessResult_MapsToOk()
    {
        var dto = new DummyDto("abc");
        var result = Result<DummyDto>.Success(dto);
        var http = result.ToHttpResult();
        http.Result.ShouldBeOfType<Ok<DummyDto>>().Value.ShouldBe(dto);
    }

    [Fact]
    public void NotFoundError_MapsToNotFound()
    {
        var notFound = Error.EntityNotFound("Dummy");
        var failure = Result<DummyDto>.Failure(notFound);
        var http = failure.ToHttpResult(notFound);
        http.Result.ShouldBeOfType<NotFound>();
    }

    [Fact]
    public void OtherFailure_MapsToProblem()
    {
        var failure = Result<DummyDto>.Failure(Error.Unexpected("boom"));
        var http = failure.ToHttpResult(Error.EntityNotFound("X"));
        http.Result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    public void ValidationFailure_MapsToProblem()
    {
        var validation = Result<DummyDto>.ValidationFailure(new System.Collections.Generic.Dictionary<string, string[]>
        {
            { "FieldA", new [] { "FieldA is required" } }
        });
        var http = validation.ToHttpResult();
        http.Result.ShouldBeOfType<ProblemHttpResult>();
    }
}
