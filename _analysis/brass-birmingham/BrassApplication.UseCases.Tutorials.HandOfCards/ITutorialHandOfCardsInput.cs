using System.Collections.Generic;
using BoardGameRules.Entities.Cards;
using BrassApplication.UseCases.Game.HandOfCards;

namespace BrassApplication.UseCases.Tutorials.HandOfCards;

public interface ITutorialHandOfCardsInput : IHandOfCardsInput
{
	void SetCardsThatCanBeSelected(List<BaseCard> cards);
}
