﻿using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Critic.Models.DTOs;
public class FoodDto
{
    public int FoodId { get; set; }
    public required string Title { get; set; }
}
