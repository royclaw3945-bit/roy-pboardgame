using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;

public interface ICreateOfflineGameOutput
{
	void PresentGameConfiguration(IEnumerable<PlayerMetadata> playerMetadata, Action<PlayerMetadata> onNextDifficultyClickedAction, Action<PlayerMetadata> onPreviousDifficultyClickedAction, Action<PlayerMetadata> onEditPortraitAndColorAction, Action<PlayerColor, string> onPlayerNameChangedAction, Action increaseNumberOfPlayers, Action decreaseNumberOfPlayers, bool canStartGame);

	void LaunchGame(BrassApplicationGame game);

	void HideView();

	void ShowChoosePortraitAndColorView(PlayerMetadata playerMetadata);

	void UpdatePortraitAndColorView(PlayerMetadata playerMetadata);

	void ShowExistingGameContainer(BrassApplicationGame game);

	void HideExistingGameContainer();

	void GameLoaded(BrassApplicationGame brassApplicationGame);

	void ShowIntroductoryGameTooltip(float x, float y);

	void HideTooltip();

	void AskOverwriteExistingGame(Func<BrassApplicationGame> startGameAction);
}
