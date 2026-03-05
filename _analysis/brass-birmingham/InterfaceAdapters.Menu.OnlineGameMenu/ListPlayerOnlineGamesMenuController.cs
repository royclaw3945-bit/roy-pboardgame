using BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.OnlineGameMenu;

public class ListPlayerOnlineGamesMenuController : IController
{
	private readonly IListPlayerOnlineGamesMenuInput _listPlayerOnlineGamesMenuUseCase;

	public ListPlayerOnlineGamesMenuController(IListPlayerOnlineGamesMenuInput listPlayerOnlineGamesMenuUseCase)
	{
		_listPlayerOnlineGamesMenuUseCase = listPlayerOnlineGamesMenuUseCase;
	}

	public void OnViewCreated()
	{
		_listPlayerOnlineGamesMenuUseCase.OnViewCreated();
	}

	public void OnViewDestroyed()
	{
		_listPlayerOnlineGamesMenuUseCase.OnViewDestroyed();
	}

	public void LogoutClicked()
	{
		_listPlayerOnlineGamesMenuUseCase.LogoutClicked();
	}

	public void CrateGameClicked()
	{
		_listPlayerOnlineGamesMenuUseCase.CreateGameClicked();
	}

	public void BackClicked()
	{
		_listPlayerOnlineGamesMenuUseCase.BackClicked();
	}

	public void UpdateFilters(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		_listPlayerOnlineGamesMenuUseCase.SetFilters(show2Players, show3Players, show4Players, showPrivate);
		_listPlayerOnlineGamesMenuUseCase.UpdateView();
	}
}
