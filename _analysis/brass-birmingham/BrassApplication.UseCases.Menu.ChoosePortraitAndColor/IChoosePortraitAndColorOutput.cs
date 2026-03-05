using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Menu.ChoosePortraitAndColor;

public interface IChoosePortraitAndColorOutput
{
	void HideView();

	void NextColorFor(PlayerColor startingColor, PlayerColor color, bool isOnlineGame = false);

	void PreviousColorFor(PlayerColor startingColor, PlayerColor color, bool isOnlineGame = false);

	void NextPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar avatar, bool isOnlineGame = false);

	void PreviousPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar avatar, bool isOnlineGame = false);
}
