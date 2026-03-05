namespace BrassApplication.UseCases.Menu.OnlineGame.PasswordRecovery;

public interface IPasswordRecoveryOutput
{
	void ShowConnectionView();

	void HideConnectionView();

	void ShowInfoViewEmailSendSuccessfully();

	void ShowInfoViewWithError(string errorKey);

	void HideView();
}
