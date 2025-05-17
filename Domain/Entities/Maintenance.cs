using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceControlSystem.Domain.Entities;

public class Maintenance
{
    public int Id { get; set; }

    public int MachineId { get; set; }
    public Machine Machine { get; set; } = default!;

    public MaintenanceType Type { get; set; }
    public DateTime PerformedAt { get; set; }
    public float DurationHours { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? RootCause { get; set; }

    [MaxLength(500)]
    public string? CorrectiveAction { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = default!;
}
