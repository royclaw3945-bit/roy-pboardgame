using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.IronDelivery;

namespace BrassApplication.UseCases.Game.Actions.DevelopAction;

public interface IDevelopActionOutput : IBaseActionOutput
{
	void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, IActionInProgressFinishDelegate finishDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowPlayerInfoToSelectIndustry();

	void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate);

	void HideCards();

	void ShowIndustriesDetailsView(BrassApplicationGame game, ISelectIndustryOfTypeDelegate selectIndustryOfActionDelegate);

	void UpdateIndustriesDetailsView(BrassApplicationGame game);

	void HideIndustriesDetailsView();

	void ActivateFinishButton();

	void HideFinishButton();

	void HideIronMarketHighlight();

	void DeliverIronAndHide();

	void StartIronDeliveryUseCase(int ironToDeliver, IIronDeliveryResultDelegate ironDeliveryResultDelegate);

	void ReplayEvents(ReadOnlyCollection<IEvent> events);

	void HideNotEnoughResourcesForSecondDevelopPopup();

	void ShowNotEnoughResourcesForSecondDevelopPopup();
}
