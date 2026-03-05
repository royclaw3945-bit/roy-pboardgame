using BrassApplication.Game;

namespace BrassApplication.AI.AiBehaviors;

public class AiBehaviorFactory
{
	public static IAiBehavior CreateAiBehavior(PlayerMetadata playerMetadata)
	{
		return playerMetadata.AiType switch
		{
			AIType.Human => new NullAiBehavior(playerMetadata.PlayerNetworkId), 
			AIType.Tutorial => new TutorialAiBehavior(playerMetadata.PlayerNetworkId), 
			AIType.Easy => new EasyAiBehavior(playerMetadata.PlayerNetworkId), 
			AIType.Medium => new NormalAiBehavior(playerMetadata.PlayerNetworkId), 
			AIType.Hard => new HardAiBehavior(playerMetadata.PlayerNetworkId), 
			_ => new NullAiBehavior(playerMetadata.PlayerNetworkId), 
		};
	}
}
