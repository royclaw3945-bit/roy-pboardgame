using BoardGameRules.Entities.Board.BuildingSlots;

namespace BrassApplication.UseCases.Game.Actions;

public interface ISelectIndustryOfTypeDelegate
{
	void SelectIndustryOfType(BuildingType buildingType);
}
