using BoardGameRules.Entities.Board.Regions;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Board;

namespace InterfaceAdapters.Tutorials.Board;

public interface ITutorialBoardView : IBoardView, IBaseView
{
	void ScrollToPosition(float x, float y, float scale);

	void EnableScrolling(bool isEnabled);

	(float x, float y) GetBuildingSlotPosition(string buildingSlotUniqueId, RegionName regionName);

	void HideAllMerchantRegions();

	void ShowRegion(RegionName regionName, float delay = 0f);
}
