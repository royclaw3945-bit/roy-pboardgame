using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.IndustriesDetails;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.IndustriesDetails;

public class IndustriesDetailsController : IController
{
	private readonly IIndustriesDetailsInput _useCase;

	public IndustriesDetailsController(IIndustriesDetailsInput industriesDetailsUseCase)
	{
		_useCase = industriesDetailsUseCase;
	}

	public void OnViewCreated()
	{
		_useCase.LoadIndustries();
		_useCase.SetPlayersSummaries();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnBackClicked()
	{
		_useCase.CloseIndustries();
	}

	public void OnIndustryClicked(BuildingType buildingType)
	{
		_useCase.OnIndustryClicked(buildingType);
	}

	public void OnCantDevelopHoverEnter(float x, float y)
	{
		_useCase.ShowCantDevelopTooltip(x, y);
	}

	public void OnCantDevelopHoverExit()
	{
		_useCase.HideTooltip();
	}

	public void OnPlayerPortraitClicked(PlayerColor color)
	{
		_useCase.LoadIndustries(color);
	}
}
