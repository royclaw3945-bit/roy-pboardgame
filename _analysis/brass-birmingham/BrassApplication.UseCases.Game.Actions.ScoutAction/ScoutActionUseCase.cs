using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.ScoutAction;

public class ScoutActionUseCase : BaseActionUseCase, IScoutActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate
{
	private readonly IScoutActionOutput _presenter;

	private Player _currentPlayer;

	private BoardGameRules.Entities.Actions.ScoutAction _scoutAction;

	private int _discardedCardCount;

	private bool _isScoutPanelVisible;

	private List<BaseCard> _cardsToDiscard = new List<BaseCard>();

	public ScoutActionUseCase(IScoutActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartScoutAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_cardsToDiscard = new List<BaseCard>();
		_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_scoutAction = currentGame.Game.TurnFlow.StartScoutAction();
		_presenter.ShowCardsToDiscard(this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
	}

	public void SelectCard(BaseCard card)
	{
		_discardedCardCount++;
		if (_discardedCardCount == 1)
		{
			_scoutAction.DiscardCard(card, _currentPlayer);
			_presenter.ShowExchangeForWildCardsView(_gameHistory.CurrentGame, this);
			_isScoutPanelVisible = true;
			PresentEvents();
		}
		else if (_discardedCardCount == 2)
		{
			_cardsToDiscard.Add(card);
			_presenter.SetupCardInSlot(0, card, delegate
			{
			});
		}
		else
		{
			_cardsToDiscard.Add(card);
			_presenter.SetupCardInSlot(1, card, OnCompleteAction);
		}
	}

	private void OnCompleteAction()
	{
		_presenter.HideExchangeForWildCardsView();
		_scoutAction.Scout(_cardsToDiscard, _currentPlayer);
		ConfirmAction();
	}

	private void ShowAnimations()
	{
		_presenter.ReplayEvents(_gameHistory.CurrentGame.Game.GetUncommittedEvents());
	}

	private void ShowSwapCardAnimations()
	{
		_presenter.ReplayEvents(_gameHistory.CurrentGame.Game.GetUncommittedEvents());
	}

	public override void CancelAction()
	{
		_discardedCardCount = 0;
		_presenter.EnableCardsLayoutGroup();
		if (_isScoutPanelVisible)
		{
			_presenter.HideExchangeForWildCardsView();
		}
		_isScoutPanelVisible = false;
		base.CancelAction();
	}

	protected override void ConfirmAction()
	{
		_discardedCardCount = 0;
		_isScoutPanelVisible = false;
		base.ConfirmAction();
	}
}
