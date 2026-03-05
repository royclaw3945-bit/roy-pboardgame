namespace BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;

public interface ICreateOnlineGameInput
{
	void CreateNewGameConfiguration();

	void StartGame();

	void ChangeGameName(string name);

	void ChangeTime();

	void ChangePassword();

	void Back();
}
