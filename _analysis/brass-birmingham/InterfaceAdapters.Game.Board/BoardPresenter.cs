using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Common.LoadScene;
using BrassApplication.UseCases.Game.Actions.BuildAction;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.RegionDetails;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Board;

public class BoardPresenter : IPresenter, IBoardOutput
{
	protected readonly IRouter _router;

	protected readonly ILocalization _localization;

	public BoardPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public virtual void UpdateBoardView(BoardGameRules.Entities.Board.Board board, GameEra currentEra, Action<RegionName> actionOnClick)
	{
		BoardViewModel boardViewModel = PrepareBoardViewModel(board, currentEra, actionOnClick);
		_router.GetView<IBoardView>().UpdateBoardView(boardViewModel);
		_router.UseCaseBuilder.GetUseCase<LoadSceneUseCase>().SetIsBoardUpdated(isBoardUpdated: true);
	}

	public virtual void ShowRegionDetailsView(RegionName regionName, BrassApplicationGame game, IDictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons)
	{
		_router.ShowView<IRegionDetailsView>();
		RegionDetailsViewModel regionDetailsViewModel = PrepareRegionDetailsViewModel(regionName, game, cantBuildReasons);
		_router.GetView<IRegionDetailsView>().UpdateRegionDetailsView(regionDetailsViewModel);
	}

	public void StartBuildActionFromBoard(RegionName regionName)
	{
		if (!_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().GetIsReplayInProgress())
		{
			_router.UseCaseBuilder.GetUseCase<BuildActionUseCase>().StartBuildActionFromBoard(regionName);
		}
	}

	protected RegionDetailsViewModel PrepareRegionDetailsViewModel(RegionName regionName, BrassApplicationGame game, IDictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons)
	{
		Region regionByName = game.Game.Board.GetRegionByName(regionName);
		RegionDetailsViewModel regionDetailsViewModel = new RegionDetailsViewModel
		{
			Region = _router.GetView<IBoardView>().GetRegionViewForGivenRegionName(regionName),
			Slots = new List<List<IndustryViewModel>>(),
			ShowBackButton = true
		};
		List<BuildingSlot> buildingSlots = regionByName.BuildingSlots;
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		foreach (BuildingSlot item in buildingSlots)
		{
			List<IndustryViewModel> list = new List<IndustryViewModel>();
			foreach (BuildingType buildingType in item.BuildingTypes)
			{
				BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(buildingType);
				IList<CantUseBuildActionReason> list3;
				if (!cantBuildReasons.ContainsKey(new Tuple<BuildingSlot, BuildingType>(item, buildingType)))
				{
					IList<CantUseBuildActionReason> list2 = new List<CantUseBuildActionReason>();
					list3 = list2;
				}
				else
				{
					list3 = cantBuildReasons[new Tuple<BuildingSlot, BuildingType>(item, buildingType)];
				}
				IList<CantUseBuildActionReason> source = list3;
				list.Add(new IndustryViewModel
				{
					BuildingType = buildingType,
					BuildingSlot = item,
					Active = false,
					IndustryAvailable = (firstBuildingOfType != null),
					CantBuildReason = (source.Any() ? _localization.GetTranslation("ActionReasons/" + source.OrderBy((CantUseBuildActionReason r) => (int)r).First()) : ""),
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
					Color = (firstBuildingOfType?.Color ?? PlayerColor.Red),
					Name = _localization.GetTranslation("UI/" + firstBuildingOfType?.BuildingType)
				});
			}
			regionDetailsViewModel.Slots.Add(list);
		}
		return regionDetailsViewModel;
	}

	public void HideRegionDetailsView()
	{
		_router.HideView<IRegionDetailsView>();
		_router.MoveOnTop<IActionInProgressView>();
	}

