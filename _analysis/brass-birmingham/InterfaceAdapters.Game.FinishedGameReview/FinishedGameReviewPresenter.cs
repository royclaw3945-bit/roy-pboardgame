using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.FinishedGameReview;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Game.EndOfGame;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Game.FinishedGameReview;

public class FinishedGameReviewPresenter : IPresenter, IFinishedGameReviewOutput
{
	private readonly IRouter _router;

	public FinishedGameReviewPresenter(IRouter router)
	{
		_router = router;
	}

	public void ReplayUnseenActions(PlayerMetadata currentPlayerMetadata, IReplayDelegate replayDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().ReplayUnseenEvents(currentPlayerMetadata, replayDelegate);
	}

	public void ShowSkipToMyNextTurnView()
	{
		if (_router.GetView<IPlayerActionsView>().IsViewVisible())
		{
			_router.HideView<IPlayerActionsView>();
		}
		_router.ShowView<IReplayView>();
	}

	public void HidePlayerActionView()
	{
		_router.HideView<IReplayView>();
	}

	public void ShowButtonToLaunchGameSummaryTable()
	{
		_router.ShowView<IFinishedGameReviewView>();
	}

	public void ShowGameSummaryTable(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromCanalEra, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place)> endOfGamePlayersScore)
	{
		_router.ShowView<IEndOfGameView>();
		_router.GetView<IEndOfGameView>().UpdateEndOfGameView(endOfGamePlayersScore);
		_router.GetView<IEndOfGameView>().SetOnReviewButtonClicked(delegate
		{
			_router.HideView<IEndOfGameView>();
		});
		_router.GetView<IEndOfGameView>().SetOnExitGameButtonClicked(delegate
		{
			_router.ShowView<ILoadSceneView>().OnComplete(delegate
			{
				_router.GetView<ILoadSceneView>().LoadScene("MenuScene");
			});
		});
	}

	public void ShowIntroductoryGameSummaryTable(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries)> endOfIntroductoryGamePlayersScore)
	{
		_router.ShowView<IEndOfGameView>();
		_router.GetView<IEndOfGameView>().UpdateEndOfIntroductoryGameView(endOfIntroductoryGamePlayersScore);
		_router.GetView<IEndOfGameView>().SetOnReviewButtonClicked(delegate
		{
			_router.HideView<IEndOfGameView>();
		});
		_router.GetView<IEndOfGameView>().SetOnExitGameButtonClicked(delegate
		{
			_router.ShowView<ILoadSceneView>().OnComplete(delegate
			{
				_router.GetView<ILoadSceneView>().LoadScene("MenuScene");
			});
		});
	}
}
