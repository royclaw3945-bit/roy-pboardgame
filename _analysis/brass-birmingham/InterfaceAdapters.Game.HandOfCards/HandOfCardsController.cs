using BoardGameRules.Entities.Cards;
using BrassApplication.UseCases.Game.HandOfCards;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.HandOfCards;

public class HandOfCardsController : IController
{
	private IHandOfCardsInput _useCase;

	public HandOfCardsController(IHandOfCardsInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void RestoreUseCase(IHandOfCardsInput useCase)
	{
		_useCase = useCase;
	}

	public void OnCardClicked(BaseCard card)
	{
		_useCase.OnCardSelected(card);
	}

	public string GetRegionFriendlyName(string regionName)
	{
		return _useCase.GetRegionFriendlyName(regionName);
	}
}
