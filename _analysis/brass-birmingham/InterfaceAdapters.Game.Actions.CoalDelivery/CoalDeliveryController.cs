using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.CoalDelivery;

public class CoalDeliveryController : IController
{
	private ICoalDeliveryInput _useCase;

	public CoalDeliveryController(ICoalDeliveryInput coalDeliveryUseCase)
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
