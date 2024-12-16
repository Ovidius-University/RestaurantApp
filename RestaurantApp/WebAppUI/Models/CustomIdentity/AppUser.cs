using Microsoft.AspNetCore.Identity;

namespace WebAppUI.Models.CustomIdentity;

public class AppUser:IdentityUser<int>
{
    // Link to providers
    public ICollection<ProviderManager>? Providers { get; set; }

    // Link to reviewers
    public ICollection<ReviewerCritic>? Reviewers { get; set; }
}
