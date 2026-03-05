using BrassApplication.UseCases.Game.PlayerActions;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayerActions;

public class PlayerActionsController : IController
{
	private readonly IPlayerActionsInput _useCase;

	public PlayerActionsController(IPlayerActionsInput playerActionsUseCase)
	{
		_useCase = playerActionsUseCase;
	}

	public void OnViewCreated()
	{
		UpdatePlayerActions();
	}

	public void UpdatePlayerActions()
	{
		_useCase.UpdatePlayerActions();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnBuildActionClicked()
	{
		_useCase.StartBuildAction();
	}

	public void OnLinkActionClicked()
	{
		_useCase.StartNetworkAction();
	}

	public void OnDevelopActionClicked()
	{
		_useCase.StartDevelopAction();
	}

	public void OnSellActionClicked()
	{
		_useCase.StartSellAction();
	}

	public void OnLoanActionClicked()
	{
		_useCase.StartLoanAction();
	}

	public void OnScoutActionClicked()
	{
		_useCase.StartScoutAction();
	}

	public void OnPassActionClicked()
	{
		_useCase.StartPassAction();
	}

	public void OnUndoActionClicked()
	{
		_useCase.UndoAction();
	}

	public void OnEndTurnClicked()
	{
		_useCase.EndTurn();
	}

	public void OnMouseEnter(string actionButton, float buttonPositionX, float buttonPositionY, int screenWidth)
	{
		_useCase.ShowTooltip(actionButton, buttonPositionX, buttonPositionY, screenWidth);
	}

	public void OnMouseExit()
	{
		_useCase.HideTooltip();
	}
}
