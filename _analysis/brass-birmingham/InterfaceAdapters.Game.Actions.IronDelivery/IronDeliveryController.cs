using BrassApplication.UseCases.Game.Actions.IronDelivery;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.IronDelivery;

public class IronDeliveryController : IController
{
	private IIronDeliveryInput _useCase;

	public IronDeliveryController(IIronDeliveryInput ironDeliveryUseCase)
	{
		_useCase = ironDeliveryUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
