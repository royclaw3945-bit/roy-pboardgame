using BrassApplication.UseCases.Menu.OnlineGame.Register;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.Login;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Menu.Register;

public class RegisterPresenter : IPresenter, IRegisterOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public RegisterPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void ShowConnectionView()
	{
		_router.ShowView<IConnectionView>();
	}

	public void HideConnectionView()
	{
		_router.HideView<IConnectionView>();
	}

	public void ShowInfoViewRegisterSuccessful()
	{
		_router.GetView<IInfoPopupView>().Setup(_localization.GetTranslation("UI/CREATE_ACCOUNT_SUCCESSFULL"), _localization.GetTranslation("UI/OK"), delegate
		{
			_router.HideView<IInfoPopupView>();
			_router.HideView<IRegisterView>();
			_router.ShowView<ILoginView>();
		});
		_router.ShowView<IInfoPopupView>();
	}

	public void ShowInfoViewWithError(string errorKey)
	{
		string translation = _localization.GetTranslation("UI/ERROR_" + errorKey);
		if (string.IsNullOrEmpty(translation))
		{
			translation = _localization.GetTranslation("UI/ERROR_UnknownError");
		}
		_router.GetView<IInfoPopupView>().Setup(translation, _localization.GetTranslation("UI/OK"), delegate
		{
			_router.HideView<IInfoPopupView>();
		});
		_router.ShowView<IInfoPopupView>();
	}

	public void HideView()
	{
		_router.HideView<IRegisterView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}
}
