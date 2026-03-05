using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.GamePhases;

public class EndOfGame : AggregateRootClient
{
	private readonly GameProgressInformation _gameProgressInformation;

	private readonly PlayersTrack _playersTrack;

	private readonly IRandomer _randomer;

	public EndOfGame(IAggregateRoot aggregateRoot, GameProgressInformation gameProgressInformation, PlayersTrack playersTrack, IRandomer randomer)
		: base(aggregateRoot)
	{
		_gameProgressInformation = gameProgressInformation;
		_playersTrack = playersTrack;
		_randomer = randomer;
		_eventRegistrations.Add(_aggregateRoot.RegisterApplyEvent<GameEnded>(ApplyEvent));
	}

	public void EndGame(Dictionary<PlayerColor, int> lastEraIndustriesVPs, Dictionary<PlayerColor, int> lastEraLinksVPs, Dictionary<PlayerColor, int> lastEraBonusesVPs)
	{
		_aggregateRoot.ApplyEvent(new GameEnded(lastEraIndustriesVPs, lastEraLinksVPs, lastEraBonusesVPs));
	}

	private void ApplyEvent(GameEnded gameEnded)
	{
		_gameProgressInformation.IsGameOver = true;
		List<Player> source = new List<Player>(_playersTrack.Players);
		source = source.OrderByDescending((Player p) => 1000000000.0 * (double)p.VP + 1000000.0 * (double)p.IncomeMarkerPosition + 1000.0 * (double)p.Money + (double)(_randomer.Next() % 1000000 / 1000000)).ToList();
		_gameProgressInformation.EndGameRanking = source.Select((Player p) => p.Color).ToList();
		Dictionary<PlayerColor, (int, int, int, int, int)> dictionary = new Dictionary<PlayerColor, (int, int, int, int, int)>();
		foreach (Player player in _playersTrack.Players)
		{
			dictionary.Add(player.Color, (gameEnded.LastEraLinksVPs[player.Color], gameEnded.LastEraIndustriesVPs[player.Color], gameEnded.LastEraBonusesVPs[player.Color], player.VP, source.IndexOf(player)));
		}
		_gameProgressInformation.EndGamePoints = dictionary;
	}
}
