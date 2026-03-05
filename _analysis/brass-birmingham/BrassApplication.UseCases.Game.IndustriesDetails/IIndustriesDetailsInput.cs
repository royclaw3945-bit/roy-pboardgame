using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Game.IndustriesDetails;

public interface IIndustriesDetailsInput
{
	void LoadIndustries();

	void LoadIndustries(PlayerColor color);

	void CloseIndustries();

	void OnIndustryClicked(BuildingType buildingType);

	void SetPlayersSummaries();

	void ShowCantDevelopTooltip(float x, float y);

	void HideTooltip();
}
