using BrassApplication.UseCases.Game.HandOfCards;

namespace BrassApplication.UseCases.Tutorials.HandOfCards;

public interface ITutorialHandOfCardsOutput : IHandOfCardsOutput
{
	void HideCurrentStageArrows();
}
