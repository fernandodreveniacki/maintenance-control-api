namespace MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class CreateMaintenanceRequest
{
    [Required]
    public int MachineId { get; set; }

    [Required]
    public MaintenanceType Type { get; set; }

    [Required]
    public DateTime PerformedAt { get; set; }

    [Required, Range(0.1, 100)]
    public double DurationHours { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
}
