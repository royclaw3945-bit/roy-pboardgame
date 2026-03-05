using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.Game;
using BrassApplication.Utils;
using UnityEngine.Analytics;

namespace Frameworks.Analytics;

public class UnityAnalytics : IAnalytics
{
	private IAnalytics _analyticsImplementation;

	public void ScreenVisit(string screenName)
	{
		AnalyticsEvent.ScreenVisit(screenName);
	}

	public void GameStart(GameMetadata gameMetadata)
	{
		AnalyticsEvent.GameStart(new Dictionary<string, object>
		{
			{ "GameType", gameMetadata.GameType },
			{
				"NumberOfPlayers",
				gameMetadata.GameStartConfiguration.NumberOfPlayers
			}
		});
	}

	public void GameOver(GameMetadata gameMetadata)
	{
		PlayerColor playerColor = gameMetadata.GameProgressInformation.EndGameRanking.First();
		PlayerMetadata playerMetadataByColor = gameMetadata.GetPlayerMetadataByColor(playerColor);
		Dictionary<string, object> eventData = new Dictionary<string, object>
		{
			{
				"IsWinnerAI",
				playerMetadataByColor.AiType != AIType.Human
			},
			{ "AIType", playerMetadataByColor.AiType },
			{ "GameType", gameMetadata.GameType },
			{
				"WinnerScore",
				gameMetadata.GameProgressInformation.EndGamePoints[playerColor].totalVP
			}
		};
		AnalyticsEvent.GameOver(null, eventData);
	}

	public void LevelStart(string levelName)
	{
		AnalyticsEvent.LevelStart(levelName);
	}

	public void LevelComplete(string levelName)
	{
		AnalyticsEvent.LevelComplete(levelName);
	}

	public void LevelFail(string levelName)
	{
		AnalyticsEvent.LevelFail(levelName);
	}

	public void LevelQuit(string levelName)
	{
		AnalyticsEvent.LevelQuit(levelName);
	}

	public void TutorialStart(string tutorialId)
	{
		AnalyticsEvent.TutorialStart(tutorialId);
	}

	public void TutorialStep(string tutorialId, int stepId)
	{
		AnalyticsEvent.TutorialStep(stepId, tutorialId);
	}

	public void TutorialComplete(string tutorialId)
	{
		AnalyticsEvent.TutorialComplete(tutorialId);
	}

	public void TutorialSkip(string tutorialId)
	{
		AnalyticsEvent.TutorialSkip(tutorialId);
	}

	public void StoreOpened()
	{
		AnalyticsEvent.StoreOpened(StoreType.Premium);
	}

	public void StoreItemClick(string itemId)
	{
		AnalyticsEvent.StoreItemClick(StoreType.Premium, itemId);
	}

	public void StoreItemPurchased(string itemId)
	{
		AnalyticsEvent.IAPTransaction("", 0f, itemId);
	}

	public void StoreItemsRestored()
	{
		AnalyticsEvent.Custom("store_items_restored");
	}

	public void LogCustomEvent(string eventName)
	{
		AnalyticsEvent.Custom(eventName);
	}
}
