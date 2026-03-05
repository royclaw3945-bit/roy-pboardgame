using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Entities.SellIndustry.Events;

namespace BoardGameRules.Entities.SellIndustry;

public class SellIndustryFlow : AggregateRootClient
{
	private BoardGameRules.Entities.Board.Board Board { get; set; }

	private PlayersTrack PlayersTrack { get; set; }

	public Dictionary<PlayerColor, int> _moneyToPayBySelling { get; private set; }

	public Dictionary<PlayerColor, List<BaseIndustry>> _soldInLastRound { get; private set; }

	public Dictionary<PlayerColor, int> _lostVpsInLastRound { get; private set; }

	public SellIndustryFlow(IAggregateRoot aggregateRoot, BoardGameRules.Entities.Board.Board board, PlayersTrack playersTrack)
		: base(aggregateRoot)
	{
		Board = board;
		PlayersTrack = playersTrack;
		_aggregateRoot.RegisterApplyEvent<IndustrySellingStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<IndustrySold>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<IndustrySellingEnded>(ApplyEvent);
	}

	public void StartIndustrySelling()
	{
		_moneyToPayBySelling = PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => Math.Max(0, -(p.IncomeLevel + p.Money)));
		_soldInLastRound = PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => new List<BaseIndustry>());
		_lostVpsInLastRound = PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => 0);
		_aggregateRoot.ApplyEvent(new IndustrySellingStarted());
	}

	public List<PlayerColor> GetPlayersThatNeedToSellIndustry()
	{
		return (from p in _moneyToPayBySelling
			where p.Value > 0
			select p.Key).ToList();
	}

	public Dictionary<PlayerColor, List<BaseIndustry>> GetSoldIndustriesInLastRound()
	{
		return _soldInLastRound;
	}

	public Dictionary<PlayerColor, int> GetLostVpsInLastRound()
	{
		return _lostVpsInLastRound;
	}

	public Dictionary<PlayerColor, int> GetRemainingDebt()
	{
		return _moneyToPayBySelling;
	}

	public int RemainingDebtToPay(PlayerColor playerColor)
	{
		return _moneyToPayBySelling[playerColor];
	}

	public void SellIndustry(PlayerColor playerColor, BuildingSlot buildingSlot)
	{
		if (buildingSlot == null)
		{
			throw new InvalidOperationException("Given building slot is null");
		}
		if (buildingSlot.BuildedIndustry == null)
		{
			throw new InvalidOperationException("Given building slot has not built industry");
		}
		if (_moneyToPayBySelling[playerColor] <= 0)
		{
			throw new InvalidOperationException("Given player doesn't need to sell industry");
		}
		int num = buildingSlot.BuildedIndustry.MoneyCost / 2;
		_moneyToPayBySelling[playerColor] = Math.Max(0, _moneyToPayBySelling[playerColor] - num);
		_soldInLastRound[playerColor].Add(buildingSlot.BuildedIndustry);
		_aggregateRoot.ApplyEvent(new IndustrySold(playerColor, num, buildingSlot.UniqueId));
	}

	public void EndIndustrySelling()
	{
		if (CanIndustrySellingBeCompleted())
		{
			foreach (KeyValuePair<PlayerColor, int> item in _moneyToPayBySelling)
			{
				if (item.Value > 0)
				{
					Player playerByColor = PlayersTrack.GetPlayerByColor(item.Key);
					int num = -Math.Min(playerByColor.VP, item.Value);
					_lostVpsInLastRound[playerByColor.Color] = -num;
					_aggregateRoot.ApplyEvent(new VictoryPointsChanged(item.Key, num));
				}
			}
			_aggregateRoot.ApplyEvent(new IndustrySellingEnded(_soldInLastRound, _lostVpsInLastRound));
			return;
		}
		throw new InvalidOperationException("Selling didn't end");
	}

	public bool CanIndustrySellingBeCompleted(PlayerColor playerColor)
	{
		if (_moneyToPayBySelling[playerColor] != 0)
		{
			return !Board.GetIndustriesOnBoardOfGivenPlayer(playerColor).Any();
		}
		return true;
	}

	public bool CanIndustrySellingBeCompleted()
	{
		return PlayersTrack.Players.All((Player p) => CanIndustrySellingBeCompleted(p.Color));
	}

	private void ApplyEvent(IndustrySellingStarted industrySellingStarted)
	{
	}

	private void ApplyEvent(IndustrySold industrySold)
	{
		PlayersTrack.GetPlayerByColor(industrySold.PlayerColor).Money += industrySold.MoneyToGet;
		Board.GetBuildingSlotByUniqueId(industrySold.SlotUniqueId).BuildedIndustry = null;
	}

	private void ApplyEvent(IndustrySellingEnded industrySellingEnded)
	{
	}
}
