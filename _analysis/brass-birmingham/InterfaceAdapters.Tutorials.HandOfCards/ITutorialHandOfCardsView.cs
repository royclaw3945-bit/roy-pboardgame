using InterfaceAdapters.Common;
using InterfaceAdapters.Game.HandOfCards;

namespace InterfaceAdapters.Tutorials.HandOfCards;

public interface ITutorialHandOfCardsView : IHandOfCardsView, IBaseView
{
	void SlipCardsOut();
}
