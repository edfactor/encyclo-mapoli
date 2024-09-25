using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Base;

/// <summary>
/// Endpoints deriving from this class will automatically be able to return paginated json.
/// The developer needs to override the GetResponse member to provide a response via a DTO.
/// </summary>
/// <typeparam name="ReqType">Request type of the endpoint.  Can be EmptyRequest</typeparam>
/// <typeparam name="RespType">Response type of the endpoint.</typeparam>
public abstract class Endpoint<ReqType, RespType> : FastEndpoints.Endpoint<ReqType, ReportResponseBase<RespType>>
    where ReqType : PaginationRequestDto
    where RespType : class
{
    public override void Configure()
    {
        if (!Env.IsTestEnvironment())
        {
           // Specify caching duration and store it in metadata
           var cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }

        Description(b => 
            b.Produces<ReportResponseBase<RespType>>(200, "application/json"));
    }
    
    /// <summary>
    /// Use to provide a simple example request when no more complex than a simple Pagination Request is needed
    /// </summary>
    protected PaginationRequestDto SimpleExampleRequest => new PaginationRequestDto { Skip = 0, Take = byte.MaxValue };

    /// <summary>
    /// Asynchronously retrieves a response for the given request.
    /// </summary>
    /// <param name="req">The request object containing the necessary parameters.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response object.</returns>
    public abstract Task<ReportResponseBase<RespType>> GetResponse(ReqType req, CancellationToken ct);

    public sealed override async Task HandleAsync(ReqType req, CancellationToken ct)
    {
        var response = await GetResponse(req, ct);
        await SendOkAsync(response, ct);
    }

}
