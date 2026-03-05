using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.BaseAction;

public interface IBaseActionOutput
{
	void HideActionInProgressView();

	void ShowPlayerActionsButtons();

	void UpdateGameStatus(PlayerMetadata replayWatchingPlayerMetadata);

	void PresentEvents(ReadOnlyCollection<IEvent> events, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate);

	void RestoreCardsView();

	void ShowEndTurnButton();

	void HideReplayView();

	Task WaitForTweensEnd();

	void HideIronDelivery();
}
