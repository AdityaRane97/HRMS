# ADR-003: Entity Framework Core and Database Strategy

## Status
ACCEPTED

## Context
The HRMS enterprise application requires:
- Persistent storage for employee, organizational, and audit data
- Soft-delete support for compliance (no hard deletes, audit trail)
- Optimistic concurrency control to prevent lost updates
- Efficient querying with support for complex hierarchical data (manager relationships)
- Migration management for multiple database environments
- GDPR/SOC2 compliance with audit trail and data retention policies

## Decision
Use **Entity Framework Core 8+ (SQL Server as primary provider)**:

1. **ORM Strategy**:
   - EF Core with code-first migrations
   - SQL Server as the primary database provider
   - Entity configurations in separate `Config/` classes for separation of concerns

2. **Audit & Soft-Delete**:
   - All entities inherit from `BaseEntity` with audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `IsDeleted`, `DeletedAt`, `DeletedBy`
   - Global query filter in `OnModelCreating()` to exclude soft-deleted records by default
   - `AuditLog` entity for immutable audit trail (3-year retention)

3. **Concurrency Control**:
   - `RowVersion` field (byte array) for optimistic concurrency
   - Prevents lost updates in concurrent scenarios
   - Exceptions mapped to `ConcurrencyException` for domain logic handling

4. **Aggregate Design**:
   - `Employee` as the core aggregate root
   - Self-referential manager relationship for reporting hierarchy
   - `DirectReports` navigation collection for efficient traversal
   - Domain events in-memory (marked `[NotMapped]`) for eventual consistency

5. **Migration Strategy**:
   - Initial migration: `InitialCreate` with all core entities
   - Migrations committed to version control
   - Script-based migration approach for schema version tracking
   - Migrations assembly: `HRMS.Infrastructure`

## Rationale
- **Industry Standard**: EF Core is the .NET ecosystem standard with rich LINQ support
- **Type Safety**: Compile-time checks prevent many SQL errors
- **Audit Compliance**: Global soft-delete and audit logging meet GDPR/SOC2 requirements
- **Concurrency Safety**: Row version prevents lost updates in high-contention scenarios
- **Version Control**: Migrations in Git enable rollback and environment parity

## Consequences
- N+1 queries possible if not careful with LINQ (mitigated by async/IQueryable discipline)
- Soft-delete performance penalty on large queries (handled with covering indexes)
- Migration complexity increases with schema changes (mitigated by early design)
- Row version adds storage overhead (~8 bytes per entity)

## Implementation Notes
- All DbSets configured in `HrmsDbContext.OnModelCreating()`
- Configurations inherit from `IEntityTypeConfiguration<T>` for modularity
- SaveChangesAsync hooks for audit timestamp updates
- API layer uses `AsNoTracking()` queries for read operations to reduce memory
- Manager self-reference requires careful cascade delete configuration

## Future Considerations
- Sharding strategy if employee count exceeds 10M
- Read replicas for reporting queries (phase 3+)
- Temporal tables for fine-grained history (SQL Server 2016+)
- Event sourcing integration for immutable event store (optional phase 4+)
