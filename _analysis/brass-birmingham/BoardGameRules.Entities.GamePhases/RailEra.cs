using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.GamePhases;

public class RailEra : BaseEra
{
	private readonly GameProgressInformation _gameProgressInformation;

	private readonly CardDealer _cardDealer;

	public RailEra(IAggregateRoot aggregateRoot, GameProgressInformation gameProgressInformation, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, CardDealer cardDealer, IRandomer randomer)
		: base(aggregateRoot, playersTrack, board, randomer)
	{
		_gameProgressInformation = gameProgressInformation;
		_cardDealer = cardDealer;
		_aggregateRoot.RegisterApplyEvent<RailEraStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<RailEraEnded>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<RailEraEndStarted>(ApplyEvent);
	}

	public void StartRailEra()
	{
		_aggregateRoot.ApplyEvent(new RailEraStarted());
	}

	public void EndRailEra()
	{
		_aggregateRoot.ApplyEvent(new RailEraEndStarted());
		ScoreLinks();
		ScoreFlippedIndustryTiles();
		ScoreBonusVP();
		_aggregateRoot.ApplyEvent(new RailEraEnded(EndOfEraLinksVPs, EndOfEraIndustriesVPs, EndOfEraBonusesVPs));
	}

	private void ApplyEvent(RailEraStarted obj)
	{
		_gameProgressInformation.CurrentGameEra = GameEra.RailEra;
		_gameProgressInformation.CurrentRoundNumber = 0;
		_cardDealer.DealCardsForStartOfEra(base.PlayersTrack.Players);
	}

	private void ApplyEvent(RailEraEnded obj)
	{
	}

	private void ApplyEvent(RailEraEndStarted obj)
	{
	}
}
