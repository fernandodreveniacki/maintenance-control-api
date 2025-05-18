namespace MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class CreateMachineRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime InstallationDate { get; set; }

    [Required]
    public MachineStatus Status { get; set; }

    public string? Manufacturer { get; set; }
}
