namespace BoardGameRules.Entities.EventSourcing.Events;

public interface IEventMetadata
{
	int Revision { get; set; }
}
