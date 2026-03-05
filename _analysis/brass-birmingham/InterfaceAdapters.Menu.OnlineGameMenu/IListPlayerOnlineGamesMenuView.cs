using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.OnlineGameMenu;

public interface IListPlayerOnlineGamesMenuView : IBaseView
{
	void UpdatePlayerGamesView(ListPlayerOnlineGamesMenuViewModel viewModel);

	void UpdateOpenGamesView(ListPlayerOnlineGamesMenuViewModel viewModel);

	void UpdateFiltersVisuals(bool show2Players, bool show3Players, bool show4Players, bool showPrivate);
}
