using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
public sealed class TaxCodeEndpoint : ProfitSharingResponseEndpoint<List<TaxCodeResponse>>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactor;

    public TaxCodeEndpoint(IProfitSharingDataContextFactory dataContextFactory) : base(Navigation.Constants.Inquiries)
    {
        _dataContextFactor = dataContextFactory;
    }

    public override void Configure()
    {
        Get("tax-codes");
        Summary(s =>
        {
            s.Summary = "Gets all available tax codes";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<TaxCodeResponse>
                {
                    new TaxCodeResponse { Id = 'A', Name= "A - Married Filing Jointly or Qualifying Widow(er)" },
                    new TaxCodeResponse { Id = 'B', Name= "B - Single or Married Filing Separately" },
                    new TaxCodeResponse { Id = 'C', Name= "C - Head of Household" },
                    new TaxCodeResponse { Id = 'D', Name= "D - Married Filing Jointly or Qualifying Widow(er) with Two Incomes" },
                    new TaxCodeResponse { Id = 'E', Name= "E - Single or Married Filing Separately with One Income" },
                    new TaxCodeResponse { Id = 'F', Name= "F - Head of Household with One Income" },
                    new TaxCodeResponse { Id = 'G', Name= "G - Married Filing Jointly or Qualifying Widow(er) with Three or More Incomes" },
                    new TaxCodeResponse { Id = 'H', Name= "H - Single or Married Filing Separately with Two or More Incomes" },
                    new TaxCodeResponse { Id = 'I', Name= "I - Head of Household with Two or More Incomes" },
                    new TaxCodeResponse { Id = 'J', Name= "J - Married Filing Jointly or Qualifying Widow(er) with Four or More Incomes" },
                    new TaxCodeResponse { Id = 'K', Name= "K - Single or Married Filing Separately with Three or More Incomes" },
                    new TaxCodeResponse { Id = 'L', Name= "L - Head of Household with Three or More Incomes" },
                    new TaxCodeResponse { Id = 'M', Name= "M - Exempt" }
                }
            } };
        });
        Group<LookupGroup>();
        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(5);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override Task<List<TaxCodeResponse>> ExecuteAsync(CancellationToken ct)
    {
        return _dataContextFactor.UseReadOnlyContext(c => c.TaxCodes.Select(x => new TaxCodeResponse { Id = x.Id, Name = x.Name }).ToListAsync(ct));
    }
}
