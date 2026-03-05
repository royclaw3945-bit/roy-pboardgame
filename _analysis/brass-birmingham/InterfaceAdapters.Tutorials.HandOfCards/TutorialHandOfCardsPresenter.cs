using System.Collections.Generic;
using BoardGameRules.Entities.Cards;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Tutorials.HandOfCards;
using BrassApplication.UseCases.Tutorials.Tutorial;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.HandOfCards;

public class TutorialHandOfCardsPresenter : HandOfCardsPresenter, ITutorialHandOfCardsOutput, IHandOfCardsOutput
{
	public TutorialHandOfCardsPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public new void UpdateHandCardsView(List<BaseCard> cards, bool shouldHideCards)
	{
		_router.GetView<IHandOfCardsView>().UpdateHandCardsView(cards, shouldHideCards);
	}

	public void HideCurrentStageArrows()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialGameSceneUseCase>().HideCurrentStageArrows();
	}
}
