using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Markets;

namespace BrassApplication.UseCases.Game.Actions.IronDelivery;

public interface IIronDeliveryOutput
{
	void HighlightMarket(IronMarket ironMarket, ISelectMarketDelegate selectMarketDelegate, int ironToDeliver);

	void HighlightIronWorks(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsToHighlight, ISelectIndustryDelegate selectIndustrySlotDelegate);

	void ShowPlayerInfoToSelectIronMarket();

	void ShowPlayerInfoToSelectIronWorks();

	void HideMarketHighlights();

	void TakeMarketResource();

	void ReplayEvents(ReadOnlyCollection<IEvent> events);
}
