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

            detail.Forfeiture = 0;
        }

        return entity;
    }
}

