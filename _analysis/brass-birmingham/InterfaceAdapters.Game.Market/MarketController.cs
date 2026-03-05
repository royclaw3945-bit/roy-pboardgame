using BrassApplication.UseCases.Game.Market;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Market;

public class MarketController : IController
{
	private readonly IMarketInput _marketUseCase;

	public MarketController(IMarketInput marketUseCase)
	{
		_marketUseCase = marketUseCase;
	}

	public void OnViewCreated()
	{
		_marketUseCase.UpdateMarket();
	}

	public void OnViewDestroyed()
	{
	}

	internal void OnMouseEnter(string element, float x, float y)
	{
		_marketUseCase.ShowTooltip(element, x, y);
	}

	internal void OnMouseExit()
	{
		_marketUseCase.HideTooltip();
	}
}
