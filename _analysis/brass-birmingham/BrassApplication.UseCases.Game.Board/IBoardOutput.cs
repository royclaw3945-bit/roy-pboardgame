using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.Board;

public interface IBoardOutput
{
	void UpdateBoardView(BoardGameRules.Entities.Board.Board board, GameEra currentEra, Action<RegionName> actionOnClick);

	void HideRegionDetailsView();

	void ShowRegionDetailsView(RegionName regionName, BrassApplicationGame game, IDictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons);

	void ShowTooltipBuildingSlot(List<BuildingType> buildingTypes, string region, RegionColor regionColor, float x, float y, int width, int height);

	void ShowTooltipIndustry(BuildingType buildingType, bool activated, int level, int VP, int linkVP, int incomePoints, string owner, PlayerColor playerColor, float x, float y, int width, int height, int resources = -1, int maxResources = -1, int beerNeeded = -1);

	void ShowTooltipLink(string owner, PlayerColor? playerColor, int pointsForLink, Dictionary<string, RegionColor> regionsConnected, string linkType, float x, float y, int width, int height);

	void ShowTooltipBorderRegion(string regionName, List<BuildingType> sellableIndustries, BonusType bonus, float x, float y, int width, int height);

	void HideTooltip();

	void StartBuildActionFromBoard(RegionName regionName);

	string GetRegionFriendlyNameLocalization(RegionName regionName);
}
