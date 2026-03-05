using BoardGameRules.Entities.Players;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.ChoosePortraitAndJoin;

public interface IChoosePortraitAndJoinView : IBaseView
{
	void UpdateView(PlayerColor playerColor, PlayerAvatar playerAvatar);

	void UpdateColor(PlayerColor playerColor);

	void UpdateAvatar(PlayerAvatar playerAvatar);
}
