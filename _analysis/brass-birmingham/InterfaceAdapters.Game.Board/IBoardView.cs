using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Actions;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Board;

public interface IBoardView : IBaseView
{
	void UpdateBoardView(BoardViewModel boardViewModel);

	void HighlightBuildingSlots(BoardViewModel boardViewModel);

	void HighlightMerchantSlots(IDictionary<RegionName, BorderRegionViewModel> borderRegions);

	void HighlightLinks(IDictionary<string, LinkViewModel> links, ILinkSelectedDelegate linkSelectedDelegate);

	void ScrollToRegion(RegionName region);

	void TurnOffRegionsActionOnClick();

	object GetRegionViewForGivenRegionName(RegionName regionName);

	IAnimation CreateLinkPlacedAnimation(string linkId, PlayerColor playerColor);

	IAnimation CreateIndustryBuiltAnimation(BuildingSlot slot, BaseIndustry industry, PlayerColor playerColor);

	IAnimation CreateResourceSoldAnimation(string slotUniqueId, int amount, string type, BuildingSlot buildingSlot);

	IAnimation CreateIncomeMarkerPositionChangedAnimation(int amount, string uniqueId);

	IAnimation CreateIndustryFlippedAnimation(BuildingSlot buildingSlot);

	IAnimation CreateBeerSuppliedFromMerchantAnimation(string merchantUniqueId, BorderRegion borderRegion);

	IAnimation CreateRemovePlayerLinksAnimation(PlayerColor player);

	IAnimation CreatePointPlayerFlippedIndustriesAnimation(PlayerColor player);

	IAnimation CreatePointPlayerFlippedLvlTwoPlusIndustriesAnimation(PlayerColor player);

	IAnimation CreateRemovePlayerLvlOneIndustriesAnimation(PlayerColor player);

	void AllowRegionInteractions(bool isRaycastTarget);

	void ZoomOutToWholeMap(GameEra? era = null);
}
