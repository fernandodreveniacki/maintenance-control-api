﻿namespace MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class RegisterUserRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}
