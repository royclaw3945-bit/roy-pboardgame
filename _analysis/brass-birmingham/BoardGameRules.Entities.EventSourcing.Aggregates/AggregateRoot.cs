using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Logging;

namespace BoardGameRules.Entities.EventSourcing.Aggregates;

public class AggregateRoot : IAggregateRoot
{
	private interface IEventRegistrationPrivate : IEventRegistration
	{
		Type GetEventType();

		void ApplyEvent(IEvent @event);
	}

	private class EventRegistration<T> : IEventRegistrationPrivate, IEventRegistration where T : class, IEvent
	{
		private readonly Type _eventType;

		private readonly Action<T> _applyEvent;

		public EventRegistration(Action<T> applyEvent)
		{
			_eventType = typeof(T);
			_applyEvent = applyEvent;
		}

		public Type GetEventType()
		{
			return _eventType;
		}

		public void ApplyEvent(IEvent @event)
		{
			if (!(@event is T obj))
			{
				throw new InvalidOperationException("Event type mismatch");
			}
			_applyEvent(obj);
		}
	}

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(AggregateRoot));

	private readonly Dictionary<Type, IEventRegistrationPrivate> _applyEventMethods = new Dictionary<Type, IEventRegistrationPrivate>();

	private readonly List<IEvent> _uncommittedEvents = new List<IEvent>();

	public virtual int Revision { get; protected set; }

	public IEventRegistration RegisterApplyEvent<T>(Action<T> applyEvent) where T : class, IEvent
	{
		EventRegistration<T> eventRegistration = new EventRegistration<T>(delegate(T e)
		{
			applyEvent(e);
		});
		_applyEventMethods.Add(typeof(T), eventRegistration);
		return eventRegistration;
	}

	public void UnregisterApplyEventRegistration(IEventRegistration eventRegistration)
	{
		if (!(eventRegistration is IEventRegistrationPrivate eventRegistrationPrivate))
		{
			throw new InvalidOperationException("Invalid eventRegistration object");
		}
		if (!_applyEventMethods.ContainsKey(eventRegistrationPrivate.GetEventType()))
		{
			throw new InvalidOperationException($"There is no event registration for event type: {eventRegistrationPrivate.GetEventType()}");
		}
		_applyEventMethods.Remove(eventRegistrationPrivate.GetEventType());
	}

	public virtual void ApplyEvent(IEvent @event)
	{
		Revision++;
		@event.Metadata.Revision = Revision;
		Logger.Verbose($"[ApplyEvent: event={@event.GetType().Name}, Revision={Revision}]");
		if (!_applyEventMethods.ContainsKey(@event.GetType()))
		{
			throw new InvalidOperationException($"There is no event handler registered for type: {@event.GetType()}");
		}
		_applyEventMethods[@event.GetType()].ApplyEvent(@event);
		_uncommittedEvents.Add(@event);
	}

	public virtual ReadOnlyCollection<IEvent> GetUncommittedEvents()
	{
		return new List<IEvent>(_uncommittedEvents).AsReadOnly();
	}

	public void ClearUncommittedEvents()
	{
		_uncommittedEvents.Clear();
	}

	public virtual void ClearUncommittedEventsAfterRevision(int revision)
	{
		int num = _uncommittedEvents.FindIndex((IEvent e) => e.Metadata.Revision == revision);
		num++;
		int count = _uncommittedEvents.Count - num;
		_uncommittedEvents.RemoveRange(num, count);
	}
}
