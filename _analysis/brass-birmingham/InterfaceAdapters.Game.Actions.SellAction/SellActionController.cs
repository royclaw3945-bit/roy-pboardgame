using BrassApplication.UseCases.Game.Actions.SellAction;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.SellAction;

public class SellActionController : IController
{
	private readonly ISellActionInput _useCase;

	public SellActionController(ISellActionInput sellActionUseCase)
	{
		_useCase = sellActionUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnEndButtonClicked()
	{
		_useCase.EndSellAction();
	}

	public void OnSellAgainButtonClicked()
	{
		_useCase.SelectNextIndustryToSell();
	}
}
