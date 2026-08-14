namespace HRMS.Domain.Common;

/// <summary>
/// Represents an aggregate root - the entry point to an aggregate.
/// Aggregates maintain internal consistency and handle domain events.
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = [];

    /// <summary>
    /// Collection of domain events raised by this aggregate.
    /// These are used for event sourcing and notifications.
    /// </summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Add a domain event to be published.
    /// Should only be called during aggregate creation or modification.
    /// </summary>
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear all domain events after they have been published.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Base class for domain events.
/// All significant domain actions should raise events.
/// Note: DomainEvent is not a DbContext entity - marked as NotMapped.
/// </summary>
[System.ComponentModel.DataAnnotations.Schema.NotMapped]
public abstract class DomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
}
