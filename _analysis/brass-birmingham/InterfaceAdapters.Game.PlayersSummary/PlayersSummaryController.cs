using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.PlayersSummary;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayersSummary;

public class PlayersSummaryController : IController
{
	private readonly IPlayersSummaryInput _useCase;

	public PlayersSummaryController(IPlayersSummaryInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
		_useCase.UpdatePlayers();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnPlayerSummaryClicked(PlayerColor playerColor)
	{
		_useCase.PlayerSelected(playerColor);
	}

	internal void OnMouseEnter(string element, float x, float y)
	{
		_useCase.ShowTooltip(element, x, y);
	}

	internal void OnMouseExit()
	{
		_useCase.OnMouseExit();
	}
}
