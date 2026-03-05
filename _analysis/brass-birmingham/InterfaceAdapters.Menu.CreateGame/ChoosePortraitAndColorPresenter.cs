using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Menu.ChoosePortraitAndColor;
using BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;
using BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Menu.CreateGame;

public class ChoosePortraitAndColorPresenter : IPresenter, IChoosePortraitAndColorOutput
{
	private readonly IRouter _router;

	public ChoosePortraitAndColorPresenter(IRouter router)
	{
		_router = router;
	}

	public void HideView()
	{
		_router.HideView<IChoosePortraitAndColorView>();
	}

	public void NextColorFor(PlayerColor startingColor, PlayerColor color, bool isOnlineGame = false)
	{
		if (isOnlineGame)
		{
			_router.UseCaseBuilder.GetUseCase<CreateOnlineGameUseCase>().NextColorFor(startingColor, color);
		}
		else
		{
			_router.UseCaseBuilder.GetUseCase<CreateOfflineGameUseCase>().NextColorFor(startingColor, color);
		}
	}

	public void PreviousColorFor(PlayerColor startingColor, PlayerColor color, bool isOnlineGame = false)
	{
		if (isOnlineGame)
		{
			_router.UseCaseBuilder.GetUseCase<CreateOnlineGameUseCase>().PreviousColorFor(startingColor, color);
		}
		else
		{
			_router.UseCaseBuilder.GetUseCase<CreateOfflineGameUseCase>().PreviousColorFor(startingColor, color);
		}
	}

	public void NextPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar avatar, bool isOnlineGame = false)
	{
		if (isOnlineGame)
		{
			_router.UseCaseBuilder.GetUseCase<CreateOnlineGameUseCase>().NextPortraitFor(avatar);
		}
		else
		{
			_router.UseCaseBuilder.GetUseCase<CreateOfflineGameUseCase>().NextPortraitFor(startingAvatar, avatar);
		}
	}

	public void PreviousPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar avatar, bool isOnlineGame = false)
	{
		if (isOnlineGame)
		{
			_router.UseCaseBuilder.GetUseCase<CreateOnlineGameUseCase>().PreviousPortraitFor(startingAvatar, avatar);
		}
		else
		{
			_router.UseCaseBuilder.GetUseCase<CreateOfflineGameUseCase>().PreviousPortraitFor(startingAvatar, avatar);
		}
	}
}
