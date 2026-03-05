using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.OnlineGame.JoinOnlineGame;

public interface IJoinOnlineGameInput
{
	void StartJoinProcedure(GameMetadata gameMetadata);

	void ShowChoosePortraitAndJoinView(int slot);

	void JoinGameClicked(int slot, PlayerAvatar playerAvatar);

	void JoinGame(int slot, string password);

	void BackClicked();
}
