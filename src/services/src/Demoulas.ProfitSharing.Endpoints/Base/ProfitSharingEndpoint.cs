using Demoulas.Common.Api.Endpoints;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Base;

/// <summary>
/// Common contract exposing a NavigationId for endpoints.
/// </summary>
public interface IHasNavigationId
{
    short NavigationId { get; }
}

/// <summary>
/// Base endpoint for scenarios with both request and response.
/// Adds a required NavigationId for cross-cutting needs like authorization, telemetry, or routing.
/// </summary>
/// <typeparam name="TRequest">The request DTO type.</typeparam>
/// <typeparam name="TResponse">The response DTO type.</typeparam>
public abstract class ProfitSharingEndpoint<TRequest, TResponse> : DemoulasEndpoint<TRequest, TResponse>, IHasNavigationId
    where TRequest : notnull
    where TResponse : notnull
{
    protected ProfitSharingEndpoint(short navigationId)
    {
        NavigationId = navigationId;
    }

    /// <summary>
    /// A short identifier representing the navigation/menu item for this endpoint.
    /// </summary>
    public short NavigationId { get; }
}

/// <summary>
/// Base endpoint for scenarios with a request and no response payload.
/// </summary>
/// <typeparam name="TRequest">The request DTO type.</typeparam>
public abstract class ProfitSharingRequestEndpoint<TRequest> : DemoulasEndpoint<TRequest>, IHasNavigationId
    where TRequest : notnull
{
    protected ProfitSharingRequestEndpoint(short navigationId)
    {
        NavigationId = navigationId;
    }

    public short NavigationId { get; }
}

/// <summary>
/// Base endpoint for scenarios without a request but with a response payload.
/// </summary>
/// <typeparam name="TResponse">The response DTO type.</typeparam>
public abstract class ProfitSharingResponseEndpoint<TResponse> : DemoulasEndpointWithoutRequest<TResponse>, IHasNavigationId
    where TResponse : notnull
{
    protected ProfitSharingResponseEndpoint(short navigationId)
    {
        NavigationId = navigationId;
    }

    public short NavigationId { get; }
}

/// <summary>
/// Result-enabled variant for read-only lookup/list endpoints without a request body.
/// Provides a consistent HTTP union result shape using domain Result{T} mapping helpers.
/// </summary>
/// <typeparam name="TResponse">Concrete response DTO or collection DTO.</typeparam>
public abstract class ProfitSharingResultResponseEndpoint<TResponse> : DemoulasEndpointWithoutRequestResult<TResponse>, IHasNavigationId
    where TResponse : notnull
{
    protected ProfitSharingResultResponseEndpoint(short navigationId)
    {
        NavigationId = navigationId;
    }

    public short NavigationId { get; }
}

/// <summary>
/// Base endpoint for scenarios without a request and without a response payload.
/// </summary>
public abstract class ProfitSharingEndpoint : DemoulasEndpointWithoutRequest, IHasNavigationId
{
    protected ProfitSharingEndpoint(short navigationId)
    {
        NavigationId = navigationId;
    }

    public short NavigationId { get; }
}
