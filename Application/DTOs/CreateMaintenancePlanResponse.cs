namespace MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Enums;

public class CreateMaintenancePlanResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
