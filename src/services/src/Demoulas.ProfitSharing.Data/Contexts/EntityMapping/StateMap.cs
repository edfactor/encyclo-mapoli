using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

/// <summary>
/// Entity configuration for US states and territories lookup table.
/// Defines the table structure and seeds all 50 states plus US territories.
/// </summary>
public sealed class StateMap : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        _ = builder.HasKey(s => s.Abbreviation);
        _ = builder.ToTable("STATE");

        _ = builder.Property(s => s.Abbreviation).IsRequired().HasMaxLength(2).HasColumnName("ABBREVIATION");
        _ = builder.Property(s => s.Name).IsRequired().HasMaxLength(100).HasColumnName("NAME");

        _ = builder.HasData(GetPredefinedStates());
    }

    /// <summary>
    /// Returns all 50 US states and US territories with their two-character abbreviations.
    /// </summary>
    private static List<State> GetPredefinedStates()
    {
        return new List<State>
        {
            // 50 States
            new() { Abbreviation = "AL", Name = "Alabama" },
            new() { Abbreviation = "AK", Name = "Alaska" },
            new() { Abbreviation = "AZ", Name = "Arizona" },
            new() { Abbreviation = "AR", Name = "Arkansas" },
            new() { Abbreviation = "CA", Name = "California" },
            new() { Abbreviation = "CO", Name = "Colorado" },
            new() { Abbreviation = "CT", Name = "Connecticut" },
            new() { Abbreviation = "DE", Name = "Delaware" },
            new() { Abbreviation = "FL", Name = "Florida" },
            new() { Abbreviation = "GA", Name = "Georgia" },
            new() { Abbreviation = "HI", Name = "Hawaii" },
            new() { Abbreviation = "ID", Name = "Idaho" },
            new() { Abbreviation = "IL", Name = "Illinois" },
            new() { Abbreviation = "IN", Name = "Indiana" },
            new() { Abbreviation = "IA", Name = "Iowa" },
            new() { Abbreviation = "KS", Name = "Kansas" },
            new() { Abbreviation = "KY", Name = "Kentucky" },
            new() { Abbreviation = "LA", Name = "Louisiana" },
            new() { Abbreviation = "ME", Name = "Maine" },
            new() { Abbreviation = "MD", Name = "Maryland" },
            new() { Abbreviation = "MA", Name = "Massachusetts" },
            new() { Abbreviation = "MI", Name = "Michigan" },
            new() { Abbreviation = "MN", Name = "Minnesota" },
            new() { Abbreviation = "MS", Name = "Mississippi" },
            new() { Abbreviation = "MO", Name = "Missouri" },
            new() { Abbreviation = "MT", Name = "Montana" },
            new() { Abbreviation = "NE", Name = "Nebraska" },
            new() { Abbreviation = "NV", Name = "Nevada" },
            new() { Abbreviation = "NH", Name = "New Hampshire" },
            new() { Abbreviation = "NJ", Name = "New Jersey" },
            new() { Abbreviation = "NM", Name = "New Mexico" },
            new() { Abbreviation = "NY", Name = "New York" },
            new() { Abbreviation = "NC", Name = "North Carolina" },
            new() { Abbreviation = "ND", Name = "North Dakota" },
            new() { Abbreviation = "OH", Name = "Ohio" },
            new() { Abbreviation = "OK", Name = "Oklahoma" },
            new() { Abbreviation = "OR", Name = "Oregon" },
            new() { Abbreviation = "PA", Name = "Pennsylvania" },
            new() { Abbreviation = "RI", Name = "Rhode Island" },
            new() { Abbreviation = "SC", Name = "South Carolina" },
            new() { Abbreviation = "SD", Name = "South Dakota" },
            new() { Abbreviation = "TN", Name = "Tennessee" },
            new() { Abbreviation = "TX", Name = "Texas" },
            new() { Abbreviation = "UT", Name = "Utah" },
            new() { Abbreviation = "VT", Name = "Vermont" },
            new() { Abbreviation = "VA", Name = "Virginia" },
            new() { Abbreviation = "WA", Name = "Washington" },
            new() { Abbreviation = "WV", Name = "West Virginia" },
            new() { Abbreviation = "WI", Name = "Wisconsin" },
            new() { Abbreviation = "WY", Name = "Wyoming" },

            // US Territories
            new() { Abbreviation = "DC", Name = "District of Columbia" },
            new() { Abbreviation = "AS", Name = "American Samoa" },
            new() { Abbreviation = "GU", Name = "Guam" },
            new() { Abbreviation = "MP", Name = "Northern Mariana Islands" },
            new() { Abbreviation = "PR", Name = "Puerto Rico" },
            new() { Abbreviation = "UM", Name = "United States Minor Outlying Islands" },
            new() { Abbreviation = "VI", Name = "Virgin Islands" }
        };
    }
}
