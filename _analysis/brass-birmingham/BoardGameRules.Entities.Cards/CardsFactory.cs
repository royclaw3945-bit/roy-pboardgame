using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.Cards;

public class CardsFactory
{
	private readonly IRandomer _randomer;

	private readonly List<BaseCard> _locationAndIndustryCards = new List<BaseCard>();

	private readonly List<BaseCard> _wildLocationCards = new List<BaseCard>();

	private readonly List<BaseCard> _wildIndustryCards = new List<BaseCard>();

	public CardsFactory(IRandomer randomer)
	{
		_randomer = randomer;
		_locationAndIndustryCards = new List<BaseCard>();
		_wildLocationCards = new List<BaseCard>();
		_wildIndustryCards = new List<BaseCard>();
		GenerateAllCards();
	}

	public List<BaseCard> GetLocationAndIndustryCards(NumberOfPlayers numberOfPlayers)
	{
		return (from c in _locationAndIndustryCards
			where c.MinNumberOfPlayers <= numberOfPlayers
			orderby _randomer.Next()
			select c).ToList();
	}

	public List<BaseCard> GetIndustryWildCards(NumberOfPlayers numberOfPlayers)
	{
		return (from c in _wildIndustryCards
			where c.MinNumberOfPlayers <= numberOfPlayers
			orderby _randomer.Next()
			select c).ToList();
	}

	public List<BaseCard> GetLocationWildCards(NumberOfPlayers numberOfPlayers)
	{
		return (from c in _wildLocationCards
			where c.MinNumberOfPlayers <= numberOfPlayers
			orderby _randomer.Next()
			select c).ToList();
	}

	private void GenerateAllCards()
	{
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, RegionName.Belper, RegionColor.Cyan));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, RegionName.Belper, RegionColor.Cyan));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, RegionName.Derby, RegionColor.Cyan));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, RegionName.Derby, RegionColor.Cyan));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, RegionName.Derby, RegionColor.Cyan));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.Leek, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.Leek, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.StokeOnTrent, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.StokeOnTrent, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.StokeOnTrent, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.Stone, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.Stone, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, RegionName.Uttoxeter, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, RegionName.Uttoxeter, RegionColor.Blue));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Stafford, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Stafford, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.BurtonOnTrent, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.BurtonOnTrent, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Cannock, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Cannock, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Tamworth, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Walsall, RegionColor.Red));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Coalbrookdale, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Coalbrookdale, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Coalbrookdale, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Dudley, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Dudley, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Kidderminster, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Kidderminster, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Wolverhampton, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Wolverhampton, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Worcester, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Worcester, RegionColor.Yellow));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Birmingham, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Birmingham, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Birmingham, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Coventry, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Coventry, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Coventry, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Nuneaton, RegionColor.Violet));
		_locationAndIndustryCards.Add(new LocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, RegionName.Redditch, RegionColor.Violet));
		for (int i = 0; i < 4; i++)
		{
			_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, new List<BuildingType> { BuildingType.IronWorks }, i / 2));
		}
		for (int j = 0; j < 2; j++)
		{
			_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, new List<BuildingType> { BuildingType.CoalMine }, j % 2));
		}
		_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, new List<BuildingType> { BuildingType.CoalMine }));
		for (int k = 0; k < 6; k++)
		{
			_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three, new List<BuildingType>
			{
				BuildingType.CottonMill,
				BuildingType.Manufacturer
			}, (k < 5) ? (k / 2) : (k - 2)));
		}
		for (int l = 0; l < 2; l++)
		{
			_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, new List<BuildingType>
			{
				BuildingType.CottonMill,
				BuildingType.Manufacturer
			}, l % 2));
		}
		for (int m = 0; m < 2; m++)
		{
			_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, new List<BuildingType> { BuildingType.Pottery }, m % 2));
		}
		_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four, new List<BuildingType> { BuildingType.Pottery }));
		for (int n = 0; n < 5; n++)
		{
			_locationAndIndustryCards.Add(new IndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two, new List<BuildingType> { BuildingType.Brewery }, n / 3));
		}
		_wildLocationCards.Add(new WildLocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two));
		_wildLocationCards.Add(new WildLocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two));
		_wildLocationCards.Add(new WildLocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three));
		_wildLocationCards.Add(new WildLocationCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four));
		_wildIndustryCards.Add(new WildIndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two));
		_wildIndustryCards.Add(new WildIndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Two));
		_wildIndustryCards.Add(new WildIndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Three));
		_wildIndustryCards.Add(new WildIndustryCard(UniqueIdGenerator.GenerateCardUniqueId(_randomer), NumberOfPlayers.Four));
	}
}
