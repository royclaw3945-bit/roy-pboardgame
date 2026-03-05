using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;

namespace BoardGameRules.Entities.Players;

public class Player : AggregateRootClient, ICloneable
{
	private const int InitialLinkTileCount = 14;

	private const int InitialIncomeMarkerPosition = 10;

	private const int InitialMoney = 0;

	public PlayerColor Color { get; }

	public List<BaseCard> Hand { get; }

	public List<BaseCard> DiscardPile { get; }

	public PlayerMat Mat { get; set; }

	public int Money { get; set; }

	public int IncomeMarkerPosition { get; set; }

	public int IncomeLevel => IncomeTrack.GetCurrentIncomeLevel(IncomeMarkerPosition);

	public int VP { get; set; }

	public int VPFromBonusesInCurrentEra { get; set; }

	public int LinkTileCount { get; set; }

	public Player(IAggregateRoot aggregateRoot, PlayerColor color)
		: base(aggregateRoot)
	{
		Color = color;
		LinkTileCount = 14;
		IncomeMarkerPosition = 10;
		Money = 0;
		Hand = new List<BaseCard>();
		DiscardPile = new List<BaseCard>();
	}

	public bool HasEnoughMoney(int amount)
	{
		return Money >= amount;
	}

	public void ChangeVPBy(int amount)
	{
		_aggregateRoot.ApplyEvent(new VictoryPointsChanged(Color, amount));
	}

	public void ChangeIncomeMarkerPositionBy(int numberOfPoints, string source)
	{
		_aggregateRoot.ApplyEvent(new IncomeMarkerPositionChanged(Color, numberOfPoints, source));
	}

	public void DecreaseIncomeLevelBy(int numberOfLevels)
	{
		_aggregateRoot.ApplyEvent(new IncomeLevelChanged(Color, numberOfLevels));
	}

	public void DevelopIndustry(BuildingType buildingType)
	{
		Mat.DevelopIndustry(buildingType);
	}

	public bool HasLinkTiles()
	{
		return LinkTileCount > 0;
	}

	public bool TakeLinkTiles()
	{
		if (!HasLinkTiles())
		{
			return false;
		}
		LinkTileCount--;
		return true;
	}

	public object Clone()
	{
		return new Player(_aggregateRoot, Color)
		{
			LinkTileCount = LinkTileCount,
			IncomeMarkerPosition = IncomeMarkerPosition,
			Money = Money
		};
	}

	public void ResetLinkTileCount()
	{
		LinkTileCount = 14;
	}
}
