namespace MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class UpdateUserRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}
