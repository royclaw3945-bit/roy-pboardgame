using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.DevelopAction;
using BrassApplication.UseCases.Game.IndustriesDetails;
using BrassApplication.UseCases.Tutorials.Actions.Develop;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.DevelopAction;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.Actions.Develop;

public class TutorialDevelopActionPresenter : DevelopActionPresenter, ITutorialDevelopActionOutput, IDevelopActionOutput, IBaseActionOutput
{
	public TutorialDevelopActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public override void UpdateIndustriesDetailsView(BrassApplicationGame game)
	{
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		_ = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().LoadIndustries();
		HighlightNone();
	}

	public void HighlightAllPosiible(BrassApplicationGame game)
	{
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		IndustriesDetailsViewModel industriesDetailsViewModel = new IndustriesDetailsViewModel
		{
			CoalMineHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.CoalMine) ? (mat.GetFirstBuildingOfType(BuildingType.CoalMine).Level - 1) : 10),
			BreweryHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.Brewery) ? (mat.GetFirstBuildingOfType(BuildingType.Brewery).Level - 1) : 10),
			IronWorksHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.IronWorks) ? (mat.GetFirstBuildingOfType(BuildingType.IronWorks).Level - 1) : 10),
			ManufactureHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.Manufacturer) ? (mat.GetFirstBuildingOfType(BuildingType.Manufacturer).Level - 1) : 10),
			CottonMillHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.CottonMill) ? (mat.GetFirstBuildingOfType(BuildingType.CottonMill).Level - 1) : 10),
			PotteryHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.Pottery) ? (mat.GetFirstBuildingOfType(BuildingType.Pottery).Level - 1) : 10)
		};
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().LoadIndustries();
		_router.GetView<IIndustriesDetailsView>().HighlightIndustriesToDevelop(industriesDetailsViewModel);
	}

	public void HighlightCottonMillOnly(BrassApplicationGame game)
	{
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		IndustriesDetailsViewModel industriesDetailsViewModel = new IndustriesDetailsViewModel
		{
			CoalMineHighestLevelIndex = 10,
			BreweryHighestLevelIndex = 10,
			IronWorksHighestLevelIndex = 10,
			ManufactureHighestLevelIndex = 10,
			CottonMillHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.CottonMill) ? (mat.GetFirstBuildingOfType(BuildingType.CottonMill).Level - 1) : 10),
			PotteryHighestLevelIndex = 10
		};
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().LoadIndustries();
		_router.GetView<IIndustriesDetailsView>().HighlightIndustriesToDevelop(industriesDetailsViewModel);
	}

	public void HighlightNone()
	{
		IndustriesDetailsViewModel industriesDetailsViewModel = new IndustriesDetailsViewModel
		{
			CoalMineHighestLevelIndex = 10,
			BreweryHighestLevelIndex = 10,
			IronWorksHighestLevelIndex = 10,
			ManufactureHighestLevelIndex = 10,
			CottonMillHighestLevelIndex = 10,
			PotteryHighestLevelIndex = 10
		};
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().LoadIndustries();
		_router.GetView<IIndustriesDetailsView>().HighlightIndustriesToDevelop(industriesDetailsViewModel);
	}

	public void ShowPlayerInfoToSelectIronWorks()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_IRON_WORKS"));
	}

	public void HighlightIronWorks(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsToHighlight)
	{
		BoardViewModel boardViewModel = new BoardViewModel
		{
			RegionsViewModels = new Dictionary<RegionName, RegionViewModel>(),
			Links = new Dictionary<string, LinkViewModel>()
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
					Highlighted = buildingSlotsToHighlight.Contains(buildingSlot),
					HighlightResource = buildingSlotsToHighlight.Contains(buildingSlot),
					IsInteractive = buildingSlotsToHighlight.Contains(buildingSlot),
					ActionOnClick = _router.UseCaseBuilder.GetUseCase<TutorialDevelopActionUseCase>().SelectIronSource,
					ResourceCount = (buildingSlot.BuildedIndustry?.GetResource() ?? 0)
				};
				list.Add(item);
			}
			RegionViewModel value = new RegionViewModel
			{
				Name = region.Name,
				Color = region.Color,
				RegionsInteractive = false,
				BuildingSlotsViewModels = list,
				FriendlyName = _localization.GetTranslation("Regions/" + region.Name)
			};
			boardViewModel.RegionsViewModels.Add(region.Name, value);
		}
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
	}
}
