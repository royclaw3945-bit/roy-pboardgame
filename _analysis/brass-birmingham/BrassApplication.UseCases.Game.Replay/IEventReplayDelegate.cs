using BoardGameRules.Entities.EventSourcing.Events;

namespace BrassApplication.UseCases.Game.Replay;

public interface IEventReplayDelegate
{
	void EventReplayCompleted(IEvent eventToReplay);
}
