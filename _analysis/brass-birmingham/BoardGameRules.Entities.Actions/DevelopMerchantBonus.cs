using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Events;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class DevelopMerchantBonus
{
	private readonly IAggregateRoot _aggregateRoot;

	public static DevelopMerchantBonus Instance;

	public bool MerchantDevelopmentNeeded { get; set; }

	public DevelopMerchantBonus(IAggregateRoot aggregateRoot)
	{
		_aggregateRoot = aggregateRoot;
		Instance = this;
	}

	public void Develop(Player player, BuildingType buildingType)
	{
		if (buildingType == BuildingType.Undefined)
		{
			throw new InvalidOperationException("Given building type is null.");
		}
		BaseIndustry firstBuildingOfType = player.Mat.GetFirstBuildingOfType(buildingType);
		if (firstBuildingOfType != null && !firstBuildingOfType.CanBeDeveloped)
		{
			throw new InvalidOperationException("Industry can't be developed.");
		}
		_aggregateRoot.ApplyEvent(new IndustryDeveloped(player.Color, buildingType));
		FinishDevelop();
	}

	public void FinishDevelop()
	{
		_aggregateRoot.ApplyEvent(new MerchantBonusDevelopFinished());
	}

	public List<CantUseDevelopActionReason> GetDevelopMerchantBonusNotPossibleReasons(GameProgressInformation gameProgressInformation)
	{
		List<CantUseDevelopActionReason> list = new List<CantUseDevelopActionReason>();
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		if (!HasPlayerAnythingToDevelop(currentPlayer.Mat))
		{
			list.Add(CantUseDevelopActionReason.NOTHING_MORE_TO_DEVELOP);
			return list;
		}
		return list;
	}

	private bool HasPlayerAnythingToDevelop(PlayerMat playerMat)
	{
		if (!playerMat.CanIndustryBeDeveloped(BuildingType.CoalMine) && !playerMat.CanIndustryBeDeveloped(BuildingType.IronWorks) && !playerMat.CanIndustryBeDeveloped(BuildingType.CottonMill) && !playerMat.CanIndustryBeDeveloped(BuildingType.Manufacturer) && !playerMat.CanIndustryBeDeveloped(BuildingType.Pottery))
		{
			return playerMat.CanIndustryBeDeveloped(BuildingType.Brewery);
		}
		return true;
	}
}
