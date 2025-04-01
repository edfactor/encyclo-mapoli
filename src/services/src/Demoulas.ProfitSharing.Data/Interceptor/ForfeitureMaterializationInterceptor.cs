using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Demoulas.ProfitSharing.Data.Interceptor;

public class ForfeitureMaterializationInterceptor : IMaterializationInterceptor
{
    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
    {
        if (entity is ProfitDetail detail)
        {
            if (detail.ProfitCodeId == /* 2 */ ProfitCode.Constants.OutgoingForfeitures
                && (detail.CommentTypeId == /* 6 */ CommentType.Constants.Forfeit ||
                    detail.CommentTypeId == /* 7 */ CommentType.Constants.UnForfeit))
            {
                return entity;
            }
            else
            {
                detail.Forfeiture = 0;
            }


            if ((detail.ProfitCodeId == /* 1 */ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal || 
                 detail.ProfitCodeId == /* 3 */ ProfitCode.Constants.OutgoingDirectPayments)
                && (detail.CommentTypeId == /* 9 */ CommentType.Constants.Voided ||
                    detail.CommentTypeId == /* 10 */ CommentType.Constants.Hardship || 
                    detail.CommentTypeId == /* 11*/ CommentType.Constants.Distribution ||
                    detail.CommentTypeId == /* 12 */ CommentType.Constants.Payoff ||
                    detail.CommentTypeId == /* 13 */ CommentType.Constants.Dirpay ||
                    detail.CommentTypeId == /* 14 */ CommentType.Constants.Rollover ||
                    detail.CommentTypeId == /* 15 */ CommentType.Constants.RothIra ))
            {
                detail.Payments = detail.Forfeiture;
                return entity;
            }
            else
            {
                detail.Payments = 0;
            }

            if ((detail.ProfitCodeId == /* 5 */ ProfitCode.Constants.OutgoingXferBeneficiary ||
                 detail.ProfitCodeId == /* 6 */ ProfitCode.Constants.IncomingQdroBeneficiary)
                && (detail.CommentTypeId == /* 1 */ CommentType.Constants.TransferIn ||
                    detail.CommentTypeId == /* 2 */ CommentType.Constants.TransferOut ||
                    detail.CommentTypeId == /* 3 */ CommentType.Constants.QdroIn ||
                    detail.CommentTypeId == /* 4 */ CommentType.Constants.QdroOut ))
            {
                detail.Allocations = detail.Forfeiture;
                return entity;
            }
            else
            {
                detail.Allocations = 0;
            }

        }

        return entity;
    }
}

