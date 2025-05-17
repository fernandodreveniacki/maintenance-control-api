using System.ComponentModel.DataAnnotations;

namespace MaintenanceControlSystem.Application.DTOs;

public class CreatePlanAssignmentRequest
{
    [Required]
    public int MachineId { get; set; }

    [Required]
    public int MaintenancePlanId { get; set; }

    [Required]
    public DateTime NextDueDate { get; set; }
}
