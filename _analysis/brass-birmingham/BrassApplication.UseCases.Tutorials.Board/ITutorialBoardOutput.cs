using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BrassApplication.UseCases.Game.Board;

namespace BrassApplication.UseCases.Tutorials.Board;

public interface ITutorialBoardOutput : IBoardOutput
{
	void HighlightOnlySpecificSlot(BoardGameRules.Entities.Board.Board board, GameEra currentEra, Region specificRegion, Action<RegionName> actionOnClick, string slotUniqueId);

	void SetAllowedIndustries(List<BuildingType> allowedIndustries);
}
