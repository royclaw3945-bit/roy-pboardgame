using System;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;

namespace BrassApplication.UseCases.Game.Actions.ScoutAction;

public interface IScoutActionOutput : IBaseActionOutput
{
	void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate);

	void HideCards();

	void ShowExchangeForWildCardsView(BrassApplicationGame game, ISelectCardDelegate selectCardDelegate);

	void SetupCardInSlot(int slotIndex, BaseCard card, Action onCallback);

	void HideExchangeForWildCardsView();

	void ReplayEvents(ReadOnlyCollection<IEvent> events);

	void EnableCardsLayoutGroup();
}
