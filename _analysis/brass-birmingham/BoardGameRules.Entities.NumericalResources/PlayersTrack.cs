using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils;
using BoardGameRules.Utils.Logging;

namespace BoardGameRules.Entities.NumericalResources;

public class PlayersTrack : AggregateRootClient
{
	private ILog Logger;

	private IList<PlayerStartConfiguration> _playerStartConfigurations;

	public List<Player> Players { get; set; }

	public Dictionary<PlayerColor, int> SpendMoneyInCurrentRound { get; set; }

	public PlayersTrack()
		: this(new AggregateRoot())
	{
	}

	public PlayersTrack(IAggregateRoot aggregateRoot)
		: base(aggregateRoot)
	{
		Players = new List<Player>();
		Logger = LogFactory.BuildLogger(GetType());
		aggregateRoot.RegisterApplyEvent<MoneySpent>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<NewTurnOrderDetermined>(ApplyEvent);
	}

	public void CreatePlayers(IAggregateRoot aggregateRoot, NumberOfPlayers numberOfPlayers)
	{
		if (_playerStartConfigurations != null && _playerStartConfigurations.Count >= (int)numberOfPlayers)
		{
			for (int i = 0; i < (int)numberOfPlayers; i++)
			{
				Players.Add(new Player(aggregateRoot, _playerStartConfigurations[i].Color));
			}
		}
		else
		{
			for (int j = 0; j < (int)numberOfPlayers; j++)
			{
				Players.Add(new Player(aggregateRoot, (PlayerColor)j));
			}
		}
		SpendMoneyInCurrentRound = Players.ToDictionary((Player p) => p.Color, (Player p) => 0);
	}

	public void SetPlayerStartConfigurations(IList<PlayerStartConfiguration> playerStartConfigurations)
	{
		_playerStartConfigurations = playerStartConfigurations;
	}

	public Player GetPlayerByColor(PlayerColor color)
	{
		return Players.FirstOrDefault((Player p) => p.Color.Equals(color));
	}

	public int GetMoneySpentByPlayerByColor(PlayerColor color)
	{
		return SpendMoneyInCurrentRound[color];
	}

	public Player GetFirstPlayer()
	{
		return Players.FirstOrDefault();
	}

	public BaseIndustry GetIndustryByUniqueId(string uniqueId)
	{
		return (from p in Players
			select p.Mat into m
			select m.GetIndustryByUniqueId(uniqueId)).FirstOrDefault((BaseIndustry i) => i != null);
	}

	public void RandomizePlayersOrder(IRandomer randomer)
	{
		Players = Players.OrderBy((Player p) => randomer.Next()).ToList();
	}

	public List<PlayerColor> GetPlayersOrder()
	{
		return Players.Select((Player p) => p.Color).ToList();
	}

	public List<PlayerColor> GetNewPlayersOrder()
	{
		return (from p in Players
			orderby SpendMoneyInCurrentRound[p.Color]
			select p.Color).ToList();
	}

	private void ApplyEvent(NewTurnOrderDetermined newTurnOrderDetermined)
	{
		Players = Players.OrderBy((Player p) => SpendMoneyInCurrentRound[p.Color]).ToList();
		foreach (Player player in Players)
		{
			SpendMoneyInCurrentRound[player.Color] = 0;
		}
	}

	private void ApplyEvent(MoneySpent moneySpent)
	{
		GetPlayerByColor(moneySpent.Player).Money -= moneySpent.Amount;
		SpendMoneyInCurrentRound[moneySpent.Player] += moneySpent.Amount;
	}
}
