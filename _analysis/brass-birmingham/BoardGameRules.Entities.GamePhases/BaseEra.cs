using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.GamePhases;

public abstract class BaseEra : AggregateRootClient
{
	protected Dictionary<PlayerColor, int> EndOfEraLinksVPs = new Dictionary<PlayerColor, int>();

	protected Dictionary<PlayerColor, int> EndOfEraIndustriesVPs = new Dictionary<PlayerColor, int>();

	protected Dictionary<PlayerColor, int> EndOfEraBonusesVPs = new Dictionary<PlayerColor, int>();

	protected PlayersTrack PlayersTrack { get; }

	protected BoardGameRules.Entities.Board.Board Board { get; }

	protected IRandomer Randomer { get; }

	public Dictionary<PlayerColor, int> GetEndOfEraLinksVPs()
	{
		return EndOfEraLinksVPs;
	}

	public Dictionary<PlayerColor, int> GetEndOfEraIndustriesVPs()
	{
		return EndOfEraIndustriesVPs;
	}

	public Dictionary<PlayerColor, int> GetCurrentEraBonusesVPs()
	{
		return EndOfEraBonusesVPs;
	}

	public BaseEra(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, IRandomer randomer)
		: base(aggregateRoot)
	{
		PlayersTrack = playersTrack;
		Board = board;
		Randomer = randomer;
		foreach (Player player in PlayersTrack.Players)
		{
			EndOfEraLinksVPs.Add(player.Color, 0);
			EndOfEraIndustriesVPs.Add(player.Color, 0);
			EndOfEraBonusesVPs.Add(player.Color, 0);
		}
	}

	protected void ScoreBonusVP()
	{
		foreach (Player player in PlayersTrack.Players)
		{
			EndOfEraBonusesVPs[player.Color] = player.VPFromBonusesInCurrentEra;
			player.VPFromBonusesInCurrentEra = 0;
		}
	}

	protected void ScoreLinks()
	{
		_aggregateRoot.ApplyEvent(new ScoreLinksPhaseStarted());
		foreach (Player player in PlayersTrack.Players)
		{
			HashSet<Link> hashSet = new HashSet<Link>();
			foreach (Region region in Board.Regions)
			{
				foreach (Link link in region.Links)
				{
					if (link.Owner == player && !hashSet.Contains(link))
					{
						hashSet.Add(link);
					}
				}
			}
			int num = 0;
			foreach (Link item in hashSet)
			{
				foreach (Region item2 in new List<Region> { item.Region1, item.Region2, item.Region3 })
				{
					if (item2 != null)
					{
						num = ((!(item2 is BorderRegion)) ? (num + (from s in item2.BuildingSlots
							select s.BuildedIndustry into i
							where i?.IsFlipped ?? false
							select i).Sum((BaseIndustry i) => i.VPPerLink)) : (num + 2));
					}
				}
			}
			EndOfEraLinksVPs[player.Color] = num;
			_aggregateRoot.ApplyEvent(new LinksVictoryPointsChanged(player.Color, num));
		}
	}

	protected void ScoreFlippedIndustryTiles()
	{
		_aggregateRoot.ApplyEvent(new ScoreFlippedIndustryTilesPhaseStarted());
		foreach (Player player in PlayersTrack.Players)
		{
			int num = (from s in Board.Regions.SelectMany((Region r) => r.BuildingSlots)
				select s.BuildedIndustry into i
				where i != null && i.IsFlipped && i.Color.Equals(player.Color)
				select i).Sum((BaseIndustry i) => i.VPForBuilding);
			EndOfEraIndustriesVPs[player.Color] = num;
			_aggregateRoot.ApplyEvent(new FlippedIndustriesVictoryPointsChanged(player.Color, num));
		}
	}
}
