using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Markets;

namespace BrassApplication.UseCases.Game.Actions.CoalDelivery;

public interface ICoalDeliveryOutput
{
	void HighlightMarket(CoalMarket coalMarket, ISelectMarketDelegate selectMarketDelegate, int coalToDeliver);

	void HighlightCoalMines(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsToHighlight, ISelectIndustryDelegate selectIndustrySlotDelegate);

	void ShowPlayerInfoToSelectCoalMarket();

	void ShowPlayerInfoToSelectCoalMine();

	void HideMarketHighlights();

	void TakeMarketResource();

	void ReplayEvents(ReadOnlyCollection<IEvent> events);
}
