using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;

namespace BrassApplication.UseCases.Game.Actions.SellAction;

public interface ISellActionOutput : IBaseActionOutput
{
	void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate);

	void HideCards();

	void HighlightValidBuildingSlots(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWithIndustriesToSell, ISelectIndustryDelegate selectIndustryDelegate);

	void ShowPlayerInfoToSelectIndustry(IActionInProgressCancelDelegate cancelActionDelegate);

	void ShowNextSellChoicePanel(List<BuildingSlot> buildingSlotsWithIndustriesToSell);

	void HideNextSellChoicePanel();

	void StartBeerDeliveryUseCase(int beerToDeliver, IEnumerable<IBeerSource> possibleBeerSources, IBeerDeliveryResultDelegate beerDeliveryResultDelegate);

	void ShowMerchantDevelopBonus(BrassApplicationGame game, ISelectIndustryOfTypeDelegate selectIndustryOfActionDelegate);

	void HideIndustriesDetailsView();

	void SetFastForwardButtonActive(bool isEnabled);
}
