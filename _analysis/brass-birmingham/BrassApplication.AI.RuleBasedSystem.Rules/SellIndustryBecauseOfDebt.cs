using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BoardGameRules.Entities.SellIndustry;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class SellIndustryBecauseOfDebt : BaseRule
{
	public SellIndustryBecauseOfDebt()
	{
		base.Id = RuleId.SellIndustryBecauseOfDebt;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 10000;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		return true;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			GameEra curEra = brassApplicationGame.Game.GameProgressInformation.CurrentGameEra;
			SellIndustryFlow sellIndustryFlow = brassApplicationGame.Game.SellIndustryFlow;
			List<BuildingSlot> source = brassApplicationGame.Game.Board.Regions.SelectMany((Region r) => r.BuildingSlots).ToList();
			foreach (PlayerColor player in sellIndustryFlow.GetPlayersThatNeedToSellIndustry())
			{
				List<BuildingSlot> source2 = source.Where((BuildingSlot s) => s.BuildedIndustry != null && s.BuildedIndustry.Color == player).ToList();
				int debt = sellIndustryFlow.RemainingDebtToPay(player);
				source2 = source2.OrderBy((BuildingSlot s) => CalculateVPWorth(s, curEra)).ToList();
				foreach (BuildingSlot item in BestSlotsToSell(new List<BuildingSlot>(), source2, debt, curEra))
				{
					BaseRule.Logger.Debug("Sell industry");
					if (!printOnly)
					{
						sellIndustryFlow.SellIndustry(player, item);
					}
				}
			}
		};
	}

	private int CalculateVPWorth(BuildingSlot slot, GameEra gameEra)
	{
		if (slot.BuildedIndustry == null)
		{
			return 0;
		}
		return ((gameEra != GameEra.CanalEra || slot.BuildedIndustry.Level != 2) ? 1 : 2) * (slot.BuildedIndustry.VPForBuilding + slot.BuildedIndustry.VPPerLink);
	}

	private List<BuildingSlot> BestSlotsToSell(List<BuildingSlot> chosen, List<BuildingSlot> toChoseFrom, int debt, GameEra curEra)
	{
		if (!toChoseFrom.Any() || debt <= 0)
		{
			return chosen;
		}
		BuildingSlot buildingSlot = toChoseFrom.First();
		chosen.Add(buildingSlot);
		toChoseFrom.Remove(buildingSlot);
		debt -= buildingSlot.BuildedIndustry.MoneyCost / 2;
		if (debt <= 0)
		{
			return chosen;
		}
		for (int i = 0; i < toChoseFrom.Count; i++)
		{
			List<BuildingSlot> chosen2 = new List<BuildingSlot>(chosen);
			List<BuildingSlot> toChoseFrom2 = new List<BuildingSlot>(toChoseFrom);
			List<BuildingSlot> list = BestSlotsToSell(chosen2, toChoseFrom2, debt, curEra);
			if (debt > 0 || list.Sum((BuildingSlot s) => CalculateVPWorth(s, curEra)) < chosen.Sum((BuildingSlot s) => CalculateVPWorth(s, curEra)))
			{
				chosen = list;
				debt = 0;
			}
		}
		return chosen;
	}
}
