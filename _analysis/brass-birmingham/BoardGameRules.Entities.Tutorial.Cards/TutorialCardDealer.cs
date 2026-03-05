using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Tutorial.Cards;

public class TutorialCardDealer : CardDealer
{
	private int _turnCounter = 1;

	private bool _turnCounterRaised;

	public TutorialCardDealer(List<BaseCard> deck)
		: base(deck)
	{
	}

	public override void DealCardsForStartOfEra(List<Player> players)
	{
		foreach (Player player in players)
		{
			switch (player.Color)
			{
			case PlayerColor.Red:
				ReturnHandToDeck(player);
				player.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Leek),
					GetLocationCard(RegionName.StokeOnTrent),
					GetLocationCard(RegionName.Stone),
					GetLocationCard(RegionName.Uttoxeter),
					GetLocationCard(RegionName.Redditch),
					GetLocationCard(RegionName.Nuneaton),
					GetIndustryCard(BuildingType.Brewery),
					GetIndustryCard(BuildingType.Manufacturer)
				});
				break;
			case PlayerColor.Yellow:
				ReturnHandToDeck(player);
				PrepareYellowHand(player);
				break;
			}
		}
	}

	private void PrepareYellowHand(Player player)
	{
		bool flag = _deck.Any((BaseCard c) => c is IndustryCard industryCard2 && industryCard2.BuildingTypes.Contains(BuildingType.CoalMine));
		bool flag2 = _deck.Any((BaseCard c) => c is IndustryCard industryCard2 && industryCard2.BuildingTypes.Contains(BuildingType.Pottery));
		bool flag3 = _deck.Any((BaseCard c) => c is IndustryCard industryCard2 && industryCard2.BuildingTypes.Contains(BuildingType.Pottery));
		int num = 0;
		while (num < 8)
		{
			bool flag4 = false;
			if (num < 8 && flag)
			{
				BaseCard industryCard = GetIndustryCard(BuildingType.CoalMine);
				flag = _deck.Any((BaseCard c) => c is IndustryCard industryCard2 && industryCard2.BuildingTypes.Contains(BuildingType.CoalMine));
				player.Hand.Add(industryCard);
				flag4 = true;
				num++;
			}
			if (num < 8 && flag2)
			{
				BaseCard industryCard = GetIndustryCard(BuildingType.Pottery);
				flag2 = _deck.Any((BaseCard c) => c is IndustryCard industryCard2 && industryCard2.BuildingTypes.Contains(BuildingType.Pottery));
				player.Hand.Add(industryCard);
				flag4 = true;
				num++;
			}
			if (num < 8 && flag3)
			{
				BaseCard industryCard = GetIndustryCard(BuildingType.IronWorks);
				flag3 = _deck.Any((BaseCard c) => c is IndustryCard industryCard2 && industryCard2.BuildingTypes.Contains(BuildingType.IronWorks));
				player.Hand.Add(industryCard);
				flag4 = true;
				num++;
			}
			if (num < 8 && !flag4)
			{
				BaseCard industryCard = GetLocationCard(RegionName.Dudley);
				player.Hand.Add(industryCard);
				num++;
			}
		}
	}

	private void ReturnHandToDeck(Player player)
	{
		_deck.AddRange(player.Hand);
		player.Hand.Clear();
	}

	public override void DealCardsForEndOfTurn(Player player)
	{
		if (!_turnCounterRaised)
		{
			_turnCounter++;
			_turnCounterRaised = true;
		}
		else
		{
			_turnCounterRaised = false;
		}
		switch (player.Color)
		{
		case PlayerColor.Yellow:
			switch (_turnCounter)
			{
			case 2:
				player?.Hand.AddRange(new List<BaseCard> { GetIndustryCard(BuildingType.CottonMill) });
				break;
			case 3:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetIndustryCard(BuildingType.CottonMill),
					GetLocationCard(RegionName.Stafford)
				});
				break;
			case 4:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Kidderminster),
					GetLocationCard(RegionName.Birmingham)
				});
				break;
			case 5:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Belper),
					GetLocationCard(RegionName.Coalbrookdale)
				});
				break;
			case 6:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetIndustryCard(BuildingType.IronWorks),
					GetLocationCard(RegionName.BurtonOnTrent)
				});
				break;
			case 7:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Coventry),
					GetLocationCard(RegionName.Dudley)
				});
				break;
			case 8:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetIndustryCard(BuildingType.CottonMill),
					GetLocationCard(RegionName.Worcester)
				});
				break;
			default:
				base.DealCardsForEndOfTurn(player);
				break;
			}
			break;
		case PlayerColor.Red:
			switch (_turnCounter)
			{
			case 6:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Derby),
					GetLocationCard(RegionName.Coalbrookdale)
				});
				break;
			case 7:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Wolverhampton),
					GetLocationCard(RegionName.BurtonOnTrent)
				});
				break;
			case 8:
				player?.Hand.AddRange(new List<BaseCard>
				{
					GetLocationCard(RegionName.Cannock),
					GetFirstCard()
				});
				break;
			default:
				base.DealCardsForEndOfTurn(player);
				break;
			}
			break;
		default:
			base.DealCardsForEndOfTurn(player);
			break;
		}
		player?.Hand.AddRange(takeFromDeck(8 - player.Hand.Count, _deck));
	}

	private BaseCard GetIndustryCard(BuildingType buildingType)
	{
		BaseCard baseCard = _deck.First((BaseCard c) => c is IndustryCard industryCard && industryCard.BuildingTypes.Contains(buildingType));
		_deck.Remove(baseCard);
		return baseCard;
	}

	private BaseCard GetLocationCard(RegionName regionName)
	{
		BaseCard baseCard = _deck.First((BaseCard c) => c is LocationCard locationCard && locationCard.Region == regionName);
		_deck.Remove(baseCard);
		return baseCard;
	}

	private BaseCard GetFirstCard()
	{
		BaseCard baseCard = _deck[0];
		_deck.Remove(baseCard);
		return baseCard;
	}
}
