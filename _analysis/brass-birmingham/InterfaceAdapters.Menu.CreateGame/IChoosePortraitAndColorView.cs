using BoardGameRules.Entities.Players;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.CreateGame;

public interface IChoosePortraitAndColorView : IBaseView
{
	void UpdateView(PlayerColor playerColor, PlayerAvatar playerAvatar, bool isOnlineGame = false);

	void UpdateColor(PlayerColor playerColor);

	void UpdateAvatar(PlayerAvatar playerAvatar);
}
