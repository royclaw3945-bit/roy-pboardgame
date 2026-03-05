namespace BrassApplication.UseCases.Menu.OnlineGame.Register;

public interface IRegisterOutput
{
	void ShowConnectionView();

	void HideConnectionView();

	void ShowInfoViewRegisterSuccessful();

	void ShowInfoViewWithError(string errorKey);

	void HideView();
}
