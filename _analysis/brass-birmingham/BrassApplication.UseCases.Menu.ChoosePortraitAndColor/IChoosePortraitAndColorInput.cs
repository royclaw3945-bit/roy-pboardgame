using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.ChoosePortraitAndColor;

public interface IChoosePortraitAndColorInput : IUseCase
{
	void BackClicked();

	void NextColorClicked(PlayerColor color, bool isOnlineGame = false);

	void PreviousColorClicked(PlayerColor color, bool isOnlineGame = false);

	void NextPortraitClicked(PlayerAvatar avatar, bool isOnlineGame = false);

	void PreviousPortraitClicked(PlayerAvatar avatar, bool isOnlineGame = false);
}
