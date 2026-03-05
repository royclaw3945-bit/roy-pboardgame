using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Game.PlayersSummary;

public interface IPlayersSummaryInput
{
	void UpdatePlayers();

	void PlayerSelected(PlayerColor playerColor);

	void ShowTooltip(string element, float x, float y);

	void OnMouseExit();
}
