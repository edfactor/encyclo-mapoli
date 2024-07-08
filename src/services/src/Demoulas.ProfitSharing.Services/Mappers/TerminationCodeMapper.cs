using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace Demoulas.ProfitSharing.Services.Mappers;

[Mapper]
public partial class TerminationCodeMapper
{
    public partial TerminationCodeResponseDto? Map(TerminationCode? source);

}
