using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Game.GameProgressDetails;

public interface IGameProgressDetailsInput
{
	void UpdateGameProgressDetails();

	void OnBackButtonClicked();

	void ShowTooltip(string element, float x, float y, PlayerColor? playerColor);

	void HideTooltip();
}
