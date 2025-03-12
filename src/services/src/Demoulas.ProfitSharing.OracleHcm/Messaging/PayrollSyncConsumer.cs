using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Services;
using MassTransit;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;
internal class PayrollSyncConsumer : IConsumer<MessageRequest<PayrollItem[]>>
{
    private readonly PayrollSyncService _payrollSyncService;
    
    public PayrollSyncConsumer(PayrollSyncService payrollSyncService)
    {
        _payrollSyncService = payrollSyncService;
    }

    public async Task Consume(ConsumeContext<MessageRequest<PayrollItem[]>> context)
    {
        foreach (var item in context.Message.Body)
        {
            await _payrollSyncService.GetBalanceTypesForProcessResultsAsync(item, context.CancellationToken);
        }
    }
}

