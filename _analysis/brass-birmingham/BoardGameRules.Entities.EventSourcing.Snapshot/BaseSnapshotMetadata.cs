using System;
using System.Collections.Generic;

namespace BoardGameRules.Entities.EventSourcing.Snapshot;

[Serializable]
public class BaseSnapshotMetadata : ISnapshotMetadata, ICloneable
{
	private readonly string GAME_ID = "GameId";

	private readonly string REVISION = "Revision";

	protected Dictionary<string, object> Properties { get; set; }

	public string GameId
	{
		get
		{
			return (string)Properties[GAME_ID];
		}
		set
		{
			Properties[GAME_ID] = value;
		}
	}

	public int Revision
	{
		get
		{
			return Convert.ToInt32(Properties[REVISION]);
		}
		set
		{
			Properties[REVISION] = value;
		}
	}

	public BaseSnapshotMetadata(string gameId, int revision = 0)
	{
		Properties = new Dictionary<string, object>();
		GameId = gameId;
		Revision = revision;
	}

	public virtual object Clone()
	{
		return new BaseSnapshotMetadata(GameId, Revision)
		{
			Properties = new Dictionary<string, object>(Properties)
		};
	}
}
