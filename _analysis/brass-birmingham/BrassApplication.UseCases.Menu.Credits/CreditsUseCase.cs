using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.Credits;

public class CreditsUseCase : IUseCase, ICreditsInput
{
	private readonly ICreditsOutput _presenter;

	public CreditsUseCase(ICreditsOutput presenter)
	{
		_presenter = presenter;
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}
}
