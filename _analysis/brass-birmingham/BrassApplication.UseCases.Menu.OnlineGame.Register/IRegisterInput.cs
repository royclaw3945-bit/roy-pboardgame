namespace BrassApplication.UseCases.Menu.OnlineGame.Register;

public interface IRegisterInput
{
	void RegisterClicked(string email, string nickname, string password, string confirmPassword);

	void BackClicked();
}
