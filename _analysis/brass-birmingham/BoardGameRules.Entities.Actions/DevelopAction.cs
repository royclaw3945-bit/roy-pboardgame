using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class DevelopAction : BaseAction
{
	public int NeededIron { get; private set; }

	public int DevelopCounter { get; private set; }

	public DevelopAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
		aggregateRoot.RegisterApplyEvent<IndustryDeveloped>(ApplyEvent);
	}

	public void Develop(BuildingType buildingType)
	{
		if (buildingType == BuildingType.Undefined)
		{
			throw new InvalidOperationException("Given building type is null.");
		}
		BaseIndustry firstBuildingOfType = _player.Mat.GetFirstBuildingOfType(buildingType);
		if (firstBuildingOfType != null && !firstBuildingOfType.CanBeDeveloped)
		{
			throw new InvalidOperationException("Industry can't be developed.");
		}
		if (DevelopCounter == 2)
		{
			throw new InvalidOperationException("You can develop 2 industries at max.");
		}
		NeededIron++;
		DevelopCounter++;
		base.AggregateRoot.ApplyEvent(new IndustryDeveloped(_player.Color, buildingType));
	}

	public override void SupplyIron(IIronSource ironSource, int amount)
	{
		base.GeneralAction.ConsumingIron.SupplyIron(ironSource, amount, NeededIron, _player);
		NeededIron -= amount;
	}

	public override void ResetAction()
	{
		base.ResetAction();
		NeededIron = 0;
		DevelopCounter = 0;
	}

	private void ApplyEvent(IndustryDeveloped industryDeveloped)
	{
		base.PlayersTrack.GetPlayerByColor(industryDeveloped.Player).DevelopIndustry(industryDeveloped.Type);
	}

	public bool CanDevelopGivenTypeOfIndustry(BuildingType buildingType, Player player)
	{
		if (!player.Mat.CanIndustryBeDeveloped(BuildingType.IronWorks))
		{
			return false;
		}
		if (player.Money < base.Board.IronMarket.IronPrice())
		{
			return base.Board.GetAmountOfIronInIronWorks() != 0;
		}
		return true;
	}

	public List<CantUseDevelopActionReason> GetDevelopActionNotPossibleReasons(GameProgressInformation gameProgressInformation)
	{
		List<CantUseDevelopActionReason> list = new List<CantUseDevelopActionReason>();
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		if (!HasPlayerAnythingToDevelop(currentPlayer.Mat))
		{
			list.Add(CantUseDevelopActionReason.NOTHING_MORE_TO_DEVELOP);
			return list;
		}
		if (currentPlayer.Money < base.Board.IronMarket.IronPrice() && base.Board.GetAmountOfIronInIronWorks() == 0)
		{
			list.Add(CantUseDevelopActionReason.NO_MONEY_TO_PURCHASE_IRON);
		}
		return list;
	}

	public bool HasPlayerIronForSecondDevelop(Player currentPlayer)
	{
		bool result = true;
		if (base.Board.GetAmountOfIronInIronWorks() < 2)
		{
			result = currentPlayer.Money >= base.Board.IronMarket.IronPrice(2 - base.Board.GetAmountOfIronInIronWorks());
		}
		return result;
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
