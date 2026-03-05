using BrassApplication.Game;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.IndustriesDetails;
using BrassApplication.UseCases.Game.IndustriesSummary;
using BrassApplication.UseCases.Game.PlayGame;
using BrassApplication.UseCases.Game.PlaySpectatorTurn;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace InterfaceAdapters.Game.PlaySpectatorTurn;

public class PlaySpectatorTurnPresenter : IPresenter, IPlaySpectatorTurnOutput
{
	private readonly IRouter _router;

	private readonly IMainThreadDispatcher _dispatcher;

	public PlaySpectatorTurnPresenter(IRouter router, IMainThreadDispatcher dispatcher)
	{
		_router = router;
		_dispatcher = dispatcher;
	}

	public void ShowSkipToLatestTurnView()
	{
		if (_router.IsViewVisible<IPlaySpectatorTurnView>())
		{
			_router.HideView<IPlaySpectatorTurnView>();
		}
		_router.ShowView<IReplayView>();
	}

	public void ShowWaitingForOnlinePlayerView(PlayerMetadata playerMetadata)
	{
		_router.HideView<IReplayView>();
		_router.ShowView<IPlaySpectatorTurnView>();
		_router.GetView<IPlaySpectatorTurnView>().SetCurrentPlayerName(playerMetadata.NickName);
		if (_router.IsViewVisible<IReplaySummaryView>())
		{
			_router.GetView<IReplaySummaryView>().MoveToTop();
		}
	}

	public void PlayGame()
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.UseCaseBuilder.GetUseCase<PlayGameUseCase>().PlayGame();
		});
	}

	public void ReplayUnseenActions(PlayerMetadata replayWatchingPlayerMetadata, PlaySpectatorTurnUseCase replayDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().ReplayUnseenEvents(replayWatchingPlayerMetadata, replayDelegate);
	}

	public void DisableBuildingFromTheBoard()
	{
		_router.UseCaseBuilder.GetUseCase<BoardUseCase>().SetAllowBuidlingFromBoard(allowBuildingFromBoard: false);
	}

	public void EnableBuildingFromTheBoard()
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.UseCaseBuilder.GetUseCase<BoardUseCase>().SetAllowBuidlingFromBoard(allowBuildingFromBoard: true);
		});
	}

	public void ShowSpectatorCards(PlayerMetadata replayWatchingPlayerMetadata)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().UpdateCardsForPlayer(replayWatchingPlayerMetadata);
	}

	public void ShowSpectatorIndustries(PlayerMetadata replayWatchingPlayerMetadata)
	{
		_router.UseCaseBuilder.GetUseCase<IndustriesSummaryUseCase>().UpdateIndustries(replayWatchingPlayerMetadata.PlayerStartConfiguration.Color);
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().LoadIndustries(replayWatchingPlayerMetadata.PlayerStartConfiguration.Color);
	}
}
