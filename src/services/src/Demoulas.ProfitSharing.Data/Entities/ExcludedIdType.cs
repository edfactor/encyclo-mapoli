using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class ExcludedIdType : ILookupTable<byte>, IEquatable<ExcludedIdType>
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

    public bool Equals(ExcludedIdType? other)
    {
        if (other != null)
        {
            return other.Id == this.Id;
        }
        return false;
    }

    public static implicit operator byte(ExcludedIdType excludedIdType)
    {
        return excludedIdType.Id;
    }

    public static bool operator ==(ExcludedIdType? left, ExcludedIdType? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (ReferenceEquals(left, null))
        {
            return false;
        }
        if (ReferenceEquals(right, null))
        {
            return false;
        }
        return left.Equals(right);
    }

    public static bool operator !=(ExcludedIdType? left, ExcludedIdType? right)
    {
        return !(left == right);
    }

    // Override GetHashCode to be consistent with Equals
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