	public void ShowTooltipBuildingSlot(List<BuildingType> buildingTypes, string region, RegionColor regionColor, float x, float y, int width, int height)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		string text = "";
		for (int i = 0; i < buildingTypes.Count; i++)
		{
			if (i != 0)
			{
				text += " / ";
			}
			text += _localization.GetTranslation($"UI/{buildingTypes[i]}");
		}
		string text2 = "<color=#";
		switch (regionColor)
		{
		case RegionColor.Blue:
			text2 += "315883>";
			break;
		case RegionColor.Cyan:
			text2 += "588585>";
			break;
		case RegionColor.Gray:
			text2 += "8c8c8c>";
			break;
		case RegionColor.Red:
			text2 += "883833>";
			break;
		case RegionColor.Violet:
			text2 += "7e6092>";
			break;
		case RegionColor.Yellow:
			text2 += "b18541>";
			break;
		}
		region = text2 + region + "</color>";
		string text3 = string.Format(_localization.GetTranslation("Tooltips/MAP/INDUSTRYSLOT"), text, region);
		Vector3 position = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0f);
		if (x < (float)(width / 2))
		{
			pivot.X = 0f;
		}
		else
		{
			pivot.X = 1f;
		}
		if (y < (float)(height / 2))
		{
			pivot.Y = 0f;
		}
		else
		{
			pivot.Y = 1f;
		}
		_router.GetView<ITooltipView>().ShowTooltip(text3, position, pivot);
	}

	public void ShowTooltipIndustry(BuildingType buildingType, bool activated, int level, int VP, int linkVP, int incomePoints, string owner, PlayerColor playerColor, float x, float y, int width, int height, int resources = -1, int maxResources = -1, int beerNeeded = -1)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		string text = "";
		string text2 = "";
		for (int i = 0; i < linkVP; i++)
		{
			text2 += "<sprite name=\"BbCommonIconsAtlas_4\">";
		}
		string text3 = "";
		for (int j = 0; j < beerNeeded; j++)
		{
			text3 += "<sprite name=\"BbCommonIconsAtlas_7\">";
		}
		switch (buildingType)
		{
		case BuildingType.Brewery:
			text = ((!activated) ? string.Format(_localization.GetTranslation("Tooltips/MAP/BREWERY"), level, text2, resources) : string.Format(_localization.GetTranslation("Tooltips/MAP/BREWERYACTIVATED"), level, text2));
			break;
		case BuildingType.CoalMine:
			text = ((!activated) ? string.Format(_localization.GetTranslation("Tooltips/MAP/COALMINE"), level, text2, resources, maxResources) : string.Format(_localization.GetTranslation("Tooltips/MAP/COALMINEACTIVATED"), level, text2));
			break;
		case BuildingType.CottonMill:
			text = ((!activated) ? string.Format(_localization.GetTranslation("Tooltips/MAP/COTTONMILL"), level, text2, text3) : string.Format(_localization.GetTranslation("Tooltips/MAP/COTTONMILLACTIVATED"), level, text2));
			break;
		case BuildingType.IronWorks:
			text = ((!activated) ? string.Format(_localization.GetTranslation("Tooltips/MAP/IRONWORKS"), level, text2, resources, maxResources) : string.Format(_localization.GetTranslation("Tooltips/MAP/IRONWORKSACTIVATED"), level, text2));
			break;
		case BuildingType.Manufacturer:
			text = ((!activated) ? string.Format(_localization.GetTranslation("Tooltips/MAP/MANUFACTURER"), level, text2, text3) : string.Format(_localization.GetTranslation("Tooltips/MAP/MANUFACTURERACTIVATED"), level, text2));
			break;
		case BuildingType.Pottery:
			text = ((!activated) ? string.Format(_localization.GetTranslation("Tooltips/MAP/POTTERY"), level, text2, text3) : string.Format(_localization.GetTranslation("Tooltips/MAP/POTTERYACTIVATED"), level, text2));
			break;
		}
		string text4 = "<color=";
		text4 = ((playerColor != PlayerColor.Violet) ? (text4 + playerColor.ToString().ToLower() + ">") : (text4 + "purple>"));
		owner = text4 + owner + "</color>";
		text = text + "\n" + string.Format(_localization.GetTranslation("Tooltips/MAP/INDUSTRYCOMMONPART"), VP, incomePoints, owner);
		Vector3 position = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0f);
		if (x < (float)(width / 2))
		{
			pivot.X = 0f;
		}
		else
		{
			pivot.X = 1f;
		}
		if (y < (float)(height / 2))
		{
			pivot.Y = 0f;
		}
		else
		{
			pivot.Y = 1f;
		}
		_router.GetView<ITooltipView>().ShowTooltip(text, position, pivot);
	}

	public void ShowTooltipLink(string owner, PlayerColor? playerColor, int pointsForLink, Dictionary<string, RegionColor> regionsConnected, string linkType, float x, float y, int width, int height)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		string text4;
		if (owner == null)
		{
			string text = "";
			for (int i = 0; i < regionsConnected.Count; i++)
			{
				if (i != 0)
				{
					text += "<sprite name=\"BbCommonIconsAtlas_4\"> ";
				}
				string text2 = regionsConnected.Keys.ToList()[i];
				string text3 = "<color=#";
				switch (regionsConnected[text2])
				{
				case RegionColor.Blue:
					text3 += "315883>";
					break;
				case RegionColor.Cyan:
					text3 += "588585>";
					break;
				case RegionColor.Gray:
					text3 += "8c8c8c>";
					break;
				case RegionColor.Red:
					text3 += "883833>";
					break;
				case RegionColor.Violet:
					text3 += "7e6092>";
					break;
				case RegionColor.Yellow:
					text3 += "b18541>";
					break;
				}
				text = text + text3 + text2 + "</color>";
			}
			text4 = string.Format(_localization.GetTranslation("Tooltips/MAP/LINKSLOT"), text);
		}
		else
		{
			string text5 = "<color=";
			text5 = ((playerColor != PlayerColor.Violet) ? (text5 + playerColor.ToString().ToLower() + ">") : (text5 + "purple>"));
			owner = text5 + owner + "</color>";
			text4 = string.Format(_localization.GetTranslation("Tooltips/MAP/LINK"), _localization.GetTranslation("UI/" + linkType.ToUpper()), pointsForLink, owner);
		}
		Vector3 position = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0f);
		if (x < (float)(width / 2))
		{
			pivot.X = 0f;
		}
		else
		{
			pivot.X = 1f;
		}
		if (y < (float)(height / 2))
		{
			pivot.Y = 0f;
		}
		else
		{
			pivot.Y = 1f;
		}
		_router.GetView<ITooltipView>().ShowTooltip(text4, position, pivot);
	}

	public void ShowTooltipBorderRegion(string regionName, List<BuildingType> sellableIndustries, BonusType bonus, float x, float y, int width, int height)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		string text = "";
		int count = sellableIndustries.Count;
		for (int i = 0; i < count; i++)
		{
			if (i != 0)
			{
				text = ((i != count - 1) ? (text + ", ") : (text + " " + _localization.GetTranslation("UI/AND") + " "));
			}
			text += _localization.GetTranslation($"UI/{sellableIndustries[i]}");
		}
		string text2 = string.Format(_localization.GetTranslation("Tooltips/MAP/MERCHANTREGION"), regionName, text, _localization.GetTranslation($"Tooltips/BONUS/{bonus}"));
		Vector3 position = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0f);
		if (x < (float)(width / 2))
		{
			pivot.X = 0f;
		}
		else
		{
			pivot.X = 1f;
		}
		if (y < (float)(height / 2))
		{
			pivot.Y = 0f;
		}
		else
		{
			pivot.Y = 1f;
		}
		_router.GetView<ITooltipView>().ShowTooltip(text2, position, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}

	protected BoardViewModel PrepareBoardViewModel(BoardGameRules.Entities.Board.Board board, GameEra currentEra, Action<RegionName> actionOnClick)
	{
		BoardViewModel boardViewModel = new BoardViewModel
		{
			RegionsViewModels = new Dictionary<RegionName, RegionViewModel>(),
			Links = new Dictionary<string, LinkViewModel>(),
			BorderRegionsViewModels = new Dictionary<RegionName, BorderRegionViewModel>()
		};
		foreach (Region region in board.Regions)
		{
			List<BuildingSlotViewModel> list = new List<BuildingSlotViewModel>();
			foreach (BuildingSlot buildingSlot in region.BuildingSlots)
			{
				BuildingSlotViewModel item = new BuildingSlotViewModel
				{
					BuildingSlot = buildingSlot,
					ImageName = buildingSlot.GetCurrentImageName(),
					BackgroundImageName = buildingSlot.GetBackgroundImageName(),
					Build = (buildingSlot.BuildedIndustry != null),
					Highlighted = false,
					ResourceCount = (buildingSlot.BuildedIndustry?.GetResource() ?? 0),
					IsInteractive = false
				};
				list.Add(item);
			}
			RegionViewModel value = new RegionViewModel
			{
				Name = region.Name,
				Color = region.Color,
				RegionsInteractive = true,
				BuildingSlotsViewModels = list,
				ActionOnClick = actionOnClick,
				FriendlyName = GetRegionFriendlyNameLocalization(region.Name)
			};
			boardViewModel.RegionsViewModels.Add(region.Name, value);
		}
		Dictionary<RegionName, BorderRegionViewModel> dictionary = new Dictionary<RegionName, BorderRegionViewModel>();
		foreach (BorderRegion borderRegion in board.GetBorderRegions((BorderRegion r) => true))
		{
			List<MerchantSlotViewModel> list2 = new List<MerchantSlotViewModel>();
			foreach (MerchantSlot merchantSlot in borderRegion.MerchantSlots)
			{
				MerchantSlotViewModel item2 = new MerchantSlotViewModel
				{
					MerchantSlot = merchantSlot,
					MerchantTileImageName = ((merchantSlot.MerchantTile != null && merchantSlot.MerchantTile.BuildingTypes.Any()) ? ((merchantSlot.MerchantTile.BuildingTypes.Count > 1) ? "UndefinedType" : string.Concat(merchantSlot.MerchantTile.BuildingTypes[0], "Type")) : "Blank"),
					HasBeer = merchantSlot.HasBeer,
					Highlighted = true,
					HighlightedBeer = false,
					IsInteractive = false,
					ActionOnClick = delegate
					{
					}
				};
				list2.Add(item2);
			}
			BorderRegionViewModel value2 = new BorderRegionViewModel
			{
				Name = borderRegion.Name,
				MerchantSlotsViewModels = list2,
				BonusType = borderRegion.MerchantBeerBonusType
			};
			dictionary.Add(borderRegion.Name, value2);
		}
		boardViewModel.BorderRegionsViewModels = dictionary;
		bool flag = currentEra == GameEra.CanalEra;
		IEnumerable<Link> source = board.Regions.SelectMany((Region r) => r.Links);
		IEnumerable<Link> canals = source.Where((Link l) => l.GetType() == typeof(Canal));
		IEnumerable<Link> rails = source.Where((Link l) => l.GetType() == typeof(Rail));
		source = ((!flag) ? rails.Concat(canals.Where((Link c) => !rails.Any((Link r) => r.LinkId == c.LinkId))) : canals.Concat(rails.Where((Link r) => !canals.Any((Link c) => c.LinkId == r.LinkId))));
		foreach (Link item3 in source)
		{
			string key = string.Concat(item3.Region1.Name, item3.Region2.Name);
			if (boardViewModel.Links.ContainsKey(key))
			{
				LinkViewModel linkViewModel = boardViewModel.Links[key];
				linkViewModel.IsTokenVisible |= item3.HasOwner;
				if (item3.HasOwner)
				{
					linkViewModel.TokenColor = item3.Owner.Color;
				}
				linkViewModel.IsRiverVisible |= item3 is Canal && flag;
				linkViewModel.IsRailVisible |= item3 is Rail;
			}
			else
			{
				boardViewModel.Links.Add(key, new LinkViewModel
				{
					Link = item3,
					IsRiverVisible = (flag && item3 is Canal),
					IsRailVisible = (item3 is Rail),
					IsTokenVisible = item3.HasOwner,
					CurrentEra = currentEra,
					TokenColor = (item3.Owner?.Color ?? PlayerColor.Blue)
				});
			}
		}
		return boardViewModel;
	}

	public string GetRegionFriendlyNameLocalization(RegionName regionName)
	{
		return _localization.GetTranslation("Regions/" + regionName);
	}
}
