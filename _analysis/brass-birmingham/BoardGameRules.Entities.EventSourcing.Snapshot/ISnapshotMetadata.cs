using System;

namespace BoardGameRules.Entities.EventSourcing.Snapshot;

public interface ISnapshotMetadata : ICloneable
{
	string GameId { get; set; }

	int Revision { get; set; }
}
