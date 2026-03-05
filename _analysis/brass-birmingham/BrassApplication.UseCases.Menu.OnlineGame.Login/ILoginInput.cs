namespace BrassApplication.UseCases.Menu.OnlineGame.Login;

public interface ILoginInput
{
	void LoginClicked(string email, string password);

	void RegisterClicked();

	void ForgotPasswordClicked();

	void BackClicked();
}
