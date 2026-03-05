using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.PlayersSummary;

namespace InterfaceAdapters.Game.IndustriesDetails;

public interface IIndustriesDetailsView : IBaseView
{
	void UpdateIndustriesDetails(IndustriesDetailsViewModel industriesDetailsViewModel, LinkCountViewModel linkCountViewModel);

	void HighlightIndustriesToDevelop(IndustriesDetailsViewModel industriesDetailsViewModel);

	void SetAsMerchantBonus();

	void SetPlayerSummary(List<PlayerViewModel> playerViewModels);

	IAnimation CreateIndustryDevelopedAnimation(BuildingType buildingType, float delay = 0f);
}
