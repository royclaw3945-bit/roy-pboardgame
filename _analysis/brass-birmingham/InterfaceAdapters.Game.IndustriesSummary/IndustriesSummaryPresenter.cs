using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.IndustriesSummary;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.IndustriesSummary;

public class IndustriesSummaryPresenter : IPresenter, IIndustriesSummaryOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public IndustriesSummaryPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void UpdateIndustriesSummary(BrassApplicationGame game, PlayerColor playerColor, IDictionary<BuildingType, IList<CantUseBuildActionReason>> cantBuildReasons)
	{
		Player player = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == playerColor);
		PlayerMat mat = player.Mat;
		List<IndustryViewModel> list = new List<IndustryViewModel>();
		List<BuildingType> list2 = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
		list2.Remove(BuildingType.Undefined);
		foreach (BuildingType item in list2)
		{
			IList<CantUseBuildActionReason> source = cantBuildReasons[item];
			BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(item);
			list.Add(new IndustryViewModel
			{
				BuildingType = item,
				Active = false,
				IndustryAvailable = (firstBuildingOfType != null),
				CantBuildReason = _localization.GetTranslation("ActionReasons/" + (source.Any() ? source.OrderBy((CantUseBuildActionReason r) => (int)r).First() : CantUseBuildActionReason.NO_REASON)),
				CanBeDeveloped = (firstBuildingOfType?.CanBeDeveloped ?? false),
				OnlyCanalEra = (firstBuildingOfType?.IsForCanalEra ?? false),
				OnlyRailEra = (firstBuildingOfType?.IsForRailEra ?? false),
				Level = (firstBuildingOfType?.Level ?? 0),
				MoneyCost = (firstBuildingOfType?.MoneyCost ?? 0),
				VPForBuilding = (firstBuildingOfType?.VPForBuilding ?? 0),
				IncomeMarkerIncrease = (firstBuildingOfType?.IncomeIncrease ?? 0),
				VpPerLink = (firstBuildingOfType?.VPPerLink ?? 0),
				CoalCost = (firstBuildingOfType?.CoalCost ?? 0),
				IronCost = (firstBuildingOfType?.IronCost ?? 0),
				BeerCost = ((firstBuildingOfType is BaseSellableIndustry baseSellableIndustry) ? baseSellableIndustry.CostOfBeerBarrelsToSellIndustry : 0),
				BeerSource = ((firstBuildingOfType is Brewery brewery) ? brewery._numberOfBeersFunc() : 0),
				CoalSource = ((firstBuildingOfType is CoalMine coalMine) ? coalMine.AvailableCoal : 0),
				IronSource = ((firstBuildingOfType is IronWorks ironWorks) ? ironWorks.AvailableIron : 0),
				HasBeer = (firstBuildingOfType is Brewery),
				HasCoal = (firstBuildingOfType is CoalMine),
				HasIron = (firstBuildingOfType is IronWorks),
				Color = (firstBuildingOfType?.Color ?? PlayerColor.Yellow)
			});
		}
		LinkCountViewModel linkCountViewModel = new LinkCountViewModel
		{
			LinkCount = player.LinkTileCount,
			Color = player.Color,
			GameEra = game.Game.GameProgressInformation.CurrentGameEra
		};
		_router.GetView<IIndustriesSummaryView>().UpdateIndustriesSummary(list, linkCountViewModel);
	}

	public void ShowIndustryDetails()
	{
		if (!_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().GetIsReplayInProgress())
		{
			_router.ShowView<IIndustriesDetailsView>();
		}
	}

	public void ShowTooltip(string element, float x, float y, int industryLevel = 0, int resources = 0)
	{
		string text = "ERROR";
		Vector3 zero = Vector3.Zero;
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		if (industryLevel == 0)
		{
			text = _localization.GetTranslation("Tooltips/HUD/NO_MORE_INDUSTRIES");
		}
		else
		{
			switch (element)
			{
			case "Cost":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/COST");
				break;
			case "Coal":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/COAL");
				break;
			case "Iron":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/IRON");
				break;
			case "VP":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/VP");
				break;
			case "Income":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/INCOME");
				break;
			case "VPForLinks":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/VPFORLINK");
				break;
			case "Brewery":
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/BREWERY"), industryLevel, resources);
				break;
			case "CoalMine":
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/COALMINE"), industryLevel, resources);
				break;
			case "IronWorks":
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/IRONWORKS"), industryLevel, resources);
				break;
			case "CottonMill":
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/COTTONMILL"), industryLevel);
				break;
			case "Manufacture":
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/MANUFACTURE"), industryLevel);
				break;
			case "Pottery":
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/POTTERY"), industryLevel);
				if (industryLevel == 1 || industryLevel == 3)
				{
					text += _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/POTTERY2");
				}
				break;
			case "CanalEra":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/CANALERAONLY");
				break;
			case "RailEra":
				text = _localization.GetTranslation("Tooltips/HUD/INDUSTRIESSUMMARY/RAILERAONLY");
				break;
			}
		}
		zero = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(1f, 0.5f);
		_router.GetView<ITooltipView>().ShowTooltip(text, zero, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}
}
