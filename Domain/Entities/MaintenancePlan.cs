using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceControlSystem.Domain.Entities;

public class MaintenancePlan
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public FrequencyType FrequencyType { get; set; }
    public int FrequencyValue { get; set; }
    public bool IsActive { get; set; } = true;

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = default!;

    public ICollection<MaintenancePlanAssignment> Assignments { get; set; } = new List<MaintenancePlanAssignment>();
}
