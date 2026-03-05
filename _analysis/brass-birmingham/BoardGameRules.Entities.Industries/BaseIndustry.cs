using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Industries;

public abstract class BaseIndustry
{
	public virtual BuildingType BuildingType { get; }

	public bool IsFlipped { get; set; }

	public string UniqueId { get; }

	public PlayerColor Color { get; }

	public int Level { get; }

	public int VPForBuilding { get; }

	public int VPPerLink { get; }

	public int IncomeIncrease { get; }

	public bool CanBeDeveloped { get; }

	public bool IsForCanalEra { get; }

	public bool IsForRailEra { get; }

	public int MoneyCost { get; }

	public int CoalCost
	{
		get
		{
			if (!ResourcesCost.ContainsKey(ResourceType.Coal))
			{
				return 0;
			}
			return ResourcesCost[ResourceType.Coal];
		}
	}

	public int IronCost
	{
		get
		{
			if (!ResourcesCost.ContainsKey(ResourceType.Iron))
			{
				return 0;
			}
			return ResourcesCost[ResourceType.Iron];
		}
	}

	public bool NeedsCoal => CoalCost > 0;

	public bool NeedsIron => IronCost > 0;

	public Dictionary<ResourceType, int> ResourcesCost { get; }

	protected BaseIndustry(string uniqueId, PlayerColor color, int level, int vpForBuilding, int vpPerLink, int incomeIncrease, bool canBeDeveloped, bool isForCanalEra, bool isForRailEra, int moneyCost, int coalCost, int ironCost)
	{
		UniqueId = uniqueId;
		Color = color;
		Level = level;
		VPForBuilding = vpForBuilding;
		VPPerLink = vpPerLink;
		IncomeIncrease = incomeIncrease;
		CanBeDeveloped = canBeDeveloped;
		IsForCanalEra = isForCanalEra;
		IsForRailEra = isForRailEra;
		MoneyCost = moneyCost;
		ResourcesCost = new Dictionary<ResourceType, int>();
		if (coalCost > 0)
		{
			ResourcesCost.Add(ResourceType.Coal, coalCost);
		}
		if (ironCost > 0)
		{
			ResourcesCost.Add(ResourceType.Iron, ironCost);
		}
	}

	public virtual void Build()
	{
	}

	public virtual int GetResource()
	{
		return 0;
	}

	public virtual int GetMaxResource()
	{
		return 0;
	}

	public virtual int GetBeerNeeded()
	{
		return 0;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		BaseIndustry baseIndustry = (BaseIndustry)obj;
		return UniqueId == baseIndustry.UniqueId;
	}

	public override int GetHashCode()
	{
		return UniqueId.GetHashCode();
	}
}
