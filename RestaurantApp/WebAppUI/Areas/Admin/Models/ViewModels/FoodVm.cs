﻿using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class FoodVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    [Price]
    public decimal Price { get; set; }
    public decimal NewPrice { get; set; }
    [Calories]
    public int Calories { get; set; }
    [Weight]
    public decimal Weight { get; set; }
    [Stock]
    public int Stock { get; set; }
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; } = false;
    //public List<ShortIngredientVm>? ListIngredients { get; set; }
}