using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Demoulas.ProfitSharing.Data.Interceptor;

public class ForfeitureMaterializationInterceptor : IMaterializationInterceptor
{
    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
    {
        if (entity is not ProfitDetail detail)
        {
            return entity;
        }

        if (detail.Forfeiture == 0)
        {
            return entity;
        }

        // Handle Forfeiture logic
        if (detail.ProfitCodeId == /*2*/ ProfitCode.Constants.OutgoingForfeitures &&
            (detail.CommentTypeId == /*6*/ CommentType.Constants.Forfeit ||
             detail.CommentTypeId == /*7*/ CommentType.Constants.UnForfeit))
        {
            return entity;
        }
        detail.Forfeiture = 0;

        // Handle Payments logic
        if ((detail.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal ||
             detail.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments) &&
            (detail.CommentTypeId == CommentType.Constants.Voided ||
             detail.CommentTypeId == CommentType.Constants.Hardship ||
             detail.CommentTypeId == CommentType.Constants.Distribution ||
             detail.CommentTypeId == CommentType.Constants.Payoff ||
             detail.CommentTypeId == CommentType.Constants.Dirpay ||
             detail.CommentTypeId == CommentType.Constants.Rollover ||
             detail.CommentTypeId == CommentType.Constants.RothIra))
        {
            detail.Payments = detail.Forfeiture;
            return entity;
        }
        detail.Payments = 0;

        // Handle Allocations logic
        if ((detail.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary ||
             detail.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary) &&
            (detail.CommentTypeId == CommentType.Constants.TransferIn ||
             detail.CommentTypeId == CommentType.Constants.TransferOut ||
             detail.CommentTypeId == CommentType.Constants.QdroIn ||
             detail.CommentTypeId == CommentType.Constants.QdroOut))
        {
            detail.Allocations = detail.Forfeiture;
            return entity;
        }
        detail.Allocations = 0;

        return entity;
    }
}
