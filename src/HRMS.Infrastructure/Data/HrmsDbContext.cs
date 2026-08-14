using Microsoft.EntityFrameworkCore;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Data.Config;

namespace HRMS.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for HRMS.
/// Manages all entities, migrations, and database operations.
/// Thread-safe and supports async operations.
/// </summary>
public class HrmsDbContext : DbContext
{
    public HrmsDbContext(DbContextOptions<HrmsDbContext> options) : base(options)
    {
    }

    // DbSets for core entities
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        // Global query filter to exclude soft-deleted entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Domain.Common.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "p");
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(
                        System.Linq.Expressions.Expression.Property(parameter, "IsDeleted"),
                        System.Linq.Expressions.Expression.Constant(false)),
                    parameter);
                entityType.SetQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// Override SaveChangesAsync to track audit information.
    /// This will be enhanced to automatically capture audit trails.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Update CreatedAt and UpdatedAt for audit trail.
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
