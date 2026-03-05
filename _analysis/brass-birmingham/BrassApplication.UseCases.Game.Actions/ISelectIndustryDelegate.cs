using BoardGameRules.Entities.Board.BuildingSlots;

namespace BrassApplication.UseCases.Game.Actions;

public interface ISelectIndustryDelegate
{
	void SelectIndustrySlot(BuildingSlot buildingSlot);
}
