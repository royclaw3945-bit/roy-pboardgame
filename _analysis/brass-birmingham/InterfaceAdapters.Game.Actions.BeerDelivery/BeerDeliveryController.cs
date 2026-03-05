using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.BeerDelivery;

public class BeerDeliveryController : IController
{
	private IBeerDeliveryInput _useCase;

	public BeerDeliveryController(IBeerDeliveryInput coalDeliveryUseCase)
	{
		_useCase = coalDeliveryUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
