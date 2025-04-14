using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public sealed class Navigation
{
    [Key]
    public int Id { get; set; }
    public int? ParentId { get; set; }
    [Required]
    public required string Title { get; set; }
    public string? SubTitle { get; set; }
    [Required]
    public required string Url { get; set; }
    public int? StatusId { get; set; }
    public int NavigationRoleId { get; set; }
    public byte OrderNumber { get; set; }
    public string? Icon { get; set; }


    public NavigationStatus? NavigationStatus { get; set; }
    public NavigationRole? NavigationRole { get; set; }
    public List<Navigation>? Children { get; set; }
    public Navigation? Parent { get; set; }

    public List<NavigationTracking>? NavigationTrackings { get; set; }
}
