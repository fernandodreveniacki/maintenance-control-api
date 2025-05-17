namespace MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class CreateMaintenancePlanRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public FrequencyType FrequencyType { get; set; }

    [Required, Range(1, 365)]
    public int FrequencyValue { get; set; }

    public bool IsActive { get; set; } = true;
}
