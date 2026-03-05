namespace BoardGameRules.Entities.EventSourcing.Events;

public interface IEvent
{
	IEventMetadata Metadata { get; set; }
}
