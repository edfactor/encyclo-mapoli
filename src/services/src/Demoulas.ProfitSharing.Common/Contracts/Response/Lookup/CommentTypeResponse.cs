using System.ComponentModel.DataAnnotations;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record CommentTypeResponse : ILookupTable<byte>
{
    [Key] // This key is used by the eager loading cache system to identify the unique values in an object
    public byte Id { get; set; }

    public required string Name { get; set; }

    public static CommentTypeResponse ResponseExample()
    {
        return new CommentTypeResponse
        {
            Id = 1,
            Name = "General Comment"
        };
    }
}
