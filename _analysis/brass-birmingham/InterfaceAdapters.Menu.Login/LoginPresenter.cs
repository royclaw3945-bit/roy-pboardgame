using BrassApplication.UseCases.Menu.OnlineGame.Login;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Menu.OnlineGameMenu;
using InterfaceAdapters.Menu.PasswordRecovery;
using InterfaceAdapters.Menu.Register;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Menu.Login;

public class LoginPresenter : IPresenter, ILoginOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public LoginPresenter(IRouter router, ILocalization localization)
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

	public void ShowOnlineGamesView()
	{
		_router.HideView<ILoginView>().OnComplete(delegate
		{
			_router.ShowView<IListPlayerOnlineGamesMenuView>();
		});
	}

	public void ShowRegisterView()
	{
		_router.HideView<ILoginView>().OnComplete(delegate
		{
			_router.ShowView<IRegisterView>();
		});
	}

	public void ShowForgotPasswordView()
	{
		_router.HideView<ILoginView>().OnComplete(delegate
		{
			_router.ShowView<IPasswordRecoveryView>();
		});
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

	public void ShowQuestionViewWithError(string errorKey)
	{
		_router.GetView<IQuestionPopupView>().Setup(_localization.GetTranslation("UI/ERROR_" + errorKey), isConfirmNegative: false, _localization.GetTranslation("UI/RECOVER"), _localization.GetTranslation("UI/CANCEL"), delegate
		{
			_router.HideView<IQuestionPopupView>();
			_router.HideView<ILoginView>();
			_router.ShowView<IPasswordRecoveryView>();
		}, delegate
		{
			_router.HideView<IQuestionPopupView>();
		});
		_router.ShowView<IQuestionPopupView>();
	}

	public void HideView()
	{
		_router.HideView<ILoginView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}
}
