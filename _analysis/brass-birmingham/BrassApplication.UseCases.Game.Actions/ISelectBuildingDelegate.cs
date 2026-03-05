using BoardGameRules.Entities.Board.BuildingSlots;

namespace BrassApplication.UseCases.Game.Actions;

public interface ISelectBuildingDelegate
{
	void SelectBuilding(BuildingSlot buildingSlot, BuildingType buildingType);
}
