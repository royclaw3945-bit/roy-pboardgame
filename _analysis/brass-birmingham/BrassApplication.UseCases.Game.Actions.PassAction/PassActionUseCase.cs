using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Cards;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.PassAction;

public class PassActionUseCase : BaseActionUseCase, IPassActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate
{
	private readonly IPassActionOutput _presenter;

	private BoardGameRules.Entities.Actions.PassAction _passAction;

	public PassActionUseCase(IPassActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartPassAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_passAction = currentGame.Game.TurnFlow.StartPassAction();
		_presenter.ShowCardsToDiscard(this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
	}

	public void SelectCard(BaseCard card)
	{
		_passAction.Pass(card, _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer);
		ConfirmAction();
	}
}
