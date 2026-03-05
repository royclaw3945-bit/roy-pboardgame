using System;

namespace BoardGameRules.Entities.EventSourcing.Events;

[Serializable]
public abstract class BaseEvent : IEvent
{
	IEventMetadata IEvent.Metadata
	{
		get
		{
			return Metadata;
		}
		set
		{
			Metadata = (EventMetadata)value;
		}
	}

	public EventMetadata Metadata { get; set; }

	protected BaseEvent()
	{
		Metadata = new EventMetadata();
	}
}
