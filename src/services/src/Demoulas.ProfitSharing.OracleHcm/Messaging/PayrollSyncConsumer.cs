using System.Threading.Channels;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;

internal class PayrollSyncChannelConsumer : BackgroundService
{
    private readonly ChannelReader<MessageRequest<PayrollItem[]>> _reader;
    private readonly PayrollSyncService _payrollSyncService;

    public PayrollSyncChannelConsumer(
        Channel<MessageRequest<PayrollItem[]>> channel,
        PayrollSyncService payrollSyncService)
    {
        _reader = channel.Reader;
        _payrollSyncService = payrollSyncService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            await ProcessMessage(message, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessMessage(MessageRequest<PayrollItem[]> message, CancellationToken cancellationToken)
    {
        foreach (var item in message.Body)
        {
            await _payrollSyncService.GetBalanceTypesForProcessResultsAsync(item, cancellationToken).ConfigureAwait(false);
        }
    }
}
