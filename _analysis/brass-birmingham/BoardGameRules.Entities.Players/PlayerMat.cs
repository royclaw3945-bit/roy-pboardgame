using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.Players;

public class PlayerMat
{
	private readonly Dictionary<BuildingType, List<BaseIndustry>> _industries = new Dictionary<BuildingType, List<BaseIndustry>>();

	private readonly Dictionary<BuildingType, List<BaseIndustry>> _archiveIndustries = new Dictionary<BuildingType, List<BaseIndustry>>();

	public static PlayerMat PrepareMat(Player player, IRandomer randomer, GameProgressInformation gameProgressInformation)
	{
		return new PlayerMat
		{
			_industries = 
			{
				{
					BuildingType.CoalMine,
					new List<BaseIndustry>
					{
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 1, 2, 4, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 5, 0, 0, 2),
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 2, 1, 7, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 7, 0, 0, 3),
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 2, 1, 7, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 7, 0, 0, 3),
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 3, 1, 6, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 8, 0, 1, 4),
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 3, 1, 6, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 8, 0, 1, 4),
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 4, 1, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 10, 0, 1, 5),
						new CoalMine(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 4, 1, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 10, 0, 1, 5)
					}
				},
				{
					BuildingType.IronWorks,
					new List<BaseIndustry>
					{
						new IronWorks(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 3, 1, 3, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 5, 1, 0, 4),
						new IronWorks(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 1, 3, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 7, 1, 0, 4),
						new IronWorks(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 7, 1, 2, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 9, 1, 0, 5),
						new IronWorks(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 9, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 12, 1, 0, 6)
					}
				},
				{
					BuildingType.Pottery,
					new List<BaseIndustry>
					{
						new Pottery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 10, 1, 5, canBeDeveloped: false, isForCanalEra: false, isForRailEra: false, 17, 0, 1, 1),
						new Pottery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 1, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 0, 1, 0, 1),
						new Pottery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 11, 1, 5, canBeDeveloped: false, isForCanalEra: false, isForRailEra: false, 22, 2, 0, 2),
						new Pottery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 1, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 0, 1, 0, 1),
						new Pottery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 5, 20, 1, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: true, 24, 2, 0, 2)
					}
				},
				{
					BuildingType.Brewery,
					new List<BaseIndustry>
					{
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 4, 2, 4, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 5, 0, 1, NumberOfBeersFunc),
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 4, 2, 4, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 5, 0, 1, NumberOfBeersFunc),
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 2, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 7, 0, 1, NumberOfBeersFunc),
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 2, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 7, 0, 1, NumberOfBeersFunc),
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 7, 2, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 9, 0, 1, NumberOfBeersFunc),
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 7, 2, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 9, 0, 1, NumberOfBeersFunc),
						new Brewery(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 10, 2, 5, canBeDeveloped: true, isForCanalEra: false, isForRailEra: true, 9, 0, 1, () => 2)
					}
				},
				{
					BuildingType.CottonMill,
					new List<BaseIndustry>
					{
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 5, 1, 5, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 12, 0, 0, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 5, 1, 5, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 12, 0, 0, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 5, 1, 5, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 12, 0, 0, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 2, 4, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 14, 1, 0, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 2, 4, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 14, 1, 0, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 9, 1, 3, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 16, 1, 1, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 9, 1, 3, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 16, 1, 1, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 9, 1, 3, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 16, 1, 1, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 12, 1, 2, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 18, 1, 1, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 12, 1, 2, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 18, 1, 1, 1),
						new CottonMill(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 12, 1, 2, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 18, 1, 1, 1)
					}
				},
				{
					BuildingType.Manufacturer,
					new List<BaseIndustry>
					{
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 1, 3, 2, 5, canBeDeveloped: true, isForCanalEra: true, isForRailEra: false, 8, 1, 0, 1),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 10, 0, 1, 1),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 2, 5, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 10, 0, 1, 1),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 3, 4, 0, 4, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 12, 2, 0, 0),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 4, 3, 1, 6, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 8, 0, 1, 1),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 5, 8, 2, 2, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 16, 1, 0, 2),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 5, 8, 2, 2, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 16, 1, 0, 2),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 6, 7, 1, 6, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 20, 0, 0, 1),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 7, 9, 0, 4, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 16, 1, 1, 0),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 8, 11, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 20, 0, 2, 1),
						new Manufacturer(UniqueIdGenerator.GenerateIndustryUniqueId(randomer), player.Color, 8, 11, 1, 1, canBeDeveloped: true, isForCanalEra: false, isForRailEra: false, 20, 0, 2, 1)
					}
				}
			},
			_archiveIndustries = 
			{
				{
					BuildingType.CoalMine,
					new List<BaseIndustry>()
				},
				{
					BuildingType.IronWorks,
					new List<BaseIndustry>()
				},
				{
					BuildingType.Pottery,
					new List<BaseIndustry>()
				},
				{
					BuildingType.Brewery,
					new List<BaseIndustry>()
				},
				{
					BuildingType.CottonMill,
					new List<BaseIndustry>()
				},
				{
					BuildingType.Manufacturer,
					new List<BaseIndustry>()
				}
			}
		};
		int NumberOfBeersFunc()
		{
			if (gameProgressInformation.CurrentGameEra != GameEra.CanalEra)
			{
				return 2;
			}
			return 1;
		}
	}

	public void DevelopIndustry(BuildingType buildingType)
	{
		_archiveIndustries[buildingType].Add(_industries[buildingType][0]);
		_industries[buildingType].RemoveAt(0);
	}

	public BaseIndustry GetFirstBuildingOfType(BuildingType buildingType)
	{
		return _industries[buildingType].FirstOrDefault();
	}

	public bool RemoveGivenBuildingFromMat(BaseIndustry baseIndustry)
	{
		if (_industries[baseIndustry.BuildingType].Remove(baseIndustry))
		{
			_archiveIndustries[baseIndustry.BuildingType].Add(baseIndustry);
			return true;
		}
		return false;
	}

	public bool CanIndustryBeDeveloped(BuildingType buildingType)
	{
		if (_industries[buildingType].Any())
		{
			return _industries[buildingType].First().CanBeDeveloped;
		}
		return false;
	}

	public BaseIndustry GetIndustryByUniqueId(string uniqueId)
	{
		return _industries.SelectMany((KeyValuePair<BuildingType, List<BaseIndustry>> k) => k.Value).FirstOrDefault((BaseIndustry i) => i.UniqueId.Equals(uniqueId));
	}

	public List<BaseIndustry> GetIndustryList(BuildingType buildingType)
	{
		return _industries[buildingType];
	}

	public BaseIndustry GetArchiveIndustryOfGivenTypeAndLevel(BuildingType buildingType, int level)
	{
		return _archiveIndustries.First((KeyValuePair<BuildingType, List<BaseIndustry>> pair) => pair.Key == buildingType).Value.First((BaseIndustry i) => i.Level == level);
	}
}
