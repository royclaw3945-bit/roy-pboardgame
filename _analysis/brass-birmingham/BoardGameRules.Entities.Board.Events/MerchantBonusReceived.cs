using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Board.Events;

public class MerchantBonusReceived : BaseEvent
{
	public BonusType BonusType { get; }

	public RegionName RegionName { get; }

	public PlayerColor PlayerColor { get; }

	public MerchantBonusReceived(BonusType bonusType, PlayerColor playerColor, RegionName regionName)
	{
		BonusType = bonusType;
		PlayerColor = playerColor;
		RegionName = regionName;
	}
}
