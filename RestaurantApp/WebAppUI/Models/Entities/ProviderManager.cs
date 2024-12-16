using WebAppUI.Models.CustomIdentity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.DotNet.Scaffolding.Shared;

namespace WebAppUI.Models.Entities;
[Table("ProviderManager")]
[PrimaryKey(nameof(ManagerId), nameof(ProviderId))]
public class ProviderManager
{
    [Column("UserId")]
    public int ManagerId { get; set; }
    public int ProviderId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public AppUser? Manager { get; set; }
    [ForeignKey(nameof(ProviderId))]
    public Provider? Provider { get; set; }
}