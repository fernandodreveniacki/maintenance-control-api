namespace MaintenanceControlSystem.Domain.Entities;

public class MaintenancePlanAssignment
{
    public int Id { get; set; }

    public int MachineId { get; set; }
    public Machine Machine { get; set; } = default!;

    public int MaintenancePlanId { get; set; }
    public MaintenancePlan MaintenancePlan { get; set; } = default!;

    public DateTime NextDueDate { get; set; }
    public DateTime? LastPerformed { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = default!;

    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
