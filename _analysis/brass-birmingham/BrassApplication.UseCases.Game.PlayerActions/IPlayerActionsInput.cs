namespace BrassApplication.UseCases.Game.PlayerActions;

public interface IPlayerActionsInput
{
	void UpdatePlayerActions();

	void StartBuildAction();

	void StartNetworkAction();

	void StartDevelopAction();

	void StartSellAction();

	void StartLoanAction();

	void StartScoutAction();

	void StartPassAction();

	void UndoAction();

	void EndTurn();

	void ShowTooltip(string actionButton, float buttonPositionX, float buttonPositionY, int screenWidth);

	void HideTooltip();
}
