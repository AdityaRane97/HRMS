using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents an audit log entry for compliance and tracking.
/// All significant actions in HRMS are logged for audit trail.
/// Supports GDPR, SOC2, and DPDP Act compliance.
/// Audit logs are retained for 3 years as per legal requirements.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// Identifier of the user performing the action.
    /// </summary>
    public Guid? ActorId { get; set; }
    public string? ActorName { get; set; }

    /// <summary>
    /// Type of entity being audited (e.g., "Employee", "Salary", "Leave").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// ID of the entity being audited.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Type of action performed (Create, Read, Update, Delete, Approve, Reject).
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the action.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Changes made in JSON format (only for sensitive data).
    /// Should NOT include sensitive values like salary amounts.
    /// </summary>
    public string? ChangesJson { get; set; }

    /// <summary>
    /// IP address or source of the request.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Correlation ID for tracking related actions.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Status of the action (Success, Failed, Unauthorized).
    /// </summary>
    public string Status { get; set; } = "Success"; // Success, Failed, Unauthorized

    public AuditLog()
    {
    }

    public AuditLog(
        Guid? actorId,
        string actorName,
        string entityType,
        Guid entityId,
        string actionType,
        string? description = null)
    {
        ActorId = actorId;
        ActorName = actorName;
        EntityType = entityType;
        EntityId = entityId;
        ActionType = actionType;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public static AuditLog CreateImmediateAction(
        Guid? actorId,
        string? actorName,
        string entityType,
        Guid entityId,
        string actionType,
        string? description = null,
        string? correlationId = null)
    {
        return new AuditLog(actorId, actorName ?? "System", entityType, entityId, actionType, description)
        {
            CorrelationId = correlationId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
