using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppUI.Models.CustomIdentity;
using WebAppUI.Models.Entities;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Models.ViewModels;
using WebAppUI.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Humanizer.Localisation;
using System.Security.Policy;

namespace WebAppUI.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser,AppRole,int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Information> Informations { get; set; }
    public DbSet<Privacy> Privacies { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<IngredientFood> IngredientsFood { get; set; }
    public DbSet<Allergen> Allergens { get; set; }
    public DbSet<AllergenFood> AllergensFood { get; set; }
    public DbSet<Reviewer> Reviewers { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Category> Categories { get; set; }
    //public DbSet<Chef> Chefs { get; set; }
    public DbSet<FoodOffer> Offers { get; set; }
    public DbSet<ProviderManager> ProviderManagers { get; set; }
    public DbSet<ReviewerCritic> ReviewerCritics { get; set; }
    public DbSet<ShopCart> ShopCarts { get; set; }
    public DbSet<WorkHour> WorkHours { get; set; }
    public DbSet<SpecialDay> SpecialDays { get; set; }
    public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
    public DbSet<PayingMethod> PayingMethods { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderContent> OrderContents { get; set; }
    public DbSet<FeedBack> FeedBacks { get; set; }
    public DbSet<UserData> UserDatas { get; set; }
    public DbSet<WebAppUI.Models.ViewModels.CardIngredientVm> CardIngredientVm { get; set; } = default!;
    public DbSet<WebAppUI.Models.ViewModels.CardReviewerVm> CardReviewerVm { get; set; } = default!;

    //protected override void OnModelCreating(ModelBuilder modelBuilding)
    //{
    //    modelBuilding.Entity<ProviderManager>()
    //        .HasKey(pk => new { pk.ProviderId, pk.ManagerId });
    //}
}
