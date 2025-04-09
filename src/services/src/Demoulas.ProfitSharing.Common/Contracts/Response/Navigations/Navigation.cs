using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public record Navigation
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public required string Title { get; set; }
    public string? SubTitle { get; set; }
    public required string Url { get; set; }
    public byte? Status { get; set; }
    public byte OrderNumber { get; set; }
    public string? Icon { get; set; }
    public List<string>? RequiredRoles { get; set; }
    public List<Navigation>? Children { get; set; }
}
