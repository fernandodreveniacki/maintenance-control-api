using System.ComponentModel.DataAnnotations;

namespace MaintenanceControlSystem.Application.DTOs;

public class CreateAlertRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public int MachineId { get; set; }
}
