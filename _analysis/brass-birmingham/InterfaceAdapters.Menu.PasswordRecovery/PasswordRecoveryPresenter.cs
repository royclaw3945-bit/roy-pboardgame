using BrassApplication.UseCases.Menu.OnlineGame.PasswordRecovery;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.Login;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Menu.PasswordRecovery;

public class PasswordRecoveryPresenter : IPresenter, IPasswordRecoveryOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public PasswordRecoveryPresenter(IRouter router, ILocalization localization)
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

	public void ShowInfoViewEmailSendSuccessfully()
	{
		_router.GetView<IInfoPopupView>().Setup(_localization.GetTranslation("UI/INFO_RECOVER_PASSWORD_EMAIL_SEND"), _localization.GetTranslation("UI/OK"), delegate
		{
			_router.HideView<IInfoPopupView>();
			_router.HideView<IPasswordRecoveryView>();
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
		_router.HideView<IPasswordRecoveryView>().OnComplete(delegate
		{
			_router.ShowView<ILoginView>();
		});
	}
}
