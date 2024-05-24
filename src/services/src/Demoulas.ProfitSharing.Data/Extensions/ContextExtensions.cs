using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Extensions;
internal static class ContextExtensions
{
    public static ModelBuilder ApplyModelConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DefinitionMap());
        modelBuilder.ApplyConfiguration(new DemographicMap());

        return modelBuilder;
    }
}
