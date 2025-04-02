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


        // Handle Payments logic
        if (detail.ProfitCodeId == /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal ||
             detail.ProfitCodeId == /*2*/ ProfitCode.Constants.OutgoingForfeitures ||
             detail.ProfitCodeId == /*3*/ ProfitCode.Constants.OutgoingDirectPayments ||
             detail.ProfitCodeId == /*5*/ ProfitCode.Constants.OutgoingXferBeneficiary)
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
