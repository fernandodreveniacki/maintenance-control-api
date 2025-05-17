namespace MaintenanceControlSystem.Application.DTOs;

public class CreateAlertResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int MachineId { get; set; }
    public DateTime CreatedAt { get; set; }
}
