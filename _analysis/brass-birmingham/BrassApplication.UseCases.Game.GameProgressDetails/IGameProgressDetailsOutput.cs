using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.GameProgressDetails;

public interface IGameProgressDetailsOutput
{
	void UpdateGameProgressDetailsView(BrassApplicationGame game);

	void HideGameProgressDetailsView();

	void ShowTooltip(string element, float x, float y, string playerName, PlayerColor? playerColor);

	void HideTooltip();
}
