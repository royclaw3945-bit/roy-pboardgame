using System.Collections.Generic;
using System.Numerics;
using System.Text;
using BoardGameRules.Entities.Actions;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BuildAction;
using BrassApplication.UseCases.Game.Actions.DevelopAction;
using BrassApplication.UseCases.Game.Actions.LoanAction;
using BrassApplication.UseCases.Game.Actions.NetworkAction;
using BrassApplication.UseCases.Game.Actions.PassAction;
using BrassApplication.UseCases.Game.Actions.ScoutAction;
using BrassApplication.UseCases.Game.Actions.SellAction;
using BrassApplication.UseCases.Game.PlayGame;
using BrassApplication.UseCases.Game.PlayerActions;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.PlayerActions;

public class PlayerActionsPresenter : IPresenter, IPlayerActionsOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public PlayerActionsPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void UpdateAvailableActions()
	{
	}

	public void UpdateActionsCounter(BrassApplicationGame game)
	{
		PlayerActionsViewModel viewModel = new PlayerActionsViewModel
		{
			Is2ndActionPresent = (game.Game.TurnFlow.GetMaxActionsCount() == 2),
			Is1stActionAvailable = (game.Game.TurnFlow.GetRemainingActionsCount() - game.Game.TurnFlow.GetMaxActionsCount() == 0),
			Is2ndActionAvailable = (game.Game.TurnFlow.GetRemainingActionsCount() > 0)
		};
		_router.GetView<IPlayerActionsView>().UpdateActionCounter(viewModel);
	}

	public void UpdateUndoButtonState(bool isUndoActionAvailable)
	{
		_router.GetView<IPlayerActionsView>().UpdateUndoButtonState(isUndoActionAvailable);
	}

	public void UpdateGamePresentation(PlayerMetadata replayWatchingPlayer)
	{
		_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().UpdateGamePresentation(replayWatchingPlayer);
	}

	public void ShowReasonsWhyCantUseBuildAction(List<CantUseBuildActionReason> reasons)
	{
		ShowInfoPopupWithReasons(reasons);
	}

	public void ShowReasonsWhyCantUseLinkAction(List<CantUseLinkActionReason> reasons)
	{
		ShowInfoPopupWithReasons(reasons);
	}

	public void ShowReasonsWhyCantUseDevelopAction(List<CantUseDevelopActionReason> reasons)
	{
		ShowInfoPopupWithReasons(reasons);
	}

	public void ShowReasonsWhyCantUseSellAction(List<CantUseSellActionReason> reasons)
	{
		ShowInfoPopupWithReasons(reasons);
	}

	public void ShowReasonsWhyCantUseLoanAction(List<CantUseLoanActionReason> reasons)
	{
		ShowInfoPopupWithReasons(reasons);
	}

	public void ShowReasonsWhyCantUseScoutAction(List<CantUseScoutActionReason> reasons)
	{
		ShowInfoPopupWithReasons(reasons);
	}

	public void StartBuildAction()
	{
		_router.UseCaseBuilder.GetUseCase<BuildActionUseCase>().StartBuildAction();
	}

	public void StartNetworkAction()
	{
		_router.UseCaseBuilder.GetUseCase<NetworkActionUseCase>().StartNetworkAction();
	}

	public void StartDevelopAction()
	{
		_router.UseCaseBuilder.GetUseCase<DevelopActionUseCase>().StartDevelopAction();
	}

	public void StartSellAction()
	{
		_router.UseCaseBuilder.GetUseCase<SellActionUseCase>().StartSellAction();
	}

	public void StartLoanAction()
	{
		_router.UseCaseBuilder.GetUseCase<LoanActionUseCase>().StartLoanAction();
	}

	public void StartScoutAction()
	{
		_router.UseCaseBuilder.GetUseCase<ScoutActionUseCase>().StartScoutAction();
	}

	public void StartPassAction()
	{
		_router.UseCaseBuilder.GetUseCase<PassActionUseCase>().StartPassAction();
	}

	public void EndHumanTurn()
	{
		_router.UseCaseBuilder.GetUseCase<PlayGameUseCase>().EndTurn();
	}

	public void ShowTooltip(string actionButton, float buttonPositionX, float buttonPositionY, int screenWidth)
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
		Vector3 position = Vector3.Zero;
		switch (actionButton)
		{
		case "Build":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/BUILD");
			position = new Vector3(buttonPositionX, buttonPositionY, 0f);
			break;
		case "LinkCanal":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/NETWORKCANAL");
			position = new Vector3(buttonPositionX, buttonPositionY, 0f);
			break;
		case "LinkRail":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/NETWORKRAIL");
			position = new Vector3(buttonPositionX, buttonPositionY, 0f);
			break;
		case "Develop":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/DEVELOP");
			position = new Vector3(buttonPositionX, buttonPositionY, 0f);
			break;
		case "Sell":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/SELL");
			position = new Vector3(buttonPositionX, buttonPositionY, 0f);
			break;
		case "Loan":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/LOAN");
			position = new Vector3(buttonPositionX - (float)(screenWidth / 30), buttonPositionY, 0f);
			break;
		case "Scout":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/SCOUT");
			position = new Vector3(buttonPositionX - (float)(screenWidth / 20), buttonPositionY, 0f);
			break;
		case "Pass":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/PASS");
			position = new Vector3(buttonPositionX - (float)(screenWidth / 13), buttonPositionY, 0f);
			break;
		case "Undo":
			text = _localization.GetTranslation("Tooltips/HUD/ACTIONS/UNDO");
			position = new Vector3(buttonPositionX, buttonPositionY, 0f);
			break;
		}
		Vector2 pivot = new Vector2(0.5f, 0f);
		_router.GetView<ITooltipView>().ShowTooltip(text, position, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}

	private void ShowInfoPopupWithReasons<T>(List<T> reasons)
	{
		StringBuilder stringBuilder = new StringBuilder("", 40);
		foreach (T reason in reasons)
		{
			stringBuilder.AppendLine(_localization.GetTranslation("ActionReasons/" + reason));
		}
		_router.GetView<IInfoPopupView>().Setup(stringBuilder.ToString(), _localization.GetTranslation("UI/OK"), delegate
		{
			_router.HideView<IInfoPopupView>();
		});
		_router.ShowView<IInfoPopupView>();
	}
}
