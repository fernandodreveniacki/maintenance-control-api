using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceControlSystem.Domain.Entities;

public class Machine
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public DateTime InstallationDate { get; set; }
    public MachineStatus Status { get; set; } = MachineStatus.Active;
    public string? Manufacturer { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = default!;

    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    public ICollection<MaintenancePlanAssignment> PlanAssignments { get; set; } = new List<MaintenancePlanAssignment>();
}
