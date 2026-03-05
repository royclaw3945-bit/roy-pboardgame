using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.EventSourcing.Events;

namespace BrassApplication.UseCases.Game.Actions.BeerDelivery;

public interface IBeerDeliveryOutput
{
	void HighlightBreweries(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsToHighlight, ISelectIndustryDelegate selectIndustrySlotDelegate);

	void HighlightMerchantSlots(BoardGameRules.Entities.Board.Board board, IEnumerable<MerchantSlot> merchantSlotsToHighlight, ISelectMerchantSlotDelegate selectMerchantSlotDelegate);

	void ShowPlayerInfoToSelectBrewery();

	void ReplayEvents(ReadOnlyCollection<IEvent> events);
}
