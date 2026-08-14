using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "dbo");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasDefaultValueSql("NEWID()");

        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.NormalizedName).HasMaxLength(100).IsRequired();
        builder.HasIndex(r => r.NormalizedName).IsUnique();

        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.IsSystemRole).HasDefaultValue(false);

        builder.Property(r => r.RowVersion).IsRowVersion();
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(r => r.IsDeleted).HasDefaultValue(false);

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity("RolePermissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId").OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade));
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "dbo");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.NormalizedName).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.NormalizedName).IsUnique();

        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Category).HasMaxLength(100).IsRequired();
        builder.Property(p => p.IsSystemPermission).HasDefaultValue(false);

        builder.Property(p => p.RowVersion).IsRowVersion();
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(p => p.Category);
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", "dbo");
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).HasDefaultValueSql("NEWID()");

        builder.HasOne(ur => ur.Employee)
            .WithMany()
            .HasForeignKey(ur => ur.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ur => ur.ValidFrom);
        builder.Property(ur => ur.ValidTo);
        builder.Property(ur => ur.IsActive).HasDefaultValue(true);

        builder.Property(ur => ur.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(ur => new { ur.EmployeeId, ur.RoleId }).IsUnique();
        builder.HasIndex(ur => new { ur.EmployeeId, ur.IsActive });
    }
}
