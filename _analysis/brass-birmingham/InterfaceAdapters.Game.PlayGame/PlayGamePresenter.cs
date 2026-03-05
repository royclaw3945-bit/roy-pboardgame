using System.Threading.Tasks;
using BrassApplication.Game;
using BrassApplication.UseCases.Common.SaveGame;
using BrassApplication.UseCases.Game.FinishedGameReview;
using BrassApplication.UseCases.Game.PlayAiTurn;
using BrassApplication.UseCases.Game.PlayGame;
using BrassApplication.UseCases.Game.PlayHumanTurn;
using BrassApplication.UseCases.Game.PlaySpectatorTurn;
using BrassApplication.UseCases.Game.SellIndustryFlow;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayAiTurn;
using InterfaceAdapters.Game.PlaySpectatorTurn;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Game.PlayGame;

public class PlayGamePresenter : IPresenter, IPlayGameOutput
{
	private readonly IRouter _router;

	public PlayGamePresenter(IRouter router)
	{
		_router = router;
	}

	public void StartHumanTurn()
	{
		if (_router.IsViewVisible<IPlayAiTurnView>())
		{
			_router.HideView<IPlayAiTurnView>();
		}
		_router.UseCaseBuilder.GetUseCase<PlayHumanTurnUseCase>().StartHumanTurn();
	}

	public async void StartAITurn(PlayerMetadata playerMetadata)
	{
		if (_router.IsViewVisible<IPlayerActionsView>())
		{
			_router.HideView<IPlayerActionsView>();
		}
		if (!_router.IsViewVisible<IPlayAiTurnView>())
		{
			await Task.Delay(100);
			_router.ShowView<IPlayAiTurnView>();
		}
		PlayAiTurnUseCase useCase = _router.UseCaseBuilder.GetUseCase<PlayAiTurnUseCase>();
		_router.GetView<IPlayAiTurnView>().SetCurrentPlayerName(playerMetadata.NickName);
		useCase.PlayAITurn();
	}

	public void StartNetworkHumanTurn()
	{
		if (_router.IsViewVisible<IPlaySpectatorTurnView>())
		{
			_router.HideView<IPlaySpectatorTurnView>();
		}
		_router.UseCaseBuilder.GetUseCase<PlayHumanTurnUseCase>().StartHumanTurn();
	}

	public void StartNetworkSpectatorTurn(PlayerMetadata replayWatchingPlayerMetadata)
	{
		if (!_router.IsViewVisible<IPlaySpectatorTurnView>())
		{
			_router.ShowView<IPlaySpectatorTurnView>();
		}
		if (_router.IsViewVisible<IPlayerActionsView>())
		{
			_router.HideView<IPlayerActionsView>();
		}
		_router.UseCaseBuilder.GetUseCase<PlaySpectatorTurnUseCase>().StartSpectatorTurn(replayWatchingPlayerMetadata);
	}

	public void StartSellingIndustries()
	{
		_router.UseCaseBuilder.GetUseCase<SellIndustryFlowUseCase>().StartSellIndustriesFlow();
	}

	public void SaveGame(BrassApplicationGame game, ISaveGameDelegate saveGameDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<SaveGameUseCase>().SaveGame(game, saveGameDelegate);
	}

	public void StartGameReview(PlayerMetadata localPlayerMetadata)
	{
		if (_router.IsViewVisible<IPlayAiTurnView>())
		{
			_router.HideView<IPlayAiTurnView>();
		}
		_router.UseCaseBuilder.GetUseCase<FinishedGameReviewUseCase>().StartGameReview(localPlayerMetadata);
	}
}
