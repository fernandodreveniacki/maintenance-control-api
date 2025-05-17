namespace MaintenanceControlSystem.Application.DTOs;

public class CreatePlanAssignmentResponse
{
    public int Id { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string PlanTitle { get; set; } = string.Empty;
    public DateTime NextDueDate { get; set; }
    public DateTime? LastPerformed { get; set; }
}
