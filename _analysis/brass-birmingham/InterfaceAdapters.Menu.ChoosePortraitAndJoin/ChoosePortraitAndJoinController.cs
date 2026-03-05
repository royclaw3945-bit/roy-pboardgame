using BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.ChoosePortraitAndJoin;

public class ChoosePortraitAndJoinController : IController
{
	private readonly IChoosePortraitAndJoinInput _useCase;

	public ChoosePortraitAndJoinController(IChoosePortraitAndJoinInput choosePortraitAndJoinUseCase)
	{
		_useCase = choosePortraitAndJoinUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void BackClicked()
	{
		_useCase.BackClicked();
	}

	public void OnNextPortraitClicked()
	{
		_useCase.NextPortraitClicked();
	}

	public void OnPreviousPortraitClicked()
	{
		_useCase.PreviousPortraitClicked();
	}

	public void OnJoinClicked()
	{
		_useCase.JoinGame();
	}
}
