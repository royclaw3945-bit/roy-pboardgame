using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.PlayerActions;

public class PlayerActionsUseCase : IUseCase, IPlayerActionsInput
{
	private readonly IPlayerActionsOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(PlayerActionsUseCase));

	public PlayerActionsUseCase(IPlayerActionsOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void UpdatePlayerActions()
	{
		_presenter.UpdateAvailableActions();
		_presenter.UpdateActionsCounter(_gameHistory.CurrentGame);
		_presenter.UpdateUndoButtonState(_gameHistory.IsUndoActionAvailable());
	}

	private void UpdateGamePresentation()
	{
		Player currentPlayer = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = _gameHistory.CurrentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		_presenter.UpdateGamePresentation(playerMetadataByColor);
	}

	public void StartBuildAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<CantUseBuildActionReason> buildActionNotPossibleReasons = currentGame.Game.TurnFlow.BuildAction.GetBuildActionNotPossibleReasons(currentGame.Game.Board, currentGame.Game.GameProgressInformation);
		if (buildActionNotPossibleReasons.Any())
		{
			_presenter.ShowReasonsWhyCantUseBuildAction(buildActionNotPossibleReasons);
			return;
		}
		Logger.Debug("StartBuildAction");
		_presenter.StartBuildAction();
	}

	public void StartNetworkAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<CantUseLinkActionReason> networkActionNotPossibleReasons = currentGame.Game.TurnFlow.NetworkAction.GetNetworkActionNotPossibleReasons(currentGame.Game.Board, currentGame.Game.GameProgressInformation);
		if (networkActionNotPossibleReasons.Any())
		{
			_presenter.ShowReasonsWhyCantUseLinkAction(networkActionNotPossibleReasons);
			return;
		}
		Logger.Debug("StartNetworkAction");
		_presenter.StartNetworkAction();
	}

	public void StartDevelopAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<CantUseDevelopActionReason> developActionNotPossibleReasons = currentGame.Game.TurnFlow.DevelopAction.GetDevelopActionNotPossibleReasons(currentGame.Game.GameProgressInformation);
		if (developActionNotPossibleReasons.Any())
		{
			_presenter.ShowReasonsWhyCantUseDevelopAction(developActionNotPossibleReasons);
			return;
		}
		Logger.Debug("StartDevelopAction");
		_presenter.StartDevelopAction();
	}

	public void StartSellAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<CantUseSellActionReason> sellActionNotPossibleReasons = currentGame.Game.TurnFlow.SellAction.GetSellActionNotPossibleReasons(currentGame.Game.Board, currentGame.Game.GameProgressInformation);
		if (sellActionNotPossibleReasons.Any())
		{
			_presenter.ShowReasonsWhyCantUseSellAction(sellActionNotPossibleReasons);
			return;
		}
		Logger.Debug("StartSellAction");
		_presenter.StartSellAction();
	}

	public void StartLoanAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<CantUseLoanActionReason> loanActionNotPossibleReasons = currentGame.Game.TurnFlow.LoanAction.GetLoanActionNotPossibleReasons(currentGame.Game.GameProgressInformation);
		if (loanActionNotPossibleReasons.Any())
		{
			_presenter.ShowReasonsWhyCantUseLoanAction(loanActionNotPossibleReasons);
			return;
		}
		Logger.Debug("StartLoanAction");
		_presenter.StartLoanAction();
	}

	public void StartScoutAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<CantUseScoutActionReason> scoutActionNotPossibleReasons = currentGame.Game.TurnFlow.ScoutAction.GetScoutActionNotPossibleReasons(currentGame.Game.Board, currentGame.Game.GameProgressInformation);
		if (scoutActionNotPossibleReasons.Any())
		{
			_presenter.ShowReasonsWhyCantUseScoutAction(scoutActionNotPossibleReasons);
			return;
		}
		Logger.Debug("StartScoutAction");
		_presenter.StartScoutAction();
	}

	public void StartPassAction()
	{
		Logger.Debug("StartPassAction");
		_presenter.StartPassAction();
	}

	public void UndoAction()
	{
		_gameHistory.UndoAction();
		UpdateGamePresentation();
	}

	public void EndTurn()
	{
		_presenter.EndHumanTurn();
	}

	public void ShowTooltip(string actionButton, float buttonPositionX, float buttonPositionY, int screenWidth)
	{
		if (actionButton == "Link")
		{
			actionButton = ((_gameHistory.CurrentGame.Metadata.GameProgressInformation.CurrentGameEra != GameEra.CanalEra) ? "LinkRail" : "LinkCanal");
		}
		_presenter.ShowTooltip(actionButton, buttonPositionX, buttonPositionY, screenWidth);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}
}
