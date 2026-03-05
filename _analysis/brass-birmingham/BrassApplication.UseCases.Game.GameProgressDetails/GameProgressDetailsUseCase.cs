using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.GameProgressDetails;

public class GameProgressDetailsUseCase : IUseCase, IGameProgressDetailsInput
{
	private readonly IGameProgressDetailsOutput _presenter;

	private readonly IGameHistory _gameHistory;

	public GameProgressDetailsUseCase(IGameProgressDetailsOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void UpdateGameProgressDetails()
	{
		_presenter.UpdateGameProgressDetailsView(_gameHistory.CurrentGame);
	}

	public void OnBackButtonClicked()
	{
		_presenter.HideGameProgressDetailsView();
	}

	public void ShowTooltip(string element, float x, float y, PlayerColor? playerColor)
	{
		string playerName = "ERROR";
		if (playerColor.HasValue)
		{
			playerName = _gameHistory.CurrentGame.Metadata.PlayerMetadata.Find((PlayerMetadata player) => player.PlayerStartConfiguration.Color == playerColor).NickName;
		}
		_presenter.ShowTooltip(element, x, y, playerName, playerColor);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}
}
