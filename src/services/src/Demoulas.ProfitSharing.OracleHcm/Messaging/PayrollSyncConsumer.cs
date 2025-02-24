using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Services;
using MassTransit;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;
internal class PayrollSyncConsumer : IConsumer<MessageRequest<PayrollItem>>
{
    private readonly PayrollSyncService _payrollSyncService;
    
    public PayrollSyncConsumer(PayrollSyncService payrollSyncService)
    {
        _payrollSyncService = payrollSyncService;
    }

    public Task Consume(ConsumeContext<MessageRequest<PayrollItem>> context)
    {
        return _payrollSyncService.GetBalanceTypesForProcessResultsAsync(context.Message.Body, context.CancellationToken);
    }
    
}

