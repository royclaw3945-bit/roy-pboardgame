using BrassApplication.UseCases.Game.Actions.DevelopAction;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.DevelopAction;

public class DevelopActionController : IController
{
	private readonly IDevelopActionInput _useCase;

	public DevelopActionController(IDevelopActionInput developActionUseCase)
	{
		_useCase = developActionUseCase;
	}

	public void OnOkButtonClicked()
	{
		_useCase.ConfirmNotEnoughResourcesForSecondDevelop();
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
