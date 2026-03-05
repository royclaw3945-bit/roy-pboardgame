using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Industries;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;

namespace BrassApplication.UseCases.Game.Actions.NetworkAction;

public interface INetworkActionOutput : IBaseActionOutput
{
	void HighlightValidConnections(BrassApplicationGame game, List<Link> linksThatCanBePlaced, ILinkSelectedDelegate linkSelectedDelegate);

	void ShowAnotherTrackSelectionPanel(List<CantUseLinkActionReason> reasons);

	void HideNumberOfTracksToBuildSelectionPanel();

	void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate);

	void HideCards();

	void HideCoalDelivery();

	void HideBeerDelivery();

	void ShowActionInProgressView(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowPlayerInfoToSelectCardToDiscard();

	void ShowPlayerInfoToSelectLinks();

	void ShowPlayerInfoToSelectIfAnotherTrackShouldBePlaced();

	void AllowRegionInteractions(bool isRaycastTarget);

	void StartBeerDeliveryUseCase(int beerToDeliver, IEnumerable<Brewery> possibleBrewerySources, IBeerDeliveryResultDelegate beerDeliveryResultDelegate);

	void StartCoalDeliveryUseCase(int coalToDeliver, IEnumerable<CoalMine> possibleCoalMinesSources, ICoalDeliveryResultDelegate coalDeliveryResultDelegate);

	void ReplayEvents(ReadOnlyCollection<IEvent> events);

	void SetFastForwardButtonActive(bool isEnabled);
}
