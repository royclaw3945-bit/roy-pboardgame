using BrassApplication.UseCases.Menu.Credits;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Menu.Credits;

public class CreditsPresenter : IPresenter, ICreditsOutput
{
	private readonly IRouter _router;

	public CreditsPresenter(IRouter router)
	{
		_router = router;
	}

	public void HideView()
	{
		_router.HideView<ICreditsView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}
}
