using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;

namespace BrassApplication.UseCases.Game.HandOfCards;

public interface IHandOfCardsOutput
{
	void UpdateHandCardsView(List<BaseCard> cards, bool shouldHideCards);

	void OnLocationCardClicked(RegionName region, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, BoardGameRules.Entities.Board.Board board);

	string GetRegionFriendlyName(string regionName);
}
