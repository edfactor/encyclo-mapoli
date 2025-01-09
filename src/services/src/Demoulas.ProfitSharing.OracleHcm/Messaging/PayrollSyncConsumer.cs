using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.Util.Extensions;
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

