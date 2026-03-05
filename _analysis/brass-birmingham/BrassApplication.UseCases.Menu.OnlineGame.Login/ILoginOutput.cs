namespace BrassApplication.UseCases.Menu.OnlineGame.Login;

public interface ILoginOutput
{
	void ShowConnectionView();

	void HideConnectionView();

	void ShowOnlineGamesView();

	void ShowRegisterView();

	void ShowForgotPasswordView();

	void ShowInfoViewWithError(string errorKey);

	void ShowQuestionViewWithError(string errorKey);

	void HideView();
}
