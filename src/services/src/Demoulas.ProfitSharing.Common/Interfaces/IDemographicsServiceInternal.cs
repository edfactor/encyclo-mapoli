using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation.Results;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsServiceInternal
{
    Task AddDemographicsStreamAsync(DemographicsRequest[] employees, ushort batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default);

    Task CleanAuditError(CancellationToken cancellationToken);

    Task AuditError(int badgeNumber, long oracleHcmId, IEnumerable<ValidationFailure> errorMessages, string requestedBy,
        CancellationToken cancellationToken = default,
        params object?[] args);
}
