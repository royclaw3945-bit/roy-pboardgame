using System.Collections.Generic;
using System.Numerics;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.PlayersSummary;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.PlayersSummary;

public class PlayersSummaryPresenter : IPresenter, IPlayersSummaryOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public PlayersSummaryPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void UpdatePlayersSummaryView(BrassApplicationGame game)
	{
		List<PlayerViewModel> list = new List<PlayerViewModel>();
		foreach (Player player in game.Game.PlayersTrack.Players)
		{
			list.Add(new PlayerViewModel
			{
				IsCurrent = game.Game.GameProgressInformation.CurrentPlayer.Equals(player),
				Color = player.Color,
				Avatar = (game.Metadata.GetPlayerMetadataByColor(player.Color)?.PlayerAvatar ?? PlayerAvatar.Arkwright),
				Money = player.Money,
				VP = player.VP,
				IncomeLevel = player.IncomeLevel,
				IncomeMarkerPosition = player.IncomeMarkerPosition,
				IncomeThreshold = BoardGameRules.Entities.NumericalResources.IncomeTrack.GetIncomeThreshold(player.IncomeMarkerPosition),
				MoneySpent = game.Game.PlayersTrack.SpendMoneyInCurrentRound[player.Color],
				Name = (game.Metadata.GetPlayerMetadataByColor(player.Color)?.NickName ?? "NO METADATA")
			});
		}
		_router.GetView<IPlayersSummaryView>().UpdatePlayersSummaryView(list);
	}

	public void ShowTooltip(string element, float x, float y)
	{
		string text = "ERROR";
		Vector3 zero = Vector3.Zero;
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		switch (element)
		{
		case "VP":
			text = _localization.GetTranslation("Tooltips/HUD/PLAYERSSUMMARY/VP");
			break;
		case "CurrentMoney":
			text = _localization.GetTranslation("Tooltips/HUD/PLAYERSSUMMARY/MONEY");
			break;
		case "Income":
			text = _localization.GetTranslation("Tooltips/HUD/PLAYERSSUMMARY/INCOME");
			break;
		case "IncomeProgress":
			text = _localization.GetTranslation("Tooltips/HUD/PLAYERSSUMMARY/INCOMEPOINTS");
			break;
		case "MoneySpent":
			text = _localization.GetTranslation("Tooltips/HUD/PLAYERSSUMMARY/MONEYSPENT");
			break;
		}
		zero = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0f, 0.5f);
		_router.GetView<ITooltipView>().ShowTooltip(text, zero, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}
}
