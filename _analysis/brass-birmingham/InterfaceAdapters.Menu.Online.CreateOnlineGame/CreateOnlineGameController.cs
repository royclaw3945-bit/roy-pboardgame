using BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Online.CreateOnlineGame;

public class CreateOnlineGameController : IController
{
	private readonly ICreateOnlineGameInput _createOnlineGameUseCase;

	public CreateOnlineGameController(ICreateOnlineGameInput createOnlineGameUseCase)
	{
		_createOnlineGameUseCase = createOnlineGameUseCase;
	}

	public void OnViewCreated()
	{
		_createOnlineGameUseCase.CreateNewGameConfiguration();
	}

	public void OnViewDestroyed()
	{
	}

	public void BackClicked()
	{
		_createOnlineGameUseCase.Back();
	}

	public void StartGameClicked()
	{
		_createOnlineGameUseCase.StartGame();
	}

	public void ChangeNameClicked(string name)
	{
		_createOnlineGameUseCase.ChangeGameName(name);
	}

	public void ChangeTimeClicked()
	{
		_createOnlineGameUseCase.ChangeTime();
	}

	public void ChangePasswordClicked()
	{
		_createOnlineGameUseCase.ChangePassword();
	}
}
