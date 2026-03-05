using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.LoanAction;
using BrassApplication.UseCases.Game.HandOfCards;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.LoanAction;

public class LoanActionPresenter : BaseActionPresenter, ILoanActionOutput, IBaseActionOutput
{
	public LoanActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount)
	{
		_router.HideView<IPlayerActionsView>();
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_CARD_TO_DISCARD"));
		_router.GetView<IActionInProgressView>().UpdateActionCounter(currentActionNumber, actionsMaxCount);
		_router.ShowView<IActionInProgressView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
	}

	public void ShowPlayerInfoToTakeLoan(IActionInProgressCancelDelegate cancelActionDelegate)
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/TAKE_LOAN"));
		_router.MoveOnTop<IActionInProgressView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
	}

	public void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(selectCardDelegate);
		_router.GetView<IHandOfCardsView>().ShowCards();
	}

	public void HideCards()
	{
		_router.GetView<IHandOfCardsView>().HideCards();
	}

	public void ShowIncomePanel(BrassApplicationGame game)
	{
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		int num = BoardGameRules.Entities.NumericalResources.IncomeTrack.DecreaseIncomeLevelBy(currentPlayer.IncomeMarkerPosition, 3);
		LoanActionViewModel loanActionViewModel = new LoanActionViewModel
		{
			PlayerColor = currentPlayer.Color,
			PlayerAvatar = game.Metadata.GetPlayerMetadataByColor(currentPlayer.Color).PlayerAvatar,
			CurrentIncomeLevel = currentPlayer.IncomeLevel,
			CurrentIncomePosition = currentPlayer.IncomeMarkerPosition,
			TargetIncomeLevel = BoardGameRules.Entities.NumericalResources.IncomeTrack.GetCurrentIncomeLevel(num),
			TargetIncomePosition = num,
			IncomePositions = game.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.IncomeMarkerPosition),
			IncomeLevels = game.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.IncomeLevel),
			PlayerAvatars = game.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.PlayerAvatar)
		};
		_router.ShowView<ILoanActionView>();
		_router.GetView<ILoanActionView>().ShowIncomePanel(loanActionViewModel, isDelayed: true);
	}

	public void HideIncomePanel()
	{
		_router.HideView<ILoanActionView>();
	}

	public void ShowTooltip(string element, float x, float y, string playerName, PlayerColor? playerColor)
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
		switch (element)
		{
		case "Token":
			text = playerName;
			break;
		case "Income":
			text = _localization.GetTranslation("Tooltips/EARNINGSTRACK/INCOME");
			break;
		case "IncomePoints":
			text = _localization.GetTranslation("Tooltips/EARNINGSTRACK/INCOMEPOINTS");
			break;
		}
		zero = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(1f, 0.5f);
		_router.GetView<ITooltipView>().ShowTooltip(text, zero, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}

	public void ReplayEvents(ReadOnlyCollection<IEvent> events)
	{
	}
}
