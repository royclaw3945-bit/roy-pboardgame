namespace BrassApplication.UseCases.Menu.OnlineGame.PasswordRecovery;

public interface IPasswordRecoveryInput
{
	void SendEmailClicked(string email);

	void BackClicked();
}
