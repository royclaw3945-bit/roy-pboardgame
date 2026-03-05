using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.LoanAction;

public class LoanActionUseCase : BaseActionUseCase, ILoanActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate
{
	private readonly ILoanActionOutput _presenter;

	private Player _currentPlayer;

	private bool _isLoanPanelVisible;

	private BoardGameRules.Entities.Actions.LoanAction _loanAction;

	public LoanActionUseCase(ILoanActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartLoanAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_loanAction = currentGame.Game.TurnFlow.StartLoanAction();
		_presenter.ShowCardsToDiscard(this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
	}

	public void TakeLoan()
	{
		_loanAction.Loan(_currentPlayer.Color);
		ShowLoanAnimations();
		ConfirmAction();
	}

	public void SelectCard(BaseCard card)
	{
		_loanAction.DiscardCard(card, _currentPlayer);
		PresentEvents();
		_presenter.HideCards();
		_presenter.ShowIncomePanel(_gameHistory.CurrentGame);
		_presenter.ShowPlayerInfoToTakeLoan(this);
		_isLoanPanelVisible = true;
	}

	private void ShowLoanAnimations()
	{
		_presenter.ReplayEvents(_gameHistory.CurrentGame.Game.GetUncommittedEvents());
	}

	public override void CancelAction()
	{
		if (_isLoanPanelVisible)
		{
			_presenter.HideIncomePanel();
		}
		_isLoanPanelVisible = false;
		base.CancelAction();
	}

	public void HideIncomePanel()
	{
		_presenter.HideIncomePanel();
		_isLoanPanelVisible = false;
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
