using System;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.EventSourcing.Aggregates;

public interface IAggregateRoot
{
	int Revision { get; }

	IEventRegistration RegisterApplyEvent<T>(Action<T> applyEvent) where T : class, IEvent;

	void UnregisterApplyEventRegistration(IEventRegistration eventRegistration);

	void ApplyEvent(IEvent @event);

	void ClearUncommittedEvents();

	void ClearUncommittedEventsAfterRevision(int revision);

	ReadOnlyCollection<IEvent> GetUncommittedEvents();
}
