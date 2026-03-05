using BoardGameRules.Entities.Board.Regions;
using BrassApplication.UseCases.Game.Actions;

namespace BrassApplication.UseCases.Game.Board;

public interface IBoardInput : IActionInProgressCancelDelegate
{
	void UpdateBoard();

	void RegionSelected(RegionName regionName);

	int GetPointsForLink(string linkId);

	void ShowTooltipIndustry(string industryUniqueId, float x, float y, int width, int height);

	void ShowTooltipLink(string linkId, float x, float y, int width, int height);

	void ShowTooltipBorderRegion(RegionName regionName, float x, float y, int width, int height);

	void HideTooltip();

	bool GetIsNightMap();
}
