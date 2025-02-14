using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Military
{
    public class MilitaryService
    {
        private readonly IProfitSharingDataContextFactory _dataContextFactory;

        public MilitaryService(IProfitSharingDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<MilitaryServiceRecord> GetMilitaryServiceRecordAsync(int employeeId)
        {
            using var context = _dataContextFactory.UseReadOnlyContext(c =>
            {
                c.ProfitDetails
                    .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
            });
        }
    }
    }
}
