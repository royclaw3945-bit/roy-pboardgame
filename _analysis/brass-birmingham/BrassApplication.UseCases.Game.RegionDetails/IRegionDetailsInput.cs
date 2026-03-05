using BoardGameRules.Entities.Board.BuildingSlots;

namespace BrassApplication.UseCases.Game.RegionDetails;

public interface IRegionDetailsInput
{
	void BuildClicked(BuildingSlot buildingSlot, BuildingType buildingType);

	void BackToBoard();
}
