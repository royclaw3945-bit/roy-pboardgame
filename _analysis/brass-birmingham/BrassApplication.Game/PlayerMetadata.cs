using System;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.AI.AiBehaviors;

namespace BrassApplication.Game;

[Serializable]
public class PlayerMetadata : ICloneable
{
	public string GameId { get; set; }

	public string PlayerNetworkId { get; set; }

	public string NickName { get; set; }

	public AIType AiType { get; set; }

	public PlayerAvatar PlayerAvatar { get; set; }

	public IAiBehavior AIBehaviour { get; set; }

	public PlayerStartConfiguration PlayerStartConfiguration { get; set; }

	public int LastSeenEventRevision { get; set; }

	public override bool Equals(object obj)
	{
		if (obj is PlayerMetadata playerMetadata)
		{
			return Equals(playerMetadata);
		}
		return false;
	}

	public bool Equals(PlayerMetadata playerMetadata)
	{
		if (playerMetadata == null)
		{
			return false;
		}
		if (GameId == playerMetadata.GameId && PlayerNetworkId == playerMetadata.PlayerNetworkId && NickName == playerMetadata.NickName && AiType == playerMetadata.AiType && PlayerAvatar == playerMetadata.PlayerAvatar && AIBehaviour == playerMetadata.AIBehaviour && (PlayerStartConfiguration == null || PlayerStartConfiguration.Equals(playerMetadata.PlayerStartConfiguration)))
		{
			return LastSeenEventRevision == playerMetadata.LastSeenEventRevision;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GameId.GetHashCode() ^ PlayerNetworkId.GetHashCode() ^ NickName.GetHashCode() ^ AiType.GetHashCode() ^ PlayerAvatar.GetHashCode() ^ AIBehaviour.GetHashCode() ^ PlayerStartConfiguration.GetHashCode() ^ LastSeenEventRevision.GetHashCode();
	}

	public object Clone()
	{
		return new PlayerMetadata
		{
			GameId = GameId,
			PlayerNetworkId = PlayerNetworkId,
			NickName = NickName,
			AiType = AiType,
			PlayerAvatar = PlayerAvatar,
			AIBehaviour = AIBehaviour,
			PlayerStartConfiguration = (PlayerStartConfiguration?.Clone() as PlayerStartConfiguration),
			LastSeenEventRevision = LastSeenEventRevision
		};
	}
}
