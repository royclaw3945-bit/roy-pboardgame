using System;

namespace BoardGameRules.Entities.EventSourcing.Snapshot;

public interface ISnapshot : ICloneable
{
	ISnapshotMetadata Metadata { get; }

	object Payload { get; }
}
