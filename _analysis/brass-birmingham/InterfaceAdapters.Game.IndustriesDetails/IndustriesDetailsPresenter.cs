using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.IndustriesDetails;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.IndustriesDetails;

public class IndustriesDetailsPresenter : IPresenter, IIndustriesDetailsOutput
{
	private readonly IRouter _router;

	protected readonly ILocalization _localization;

	public IndustriesDetailsPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void UpdateIndustriesDetails(BrassApplicationGame game)
	{
		UpdateIndustriesDetails(game, game.Game.GameProgressInformation.CurrentPlayer.Color);
	}

	public void UpdateIndustriesDetails(BrassApplicationGame game, PlayerColor color)
	{
		Player player = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == color);
		PlayerMat mat = player.Mat;
		Dictionary<BuildingType, List<IndustryViewModel>> dictionary = new Dictionary<BuildingType, List<IndustryViewModel>>();
		List<BuildingType> list = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
		list.Remove(BuildingType.Undefined);
		foreach (BuildingType item in list)
		{
			dictionary.Add(item, new List<IndustryViewModel>());
			foreach (BaseIndustry industry in mat.GetIndustryList(item))
			{
				if (dictionary[item].Any((IndustryViewModel i) => i.Level == industry.Level))
				{
					for (int num = 0; num < dictionary[item].Count; num++)
					{
						if (dictionary[item][num].Level == industry.Level)
						{
							dictionary[item][num].SameLevelCount++;
						}
					}
					continue;
				}
				if (dictionary[item].Count == 0 && industry.Level > 1)
				{
					for (int num2 = 1; num2 < industry.Level; num2++)
					{
						BaseIndustry archiveIndustryOfGivenTypeAndLevel = mat.GetArchiveIndustryOfGivenTypeAndLevel(item, num2);
						dictionary[item].Add(new IndustryViewModel
						{
							BuildingType = item,
							IndustryAvailable = false,
							CanBeDeveloped = false,
							Level = (archiveIndustryOfGivenTypeAndLevel?.Level ?? 0),
							MoneyCost = (archiveIndustryOfGivenTypeAndLevel?.MoneyCost ?? 0),
							VPForBuilding = (archiveIndustryOfGivenTypeAndLevel?.VPForBuilding ?? 0),
							IncomeMarkerIncrease = (archiveIndustryOfGivenTypeAndLevel?.IncomeIncrease ?? 0),
							VpPerLink = (archiveIndustryOfGivenTypeAndLevel?.VPPerLink ?? 0),
							CoalCost = (archiveIndustryOfGivenTypeAndLevel?.CoalCost ?? 0),
							IronCost = (archiveIndustryOfGivenTypeAndLevel?.IronCost ?? 0),
							BeerCost = ((archiveIndustryOfGivenTypeAndLevel is BaseSellableIndustry baseSellableIndustry) ? baseSellableIndustry.CostOfBeerBarrelsToSellIndustry : 0),
							BeerSource = ((archiveIndustryOfGivenTypeAndLevel is Brewery brewery) ? brewery._numberOfBeersFunc() : 0),
							CoalSource = ((archiveIndustryOfGivenTypeAndLevel is CoalMine coalMine) ? coalMine.AvailableCoal : 0),
							IronSource = ((archiveIndustryOfGivenTypeAndLevel is IronWorks ironWorks) ? ironWorks.AvailableIron : 0),
							HasBeer = (archiveIndustryOfGivenTypeAndLevel is Brewery),
							HasCoal = (archiveIndustryOfGivenTypeAndLevel is CoalMine),
							HasIron = (archiveIndustryOfGivenTypeAndLevel is IronWorks),
							OnlyCanalEra = archiveIndustryOfGivenTypeAndLevel.IsForCanalEra,
							OnlyRailEra = archiveIndustryOfGivenTypeAndLevel.IsForRailEra,
							Color = color
						});
					}
				}
				dictionary[item].Add(new IndustryViewModel
				{
					BuildingType = item,
					IndustryAvailable = (industry != null),
					CanBeDeveloped = industry.CanBeDeveloped,
					Level = (industry?.Level ?? 0),
					SameLevelCount = 1,
					MoneyCost = (industry?.MoneyCost ?? 0),
					VPForBuilding = (industry?.VPForBuilding ?? 0),
					IncomeMarkerIncrease = (industry?.IncomeIncrease ?? 0),
					VpPerLink = (industry?.VPPerLink ?? 0),
					CoalCost = (industry?.CoalCost ?? 0),
					IronCost = (industry?.IronCost ?? 0),
					BeerCost = ((industry is BaseSellableIndustry baseSellableIndustry2) ? baseSellableIndustry2.CostOfBeerBarrelsToSellIndustry : 0),
					BeerSource = ((industry is Brewery brewery2) ? brewery2._numberOfBeersFunc() : 0),
					CoalSource = ((industry is CoalMine coalMine2) ? coalMine2.AvailableCoal : 0),
					IronSource = ((industry is IronWorks ironWorks2) ? ironWorks2.AvailableIron : 0),
					HasBeer = (industry is Brewery),
					HasCoal = (industry is CoalMine),
					HasIron = (industry is IronWorks),
					OnlyCanalEra = industry.IsForCanalEra,
					OnlyRailEra = industry.IsForRailEra,
					Color = color
				});
			}
		}
		IndustriesDetailsViewModel obj = new IndustriesDetailsViewModel
		{
			CoalMineList = dictionary[BuildingType.CoalMine],
			BreweryList = dictionary[BuildingType.Brewery],
			IronWorksList = dictionary[BuildingType.IronWorks],
			ManufactureList = dictionary[BuildingType.Manufacturer],
			CottonMillList = dictionary[BuildingType.CottonMill],
			PotteryList = dictionary[BuildingType.Pottery]
		};
		BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(BuildingType.CoalMine);
		obj.CoalMineHighestLevelIndex = ((firstBuildingOfType != null) ? (firstBuildingOfType.Level - 1) : 10);
		BaseIndustry firstBuildingOfType2 = mat.GetFirstBuildingOfType(BuildingType.Brewery);
		obj.BreweryHighestLevelIndex = ((firstBuildingOfType2 != null) ? (firstBuildingOfType2.Level - 1) : 10);
		BaseIndustry firstBuildingOfType3 = mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		obj.IronWorksHighestLevelIndex = ((firstBuildingOfType3 != null) ? (firstBuildingOfType3.Level - 1) : 10);
		BaseIndustry firstBuildingOfType4 = mat.GetFirstBuildingOfType(BuildingType.Manufacturer);
		obj.ManufactureHighestLevelIndex = ((firstBuildingOfType4 != null) ? (firstBuildingOfType4.Level - 1) : 10);
		BaseIndustry firstBuildingOfType5 = mat.GetFirstBuildingOfType(BuildingType.CottonMill);
		obj.CottonMillHighestLevelIndex = ((firstBuildingOfType5 != null) ? (firstBuildingOfType5.Level - 1) : 10);
		BaseIndustry firstBuildingOfType6 = mat.GetFirstBuildingOfType(BuildingType.Pottery);
		obj.PotteryHighestLevelIndex = ((firstBuildingOfType6 != null) ? (firstBuildingOfType6.Level - 1) : 10);
		IndustriesDetailsViewModel industriesDetailsViewModel = obj;
		LinkCountViewModel linkCountViewModel = new LinkCountViewModel
		{
			LinkCount = player.LinkTileCount,
			Color = player.Color,
			GameEra = game.Game.GameProgressInformation.CurrentGameEra
		};
		_router.GetView<IIndustriesDetailsView>().UpdateIndustriesDetails(industriesDetailsViewModel, linkCountViewModel);
	}

	public void SetPlayerSummary(BrassApplicationGame game)
	{
		List<PlayerViewModel> list = new List<PlayerViewModel>();
		foreach (Player player in game.Game.PlayersTrack.Players)
		{
			list.Add(new PlayerViewModel
			{
				IsCurrent = game.Game.GameProgressInformation.CurrentPlayer.Equals(player),
				Color = player.Color,
				Avatar = (game.Metadata.GetPlayerMetadataByColor(player.Color)?.PlayerAvatar ?? PlayerAvatar.Arkwright),
				Money = player.Money,
				VP = player.VP,
				IncomeLevel = player.IncomeLevel,
				IncomeMarkerPosition = player.IncomeMarkerPosition,
				IncomeThreshold = BoardGameRules.Entities.NumericalResources.IncomeTrack.GetIncomeThreshold(player.IncomeMarkerPosition),
				MoneySpent = game.Game.PlayersTrack.SpendMoneyInCurrentRound[player.Color],
				Name = (game.Metadata.GetPlayerMetadataByColor(player.Color)?.NickName ?? "NO METADATA")
			});
		}
		_router.GetView<IIndustriesDetailsView>().SetPlayerSummary(list);
	}

	public void HideIndustriesDetails()
	{
		_router.HideView<IIndustriesDetailsView>();
	}

	public void ShowCantDevelopTooltip(float x, float y)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		_router.GetView<ITooltipView>().ShowTooltip(_localization.GetTranslation("Tooltips/INDUSTRIESDETAILS/CANTDEVELOP"), new Vector3(x, y, 0f), new Vector2(0f, 0f));
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}
}
