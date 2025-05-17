namespace MaintenanceControlSystem.Application.DTOs;

public class CreateMaintenanceResponse
{
    public int Id { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public double DurationHours { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
}
