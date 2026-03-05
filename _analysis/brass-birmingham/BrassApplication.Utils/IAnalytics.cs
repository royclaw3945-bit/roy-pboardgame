using BrassApplication.Game;

namespace BrassApplication.Utils;

public interface IAnalytics
{
	void ScreenVisit(string screenName);

	void GameStart(GameMetadata gameMetadata);

	void GameOver(GameMetadata gameMetadata);

	void LevelStart(string levelName);

	void LevelComplete(string levelName);

	void LevelFail(string levelName);

	void LevelQuit(string levelName);

	void TutorialStart(string tutorialId);

	void TutorialStep(string tutorialId, int stepId);

	void TutorialComplete(string tutorialId);

	void TutorialSkip(string tutorialId);

	void StoreOpened();

	void StoreItemClick(string itemId);

	void StoreItemPurchased(string itemId);

	void StoreItemsRestored();

	void LogCustomEvent(string eventName);
}
