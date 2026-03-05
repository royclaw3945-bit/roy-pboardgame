using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.GameProgressDetails;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IncomeTrack;

namespace InterfaceAdapters.Game.GameProgressDetails;

public class GameProgressDetailsController : IController, IIncomeTrackController
{
	private readonly IGameProgressDetailsInput _gameProgressDetailsUseCase;

	public GameProgressDetailsController(IGameProgressDetailsInput gameProgressDetailsUseCase)
	{
		_gameProgressDetailsUseCase = gameProgressDetailsUseCase;
	}

	public void OnViewCreated()
	{
		_gameProgressDetailsUseCase.UpdateGameProgressDetails();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnBackButtonClicked()
	{
		_gameProgressDetailsUseCase.OnBackButtonClicked();
	}

	public void OnMouseEnter(string element, float x, float y, PlayerColor? playerColor = null)
	{
		_gameProgressDetailsUseCase.ShowTooltip(element, x, y, playerColor);
	}

	public void OnMouseExit()
	{
		_gameProgressDetailsUseCase.HideTooltip();
	}
}
