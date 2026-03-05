using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.BaseAction;

public abstract class BaseActionUseCase : IUseCase, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate
{
	private readonly IBaseActionOutput _basePresenter;

	protected readonly IGameHistory _gameHistory;

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(BaseActionUseCase));

	protected BaseActionUseCase(IBaseActionOutput basePresenter, IGameHistory gameHistory)
	{
		_basePresenter = basePresenter;
		_gameHistory = gameHistory;
	}

	protected virtual void ConfirmAction()
	{
		Logger.Debug("ConfirmAction");
		_gameHistory.CurrentGame.Game.TurnFlow.ConfirmCurrentAction();
		_basePresenter.RestoreCardsView();
		_basePresenter.HideActionInProgressView();
		PresentEvents();
	}

	public virtual void CancelAction()
	{
		Logger.Debug("CancelAction");
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		currentGame.Game.TurnFlow.CancelCurrentAction();
		if (_gameHistory.IsCancelActionAvailable())
		{
			_gameHistory.CancelAction();
		}
		_basePresenter.RestoreCardsView();
		_basePresenter.HideActionInProgressView();
		_basePresenter.ShowPlayerActionsButtons();
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		_basePresenter.UpdateGameStatus(playerMetadataByColor);
	}

	protected void PresentEvents()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		_basePresenter.PresentEvents(currentGame.Game.GetUncommittedEvents(), playerMetadataByColor, this);
	}

	public void ReplayStarted()
	{
	}

	public virtual void ReplayCompleted()
	{
		int remainingActionsCount = _gameHistory.CurrentGame.Game.TurnFlow.GetRemainingActionsCount();
		bool flag = _gameHistory.CurrentGame.Game.TurnFlow.CurrentAction != null;
		Logger.Debug("BaseActionUseCase.ReplayCompleted(), remaining actions count {0}, action in progress {1}", remainingActionsCount, flag);
		_basePresenter.HideReplayView();
		if (!flag)
		{
			if (remainingActionsCount == 0)
			{
				_basePresenter.ShowEndTurnButton();
			}
			else
			{
				_basePresenter.ShowPlayerActionsButtons();
			}
		}
	}
}
