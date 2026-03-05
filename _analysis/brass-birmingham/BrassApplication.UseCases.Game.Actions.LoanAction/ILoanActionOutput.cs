using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;

namespace BrassApplication.UseCases.Game.Actions.LoanAction;

public interface ILoanActionOutput : IBaseActionOutput
{
	void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowPlayerInfoToTakeLoan(IActionInProgressCancelDelegate cancelActionDelegate);

	void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate);

	void HideCards();

	void ShowIncomePanel(BrassApplicationGame game);

	void HideIncomePanel();

	void ReplayEvents(ReadOnlyCollection<IEvent> events);

	void ShowTooltip(string element, float x, float y, string playerName, PlayerColor? playerColor);

	void HideTooltip();
}
