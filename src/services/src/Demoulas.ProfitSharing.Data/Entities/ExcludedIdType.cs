using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class ExcludedIdType : ILookupTable<byte>
{
    public required byte Id { get; set; }
    public required string Name { get; set; }

    public static class Constants
    {
        public static ExcludedIdType QPay066TAExclusions => new() { Id = 1, Name = "QPay066TA Exclusions" };
        public static ExcludedIdType QPay066IExclusions => new() { Id = 2, Name = "QPay066I Exclusions" };
    }

    // Override Equals method for correct comparison between materialized values and constants
    public override bool Equals(object? obj)
    {
        if (obj is ExcludedIdType otherExcludedIdType)
        {
            return Id == otherExcludedIdType.Id;
        }
        return false;
    }

    // Override GetHashCode to be consistent with Equals
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static implicit operator byte(ExcludedIdType excludedIdType)
    {
        return excludedIdType.Id;
    }
}
