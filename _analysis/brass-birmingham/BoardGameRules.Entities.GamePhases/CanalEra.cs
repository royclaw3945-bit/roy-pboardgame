using System.Linq;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.GamePhases;

public class CanalEra : BaseEra
{
	private readonly IntroductoryGameScoring _introductoryGameScoring;

	public CanalEra(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, IRandomer randomer, GameProgressInformation gameProgressInformation)
		: base(aggregateRoot, playersTrack, board, randomer)
	{
		_introductoryGameScoring = new IntroductoryGameScoring(aggregateRoot, playersTrack, board, gameProgressInformation);
		_aggregateRoot.RegisterApplyEvent<CanalEraStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<CanalEraEnded>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<LinksRemoved>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<ObsoleteIndustriesRemoved>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<CanalEraEndStarted>(ApplyEvent);
	}

	public void StartCanalEra()
	{
		_aggregateRoot.ApplyEvent(new CanalEraStarted());
	}

	public void EndCanalEra(bool isIntroductoryGame)
	{
		_aggregateRoot.ApplyEvent(new CanalEraEndStarted(isIntroductoryGame));
		ScoreLinks();
		RemoveCanalLinks();
		ScoreBonusVP();
		ScoreFlippedIndustryTiles();
		RemoveObsoleteIndustries();
		if (isIntroductoryGame)
		{
			_introductoryGameScoring.CalculateAdditionalPoints();
		}
		_aggregateRoot.ApplyEvent(new CanalEraEnded(EndOfEraLinksVPs, EndOfEraIndustriesVPs, EndOfEraBonusesVPs));
	}

	private void RemoveCanalLinks()
	{
		foreach (Player player in base.PlayersTrack.Players)
		{
			_aggregateRoot.ApplyEvent(new LinksRemoved(player.Color));
		}
	}

	private void ApplyEvent(CanalEraStarted canalEraStarted)
	{
	}

	private void ApplyEvent(CanalEraEnded canalEraEnded)
	{
		base.Board.ResetMerchantBeer();
		base.PlayersTrack.Players.ForEach(delegate(Player p)
		{
			p.ResetLinkTileCount();
		});
		ShuffleDrawDeck();
	}

	private void RemoveObsoleteIndustries()
	{
		foreach (Player player in base.PlayersTrack.Players)
		{
			_aggregateRoot.ApplyEvent(new ObsoleteIndustriesRemoved(player.Color));
		}
	}

	private void ShuffleDrawDeck()
	{
		base.Board.CardDeck.AddRange(from c in base.PlayersTrack.Players.SelectMany((Player p) => p.DiscardPile)
			orderby base.Randomer.Next()
			select c);
	}

	private void ApplyEvent(LinksRemoved linksRemoved)
	{
		foreach (Link item in from l in base.Board.Regions.SelectMany((Region r) => r.Links)
			where l.HasOwner && l.Owner.Color.Equals(linksRemoved.PlayerColor)
			select l)
		{
			item.RemoveOwner();
		}
	}

	private void ApplyEvent(ObsoleteIndustriesRemoved obsoleteIndustriesRemoved)
	{
		foreach (BuildingSlot item in from s in base.Board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry != null && s.BuildedIndustry.Level == 1 && s.BuildedIndustry.Color.Equals(obsoleteIndustriesRemoved.PlayerColor)
			select s)
		{
			base.Board.RemovedIndustries.Add(item.BuildedIndustry);
			item.BuildedIndustry = null;
		}
	}

	private void ApplyEvent(CanalEraEndStarted canalEraEndStarted)
	{
	}
}
