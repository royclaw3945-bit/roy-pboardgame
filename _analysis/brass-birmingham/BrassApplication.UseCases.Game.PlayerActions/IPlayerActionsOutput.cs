using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.PlayerActions;

public interface IPlayerActionsOutput
{
	void UpdateAvailableActions();

	void UpdateActionsCounter(BrassApplicationGame game);

	void UpdateUndoButtonState(bool isUndoActionAvailable);

	void UpdateGamePresentation(PlayerMetadata replayWatchingPlayer);

	void ShowReasonsWhyCantUseBuildAction(List<CantUseBuildActionReason> reasons);

	void ShowReasonsWhyCantUseLinkAction(List<CantUseLinkActionReason> reasons);

	void ShowReasonsWhyCantUseDevelopAction(List<CantUseDevelopActionReason> reasons);

	void ShowReasonsWhyCantUseSellAction(List<CantUseSellActionReason> reasons);

	void ShowReasonsWhyCantUseLoanAction(List<CantUseLoanActionReason> reasons);

	void ShowReasonsWhyCantUseScoutAction(List<CantUseScoutActionReason> reasons);

	void StartBuildAction();

	void StartNetworkAction();

	void StartDevelopAction();

	void StartSellAction();

	void StartLoanAction();

	void StartScoutAction();

	void StartPassAction();

	void EndHumanTurn();

	void ShowTooltip(string actionButton, float buttonPositionX, float buttonPositionY, int screenWidth);

	void HideTooltip();
}
