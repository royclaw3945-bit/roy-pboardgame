using BrassApplication.UseCases.Menu.Credits;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Credits;

public class CreditsController : IController
{
	private readonly ICreditsInput _useCase;

	public CreditsController(ICreditsInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void BackClicked()
	{
		_useCase.BackClicked();
	}
}
