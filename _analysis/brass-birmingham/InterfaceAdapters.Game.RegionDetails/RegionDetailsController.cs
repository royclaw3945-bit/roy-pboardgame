using BoardGameRules.Entities.Board.BuildingSlots;
using BrassApplication.UseCases.Game.RegionDetails;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.RegionDetails;

public class RegionDetailsController : IController
{
	private readonly IRegionDetailsInput _useCase;

	public RegionDetailsController(IRegionDetailsInput regionDetailsUseCase)
	{
		_useCase = regionDetailsUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnBuildClicked(BuildingSlot buildingSlot, BuildingType buildingType)
	{
		_useCase.BuildClicked(buildingSlot, buildingType);
	}

	public void OnBackButtonClicked()
	{
		_useCase.BackToBoard();
	}
}
