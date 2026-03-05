using BrassApplication.Game;
using BrassApplication.UseCases.Game.PlayHumanTurn;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Game.PlayHumanTurn;

public class PlayHumanTurnPresenter : IPresenter, IPlayHumanTurnOutput
{
	private readonly IRouter _router;

	public PlayHumanTurnPresenter(IRouter router)
	{
		_router = router;
	}

	public void ReplayUnseenActions(PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().ReplayUnseenEvents(replayWatchingPlayerMetadata, replayDelegate);
	}

	public void ShowSkipToMyNextTurnView()
	{
		if (_router.IsViewVisible<IPlayerActionsView>())
		{
			_router.HideView<IPlayerActionsView>();
		}
		_router.ShowView<IReplayView>();
	}

	public void RestorePlayerActionsView()
	{
		_router.HideView<IReplayView>();
		_router.ShowView<IPlayerActionsView>();
		if (_router.IsViewVisible<IReplaySummaryView>())
		{
			_router.GetView<IReplaySummaryView>().MoveToTop();
		}
		_router.GetView<IPlayerActionsView>().HideEndTurnButton();
	}
}
