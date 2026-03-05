using System;

namespace BrassApplication.Utils;

public interface INativePlatform
{
	string PushNotificationDeviceId { get; }

	void Login();

	void RetriveDevicePushId(Action<string> onComplete);

	void ShowAchievements();

	void UnlockAchievement(string achievementId);

	void IncrementAchievement(string achievementId, float percentageIncrement);

	float GetAchievementPercentage(string achievementId);

	void SubmitScore(string leaderboardId, long score);

	void ShowLeaderboards();

	void ShowLeaderboard(string leaderboardId);

	void ConnectToIAP();

	void RestoreIAPPurchases(Action<bool> onComplete);

	bool IsProductPurchased(string productId);

	void PurchaseIAP(string productID, Action<bool> onComplete);

	void ShowRatePopup();

	void ShareSystemDefault(string caption, string message);

	void ShareSystemDefault(string caption, string message, object texture2D);

	void SendEmail(string subject, string body, string recipients);

	bool DidUserSeenQuestionAboutPush();

	bool DidRateOrDeclineToRateTheApp();
}
