using BrassApplication.UseCases.Game.GameLog;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.GameLogView;

public class GameLogController : IController
{
	private readonly IGameLogInput _useCase;

	public GameLogController(IGameLogInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
		_useCase.UpdateGameLog();
	}

	public void OnBackButtonClicked()
	{
		_useCase.CloseGameLog();
	}

	public void OnViewDestroyed()
	{
	}
}
