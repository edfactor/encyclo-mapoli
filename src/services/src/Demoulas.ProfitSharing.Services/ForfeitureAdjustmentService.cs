using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using System.Threading;
using System.Threading.Tasks;
using System;
using Demoulas.Common.Contracts.Contracts.Response;
using System.Collections.Generic;

namespace Demoulas.ProfitSharing.Services;
public class ForfeitureAdjustmentService : IForfeitureAdjustmentService
{


    public Task<ForfeitureAdjustmentReportResponse> GetForfeitureAdjustmentReportAsync(ForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default)
    {
       throw new NotImplementedException();
    }

    public Task<ForfeitureAdjustmentReportDetail> UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default)
    {
       throw new NotImplementedException();
    }
}
