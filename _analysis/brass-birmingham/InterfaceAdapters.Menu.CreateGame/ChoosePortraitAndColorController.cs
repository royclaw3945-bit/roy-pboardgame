using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Menu.ChoosePortraitAndColor;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.CreateGame;

public class ChoosePortraitAndColorController : IController
{
	private readonly IChoosePortraitAndColorInput _useCase;

	public ChoosePortraitAndColorController(IChoosePortraitAndColorInput choosePortraitAndColorUseCase)
	{
		_useCase = choosePortraitAndColorUseCase;
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

	public void OnNextColorClicked(PlayerColor playerColor, bool isOnlineGame = false)
	{
		_useCase.NextColorClicked(playerColor, isOnlineGame);
	}

	public void OnPreviousColorClicked(PlayerColor playerColor, bool isOnlineGame = false)
	{
		_useCase.PreviousColorClicked(playerColor, isOnlineGame);
	}

	public void OnNextPortraitClicked(PlayerAvatar avatar, bool isOnlineGame = false)
	{
		_useCase.NextPortraitClicked(avatar, isOnlineGame);
	}

	public void OnPreviousPortraitClicked(PlayerAvatar avatar, bool isOnlineGame = false)
	{
		_useCase.PreviousPortraitClicked(avatar, isOnlineGame);
	}
}
