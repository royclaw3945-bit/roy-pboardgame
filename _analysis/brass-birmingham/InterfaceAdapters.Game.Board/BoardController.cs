using BoardGameRules.Entities.Board.Regions;
using BrassApplication.UseCases.Game.Board;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Board;

public class BoardController : IController
{
	private readonly IBoardInput _boardInput;

	public BoardController(IBoardInput boardInput)
	{
		_boardInput = boardInput;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnRegionClicked(RegionName regionName)
	{
		_boardInput.RegionSelected(regionName);
	}

	public int GetPointsForLink(string linkId)
	{
		return _boardInput.GetPointsForLink(linkId);
	}

	public void ShowTooltipIndustry(string element, float x, float y, int width, int height)
	{
		_boardInput.ShowTooltipIndustry(element, x, y, width, height);
	}

	public void ShowTooltipLink(string linkUniqueId, float x, float y, int width, int height)
	{
		_boardInput.ShowTooltipLink(linkUniqueId, x, y, width, height);
	}

	public void ShowTooltipBorderRegion(RegionName regionName, float x, float y, int width, int height)
	{
		_boardInput.ShowTooltipBorderRegion(regionName, x, y, width, height);
	}

	public void HideTooltip()
	{
		_boardInput.HideTooltip();
	}

	public bool GetIsNightMap()
	{
		return _boardInput.GetIsNightMap();
	}
}
