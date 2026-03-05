using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Industries;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Actions.IronDelivery;

namespace BrassApplication.UseCases.Game.Actions.BuildAction;

public interface IBuildActionOutput : IBaseActionOutput
{
	void HighlightValidBuildingSlots(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, ISelectBuildingDelegate selectBuildingDelegate, Action<RegionName> actionOnClick);

	void HighlightValidBuildingSlotsInRegion(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, ISelectBuildingDelegate selectBuildingDelegate, Action<RegionName> actionOnClick, RegionName regionName);

	void TurnOffSlotsHighlightning(BoardGameRules.Entities.Board.Board board);

	void ShowPlayerInfoToSelectBuildingSlot(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowValidCardsToBuild(List<BaseCard> validCardList, ISelectCardDelegate selectCardDelegate);

	void HideCards();

	void ShowRegionDetailsView(RegionName regionName, BrassApplicationGame game, IDictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons);

	void HideRegionDetailsView();

	new void HideIronDelivery();

	void HideCoalDelivery();

	void ShowPlayerInfoToSelectIndustry();

	void ShowPlayerInfoToSelectCardToDiscard();

	void StartIronDeliveryUseCase(int ironToDeliver, IIronDeliveryResultDelegate ironDeliveryResultDelegate);

	void StartCoalDeliveryUseCase(int coalToDeliver, IEnumerable<CoalMine> possibleCoalMinesSources, ICoalDeliveryResultDelegate coalDeliveryResultDelegate);

	void SetFastForwardButtonActive(bool isEnabled);
}
