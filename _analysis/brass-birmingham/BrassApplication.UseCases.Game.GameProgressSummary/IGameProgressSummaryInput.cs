namespace BrassApplication.UseCases.Game.GameProgressSummary;

public interface IGameProgressSummaryInput
{
	void UpdateGameProgressSummary();

	void EarningsTrackClicked();

	void GameLogClicked();

	void ShowTooltip(string element, float x, float y);

	void HideTooltip();
}
