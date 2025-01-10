using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public interface IPayProfitUpdateService
{
    Task SetZeroContributionReason(IQueryable<PayProfit> records, byte zeroContributionReasonId, CancellationToken cancellationToken);
}

public sealed class PayProfitUpdateService : IPayProfitUpdateService
{
    public Task SetZeroContributionReason(IQueryable<PayProfit> records, byte zeroContributionReasonId, CancellationToken cancellationToken)
    {
        return records.ExecuteUpdateAsync(x => x.SetProperty(pp => pp.ZeroContributionReasonId, zeroContributionReasonId), cancellationToken);
    }
}
