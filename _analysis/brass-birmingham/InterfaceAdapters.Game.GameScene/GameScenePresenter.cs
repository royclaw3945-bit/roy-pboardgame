using System.Numerics;
using BrassApplication.UseCases.Game.GameScene;
using BrassApplication.UseCases.Game.PlayGame;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.ExitToMenu;
using InterfaceAdapters.Game.GameProgressSummary;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.Market;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.GameScene;

public class GameScenePresenter : IPresenter, IGameSceneOutput
{
	protected readonly IRouter _router;

	private readonly ILocalization _localization;

	public GameScenePresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public virtual void ShowGamePanel()
	{
		_router.ShowView<IGameSceneView>();
	}

	public void ShowAndHideLoadingView()
	{
		_router.ShowView<ILoadSceneView>().OnComplete(delegate
		{
			_router.HideView<ILoadSceneView>().OnComplete(delegate
			{
				_router.ShowView<IGameSceneView>();
			});
		});
	}

	public void ShowLoadingView()
	{
		_router.ShowView<ILoadSceneView>();
	}

	public void ShowExitMenuView()
	{
		_router.ShowView<IExitToMenuView>();
	}

	public void ShowBoard()
	{
		_router.ShowView<IBoardView>();
	}

	public void ShowCards()
	{
		_router.ShowView<IHandOfCardsView>();
	}

	public void ShowMarket()
	{
		_router.ShowView<IMarketView>();
	}

	public void ShowPlayersSummary()
	{
		_router.ShowView<IPlayersSummaryView>();
	}

	public void ShowPlayerActions()
	{
		_router.ShowView<IPlayerActionsView>();
	}

	public void ShowIndustriesSummary()
	{
		_router.ShowView<IIndustriesSummaryView>();
	}

	public void ShowGameProgressSummary()
	{
		_router.ShowView<IGameProgressSummaryView>();
	}

	public void PlayGame()
	{
		_router.UseCaseBuilder.GetUseCase<PlayGameUseCase>().PlayGame();
	}

	public void ShowBackToMenuTooltip(float x, float y)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		Vector3 position = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0.5f);
		_router.GetView<ITooltipView>().ShowTooltip(_localization.GetTranslation("Tooltips/MENUBUTTON"), position, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}
}
