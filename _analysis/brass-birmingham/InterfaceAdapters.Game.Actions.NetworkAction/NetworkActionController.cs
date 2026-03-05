using BrassApplication.UseCases.Game.Actions.NetworkAction;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.NetworkAction;

public class NetworkActionController : IController
{
	private readonly INetworkActionInput _useCase;

	public NetworkActionController(INetworkActionInput networkActionUseCase)
	{
		_useCase = networkActionUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnEndButtonClicked()
	{
		_useCase.EndNetworkAction();
	}

	public void OnLinkAgainButtonClicked()
	{
		_useCase.SelectSecondLink();
	}
}
