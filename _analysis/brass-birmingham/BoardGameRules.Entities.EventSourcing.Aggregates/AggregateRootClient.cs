using System.Collections.Generic;

namespace BoardGameRules.Entities.EventSourcing.Aggregates;

public abstract class AggregateRootClient
{
	protected IAggregateRoot _aggregateRoot;

	protected readonly List<IEventRegistration> _eventRegistrations = new List<IEventRegistration>();

	protected AggregateRootClient(IAggregateRoot aggregateRoot)
	{
		_aggregateRoot = aggregateRoot;
	}
}
