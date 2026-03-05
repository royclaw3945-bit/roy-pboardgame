using BoardGameRules.Entities.Players;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.PlayersSummary;

public class PlayersSummaryUseCase : IUseCase, IPlayersSummaryInput
{
	private readonly IPlayersSummaryOutput _presenter;

	private readonly IGameHistory _gameHistory;

	public PlayersSummaryUseCase(IPlayersSummaryOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void UpdatePlayers()
	{
		_presenter.UpdatePlayersSummaryView(_gameHistory.CurrentGame);
	}

	public void PlayerSelected(PlayerColor playerColor)
	{
	}

	public void ShowTooltip(string element, float x, float y)
	{
		_presenter.ShowTooltip(element, x, y);
	}

	public void OnMouseExit()
	{
		_presenter.HideTooltip();
	}
}
