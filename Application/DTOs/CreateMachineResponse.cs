namespace MaintenanceControlSystem.Application.DTOs;

public class CreateMachineResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime InstallationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
}
