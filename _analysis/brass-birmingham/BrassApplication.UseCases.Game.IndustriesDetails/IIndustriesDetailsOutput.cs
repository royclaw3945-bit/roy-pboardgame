using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.IndustriesDetails;

public interface IIndustriesDetailsOutput
{
	void UpdateIndustriesDetails(BrassApplicationGame game);

	void UpdateIndustriesDetails(BrassApplicationGame game, PlayerColor color);

	void SetPlayerSummary(BrassApplicationGame game);

	void HideIndustriesDetails();

	void ShowCantDevelopTooltip(float x, float y);

	void HideTooltip();
}
