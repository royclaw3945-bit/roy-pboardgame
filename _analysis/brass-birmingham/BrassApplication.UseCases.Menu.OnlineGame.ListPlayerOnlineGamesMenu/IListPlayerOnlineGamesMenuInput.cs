namespace BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;

public interface IListPlayerOnlineGamesMenuInput
{
	void UpdateView();

	void SetFilters(bool show2Players, bool show3Players, bool show4Players, bool showPrivate);

	void LogoutClicked();

	void CreateGameClicked();

	void BackClicked();

	void JoinGameClicked(string gameId);

	void OnViewDestroyed();

	void OnViewCreated();
}
