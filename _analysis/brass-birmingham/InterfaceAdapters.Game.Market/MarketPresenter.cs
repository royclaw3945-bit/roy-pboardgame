using System.Numerics;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Market;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Market;

public class MarketPresenter : IPresenter, IMarketOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public MarketPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}

	public void ShowTooltip(string element, float x, float y, int resourceCost)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		string text = "ERROR";
		Vector3 zero = Vector3.Zero;
		if (!(element == "Iron"))
		{
			if (element == "Coal")
			{
				text = string.Format(_localization.GetTranslation("Tooltips/HUD/MARKET/COAL"), resourceCost);
			}
		}
		else
		{
			text = string.Format(_localization.GetTranslation("Tooltips/HUD/MARKET/IRON"), resourceCost);
		}
		zero = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0f);
		_router.GetView<ITooltipView>().ShowTooltip(text, zero, pivot);
	}

	public void UpdateMarket(BrassApplicationGame game)
	{
		MarketViewModel marketViewModel = new MarketViewModel();
		marketViewModel.AmountOfCoal = game.Game.Board.CoalMarket.AvailableCoal;
		marketViewModel.AmountOfIron = game.Game.Board.IronMarket.AvailableIron;
		_router.GetView<IMarketView>().UpdateMarket(marketViewModel);
	}
}
