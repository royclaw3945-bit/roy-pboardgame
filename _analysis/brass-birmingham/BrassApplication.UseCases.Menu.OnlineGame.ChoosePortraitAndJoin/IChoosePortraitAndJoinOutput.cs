using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;

public interface IChoosePortraitAndJoinOutput
{
	void HideView();

	void SetPortrait(PlayerAvatar currentAvatar);

	void SetView(PlayerAvatar currentAvatar, PlayerColor color);
}
