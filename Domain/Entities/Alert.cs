using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenanceControlSystem.Domain.Entities;

public class Alert
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("Assignment")]
    public int AssignmentId { get; set; }
    public MaintenancePlanAssignment Assignment { get; set; } = default!;

    [ForeignKey("CreatedByUser")]
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = default!;
}
