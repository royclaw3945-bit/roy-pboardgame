using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.PlayersSummary;

public interface IPlayersSummaryOutput
{
	void UpdatePlayersSummaryView(BrassApplicationGame game);

	void ShowTooltip(string element, float x, float y);

	void HideTooltip();
}
