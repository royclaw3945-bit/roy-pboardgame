using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.GameProgressSummary;

public interface IGameProgressSummaryOutput
{
	void UpdateGameProgressSummaryView(BrassApplicationGame game);

	void ShowGameProgressDetails();

	void ShowGameLog();

	void ShowTooltip(string element, float x, float y);

	void HideTooltip();
}
