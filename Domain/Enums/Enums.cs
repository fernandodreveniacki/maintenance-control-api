
namespace MaintenanceControlSystem.Domain.Enums;


public enum MaintenanceType
{
    Preventiva = 0,
    Corretiva = 1,
    Preditiva = 2
}

public enum FrequencyType
{
    Days,
    Weeks,
    OperatingHours
}

public enum MachineStatus
{
    Active = 1,
    Inactive = 2,
    Maintenance = 3
}

public enum UserRole
{
    Admin = 0,
    Manager = 1,
    Technician = 2
}