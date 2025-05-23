﻿using Microsoft.EntityFrameworkCore;
using MaintenanceControlSystem.Domain.Entities;

namespace MaintenanceControlSystem.Infrastructure.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<MaintenancePlan> MaintenancePlans => Set<MaintenancePlan>();
    public DbSet<MaintenancePlanAssignment> PlanAssignments => Set<MaintenancePlanAssignment>();
    public DbSet<Maintenance> Maintenances => Set<Maintenance>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        // Relacionamento Machine → CreatedByUser
        modelBuilder.Entity<Machine>()
            .HasOne(m => m.CreatedByUser)
            .WithMany()
            .HasForeignKey(m => m.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Maintenance → CreatedByUser
        modelBuilder.Entity<Maintenance>()
            .HasOne(m => m.CreatedByUser)
            .WithMany()
            .HasForeignKey(m => m.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento MaintenancePlan → CreatedByUser
        modelBuilder.Entity<MaintenancePlan>()
            .HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento MaintenancePlanAssignment → CreatedByUser
        modelBuilder.Entity<MaintenancePlanAssignment>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Alert → CreatedByUser
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Alert → Machine
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Machine)
            .WithMany()
            .HasForeignKey(a => a.MachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
