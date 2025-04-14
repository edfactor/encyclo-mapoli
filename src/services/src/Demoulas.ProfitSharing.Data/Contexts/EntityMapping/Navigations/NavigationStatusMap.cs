using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
internal sealed class NavigationStatusMap : IEntityTypeConfiguration<NavigationStatus>
{
    public void Configure(EntityTypeBuilder<NavigationStatus> builder)
    {
        builder.HasData(new List<NavigationStatus>() 
        { 
            new NavigationStatus() {Id =1, Name = "Not Started"},
            new NavigationStatus() {Id =2, Name = "In Progress"},
            new NavigationStatus() {Id =3, Name = "Blocked"},
            new NavigationStatus() {Id =4, Name = "Successful"}
        }
        );
    }
}
