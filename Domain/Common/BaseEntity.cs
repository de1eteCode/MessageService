using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

public abstract class BaseEntity {
    private readonly List<BaseEvent> _domainEvents = new();

    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public Guid UID { get; set; }

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent) {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent) {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents() {
        _domainEvents.Clear();
    }
}