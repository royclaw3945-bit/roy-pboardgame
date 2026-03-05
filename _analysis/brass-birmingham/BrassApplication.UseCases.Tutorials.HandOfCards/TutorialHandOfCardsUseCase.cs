using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Cards;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.HandOfCards;

namespace BrassApplication.UseCases.Tutorials.HandOfCards;

public class TutorialHandOfCardsUseCase : HandOfCardsUseCase, ITutorialHandOfCardsInput, IHandOfCardsInput
{
	private readonly ITutorialHandOfCardsOutput _presenter;

	private List<BaseCard> _cardsThatCanBeSelected;

	public TutorialHandOfCardsUseCase(ITutorialHandOfCardsOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public new void OnCardSelected(BaseCard card)
	{
		if (_cardsThatCanBeSelected.Any((BaseCard c) => c.Equals(card)))
		{
			if (_selectCardDelegate != null)
			{
				_presenter.HideCurrentStageArrows();
			}
			_selectCardDelegate?.SelectCard(card);
		}
	}

	public void SetCardsThatCanBeSelected(List<BaseCard> cards)
	{
		_cardsThatCanBeSelected = cards;
	}
}
