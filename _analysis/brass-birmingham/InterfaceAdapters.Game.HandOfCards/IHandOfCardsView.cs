using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Cards;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.HandOfCards;

public interface IHandOfCardsView : IBaseView
{
	void UpdateHandCardsView(List<BaseCard> cards, bool shouldHideCards);

	void HighlightCards(List<BaseCard> validCardList);

	void ShowCards();

	void HideCards();

	void TurnOffInteractions();

	Tuple<object, int> GetCardObjectInfo(BaseCard card);

	IAnimation CreateCardDiscardedAnimation(BaseCard cardToDiscard);

	IAnimation CreateOtherPlayerCardDiscardedAnimation(BaseCard cardToDiscard);

	(float x, float y, float z) GetCardPosition(int cardNumber);

	void EnableCardsLayoutGroup();
}
