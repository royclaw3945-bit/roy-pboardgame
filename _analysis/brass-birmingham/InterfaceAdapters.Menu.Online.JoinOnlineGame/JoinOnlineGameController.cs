using BrassApplication.Game;
using BrassApplication.UseCases.Menu.OnlineGame.JoinOnlineGame;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Online.JoinOnlineGame;

public class JoinOnlineGameController : IController
{
	private readonly IJoinOnlineGameInput _createOnlineGameUseCase;

	public JoinOnlineGameController(IJoinOnlineGameInput createOnlineGameUseCase)
	{
		_createOnlineGameUseCase = createOnlineGameUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void SetGameToJoin(GameMetadata gameMetadata)
	{
		_createOnlineGameUseCase.StartJoinProcedure(gameMetadata);
	}

	public void BackClicked()
	{
		_createOnlineGameUseCase.BackClicked();
	}

	public void JoinGameClicked(int slot)
	{
		_createOnlineGameUseCase.ShowChoosePortraitAndJoinView(slot);
	}
}
